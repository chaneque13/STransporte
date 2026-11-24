using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticaTrailers.Models;

[Table("Viaje")]
public class Viaje
{
    [Key]
    [Column("id_viaje")]
    public int IdViaje { get; set; }

    [Column("id_cliente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    public int IdCliente { get; set; }

    [Column("id_trailer")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un trailer.")]
    public int IdTrailer { get; set; }

    [Column("id_conductor")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un conductor.")]
    public int IdConductor { get; set; }

    [Column("origen")]
    [Required(ErrorMessage = "El origen es obligatorio.")]
    [StringLength(100)]
    public string Origen { get; set; } = string.Empty;

    [Column("destino")]
    [Required(ErrorMessage = "El destino es obligatorio.")]
    [StringLength(100)]
    public string Destino { get; set; } = string.Empty;

    [Column("fecha_salida")]
    public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1).AddHours(8);

    [Column("fecha_llegada")]
    public DateTime FechaLlegada { get; set; } = DateTime.Today.AddDays(2).AddHours(8);

    [Column("estado_viaje")]
    [Required]
    [StringLength(20)]
    public string EstadoViaje { get; set; } = Estados.ViajeProgramado;

    public Cliente? Cliente { get; set; }
    public Trailer? Trailer { get; set; }
    public Conductor? Conductor { get; set; }
    public List<Carga> Cargas { get; set; } = new();
}
