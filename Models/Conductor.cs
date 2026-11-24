using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticaTrailers.Models;

[Table("Conductor")]
public class Conductor
{
    [Key]
    [Column("id_conductor")]
    public int IdConductor { get; set; }

    [Column("nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column("licencia")]
    [Required(ErrorMessage = "El número de licencia es obligatorio.")]
    [StringLength(20)]
    public string Licencia { get; set; } = string.Empty;

    [Column("vigencia_licencia", TypeName = "date")]
    public DateTime VigenciaLicencia { get; set; } = DateTime.Today.AddYears(1);

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [Column("estado")]
    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = Estados.Disponible;

    public List<Viaje> Viajes { get; set; } = new();
}
