using LogisticaTrailers.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticaTrailers.Data;

public static class DbSeeder
{
    /// <summary>Crea la base si no existe y, si está vacía, carga datos de ejemplo.</summary>
    public static async Task InicializarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        await db.Database.EnsureCreatedAsync();

        if (await db.Clientes.AnyAsync()) return;

        db.Clientes.AddRange(
            new Cliente { Nombre = "Distribuidora del Pacífico", Telefono = "3121234567", Correo = "contacto@pacifico.example", Direccion = "Av. Insurgentes 100, Colima" },
            new Cliente { Nombre = "Agroindustrias del Bajío", Telefono = "4619876543", Correo = "logistica@agrobajio.example", Direccion = "Blvd. Industrial 45, Celaya" });

        db.Trailers.AddRange(
            new Trailer { Placas = "45-AB-1Z", CapacidadTon = 28, TipoUnidad = "Caja seca" },
            new Trailer { Placas = "78-CD-2Y", CapacidadTon = 24, TipoUnidad = "Refrigerado" },
            new Trailer { Placas = "12-EF-3X", CapacidadTon = 30, TipoUnidad = "Plataforma", Estado = Estados.TrailerMantenimiento });

        db.Conductores.AddRange(
            new Conductor { Nombre = "Juan Pérez Ramírez", Licencia = "LF-100234", VigenciaLicencia = DateTime.Today.AddYears(2), Telefono = "3125550101" },
            new Conductor { Nombre = "María López Torres", Licencia = "LF-100567", VigenciaLicencia = DateTime.Today.AddYears(1), Telefono = "3125550102" });

        await db.SaveChangesAsync();
    }
}
