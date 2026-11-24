using LogisticaTrailers.Data;
using LogisticaTrailers.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticaTrailers.Services;

/// <summary>Reglas de negocio de viajes y cargas (validaciones del planteamiento del problema).</summary>
public class ViajeService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ViajeService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    /// <summary>Devuelve la lista de errores de negocio; vacía si el viaje es válido.</summary>
    public async Task<List<string>> ValidarAsync(Viaje v)
    {
        var errores = new List<string>();

        if (v.FechaLlegada <= v.FechaSalida)
            errores.Add("La fecha de llegada debe ser posterior a la fecha de salida.");

        await using var db = await _factory.CreateDbContextAsync();

        var trailer = await db.Trailers.AsNoTracking().FirstOrDefaultAsync(t => t.IdTrailer == v.IdTrailer);
        var conductor = await db.Conductores.AsNoTracking().FirstOrDefaultAsync(c => c.IdConductor == v.IdConductor);

        if (trailer is null)
            errores.Add("El trailer seleccionado no existe.");
        else if (trailer.Estado == Estados.TrailerMantenimiento)
            errores.Add($"El trailer {trailer.Placas} está en mantenimiento y no se puede asignar.");

        if (conductor is null)
            errores.Add("El conductor seleccionado no existe.");
        else if (conductor.VigenciaLicencia.Date < v.FechaLlegada.Date)
            errores.Add($"La licencia de {conductor.Nombre} vence ({conductor.VigenciaLicencia:dd/MM/yyyy}) antes de la llegada del viaje.");

        if (errores.Count == 0)
        {
            var activos = Estados.ViajeActivo;
            var salida = v.FechaSalida;
            var llegada = v.FechaLlegada;

            bool trailerOcupado = await db.Viajes.AnyAsync(x =>
                x.IdViaje != v.IdViaje && x.IdTrailer == v.IdTrailer &&
                activos.Contains(x.EstadoViaje) &&
                x.FechaSalida < llegada && x.FechaLlegada > salida);
            if (trailerOcupado)
                errores.Add("El trailer ya está asignado a otro viaje en ese rango de fechas.");

            bool conductorOcupado = await db.Viajes.AnyAsync(x =>
                x.IdViaje != v.IdViaje && x.IdConductor == v.IdConductor &&
                activos.Contains(x.EstadoViaje) &&
                x.FechaSalida < llegada && x.FechaLlegada > salida);
            if (conductorOcupado)
                errores.Add("El conductor ya está asignado a otro viaje en ese rango de fechas.");
        }

        return errores;
    }

    /// <summary>Inserta o actualiza un viaje ya validado.</summary>
    public async Task GuardarAsync(Viaje v)
    {
        await using var db = await _factory.CreateDbContextAsync();
        if (v.IdViaje == 0)
        {
            v.EstadoViaje = Estados.ViajeProgramado;
            db.Viajes.Add(v);
        }
        else
        {
            // Solo se actualizan los campos editables; el estado cambia con CambiarEstadoAsync.
            var actual = await db.Viajes.FirstAsync(x => x.IdViaje == v.IdViaje);
            actual.IdCliente = v.IdCliente;
            actual.IdTrailer = v.IdTrailer;
            actual.IdConductor = v.IdConductor;
            actual.Origen = v.Origen;
            actual.Destino = v.Destino;
            actual.FechaSalida = v.FechaSalida;
            actual.FechaLlegada = v.FechaLlegada;
        }
        await db.SaveChangesAsync();
    }

    /// <summary>Cambia el estado del viaje respetando el flujo permitido. Devuelve un error o null.</summary>
    public async Task<string?> CambiarEstadoAsync(int idViaje, string nuevoEstado)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var viaje = await db.Viajes
            .Include(x => x.Trailer).Include(x => x.Conductor)
            .FirstOrDefaultAsync(x => x.IdViaje == idViaje);
        if (viaje is null) return "El viaje no existe.";

        bool permitido = (viaje.EstadoViaje, nuevoEstado) switch
        {
            (Estados.ViajeProgramado, Estados.ViajeEnTransito) => true,
            (Estados.ViajeProgramado, Estados.ViajeCancelado) => true,
            (Estados.ViajeEnTransito, Estados.ViajeEntregado) => true,
            (Estados.ViajeEnTransito, Estados.ViajeCancelado) => true,
            _ => false
        };
        if (!permitido)
            return $"No se puede pasar un viaje de '{viaje.EstadoViaje}' a '{nuevoEstado}'.";

        if (nuevoEstado == Estados.ViajeEnTransito)
        {
            if (viaje.Trailer!.Estado == Estados.TrailerMantenimiento)
                return "El trailer está en mantenimiento; no se puede iniciar el viaje.";
            viaje.Trailer.Estado = Estados.TrailerEnRuta;
            viaje.Conductor!.Estado = Estados.ConductorEnViaje;
        }
        else
        {
            // Entregado o Cancelado: se liberan las unidades que estaban ocupadas por este viaje.
            if (viaje.Trailer!.Estado == Estados.TrailerEnRuta) viaje.Trailer.Estado = Estados.Disponible;
            if (viaje.Conductor!.Estado == Estados.ConductorEnViaje) viaje.Conductor.Estado = Estados.Disponible;
        }

        viaje.EstadoViaje = nuevoEstado;
        await db.SaveChangesAsync();
        return null;
    }

    /// <summary>Agrega una carga a un viaje validando que no exceda la capacidad del trailer.</summary>
    public async Task<string?> AgregarCargaAsync(Carga carga)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var viaje = await db.Viajes
            .Include(x => x.Trailer).Include(x => x.Cargas)
            .FirstOrDefaultAsync(x => x.IdViaje == carga.IdViaje);
        if (viaje is null) return "El viaje no existe.";

        if (viaje.EstadoViaje is Estados.ViajeEntregado or Estados.ViajeCancelado)
            return "No se puede modificar la carga de un viaje entregado o cancelado.";

        var capacidadKg = viaje.Trailer!.CapacidadTon * 1000m;
        var totalKg = viaje.Cargas.Sum(c => c.PesoKg) + carga.PesoKg;
        if (totalKg > capacidadKg)
            return $"La carga total ({totalKg:N2} kg) excede la capacidad del trailer ({capacidadKg:N2} kg).";

        db.Cargas.Add(carga);
        await db.SaveChangesAsync();
        return null;
    }

    /// <summary>Elimina una carga si su viaje sigue abierto. Devuelve un error o null.</summary>
    public async Task<string?> EliminarCargaAsync(int idCarga)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var carga = await db.Cargas.Include(c => c.Viaje).FirstOrDefaultAsync(c => c.IdCarga == idCarga);
        if (carga is null) return null;
        if (carga.Viaje!.EstadoViaje is Estados.ViajeEntregado or Estados.ViajeCancelado)
            return "No se puede modificar la carga de un viaje entregado o cancelado.";
        db.Cargas.Remove(carga);
        await db.SaveChangesAsync();
        return null;
    }
}
