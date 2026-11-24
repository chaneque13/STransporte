namespace LogisticaTrailers.Models;

/// <summary>Valores permitidos para los campos "estado" del sistema.</summary>
public static class Estados
{
    public const string Disponible = "Disponible";

    // Trailer
    public const string TrailerEnRuta = "En ruta";
    public const string TrailerMantenimiento = "En mantenimiento";
    public static readonly string[] Trailer = { Disponible, TrailerEnRuta, TrailerMantenimiento };

    // Conductor
    public const string ConductorEnViaje = "En viaje";
    public const string ConductorDescanso = "De descanso";
    public static readonly string[] Conductor = { Disponible, ConductorEnViaje, ConductorDescanso };

    // Viaje
    public const string ViajeProgramado = "Programado";
    public const string ViajeEnTransito = "En tránsito";
    public const string ViajeEntregado = "Entregado";
    public const string ViajeCancelado = "Cancelado";
    public static readonly string[] Viaje = { ViajeProgramado, ViajeEnTransito, ViajeEntregado, ViajeCancelado };

    /// <summary>Estados en los que un viaje ocupa trailer y conductor.</summary>
    public static readonly string[] ViajeActivo = { ViajeProgramado, ViajeEnTransito };

    // Tipos de unidad
    public static readonly string[] TiposUnidad = { "Caja seca", "Refrigerado", "Plataforma", "Tanque", "Góndola" };
}
