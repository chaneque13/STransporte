using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticaTrailers.Models;

[Table("Trailer")]
public class Trailer
{
    [Key]
    [Column("id_trailer")]
    public int IdTrailer { get; set; }

    [Column("placas")]
    [Required(ErrorMessage = "Las placas son obligatorias.")]
    [StringLength(15)]
    public string Placas { get; set; } = string.Empty;

    [Column("capacidad_ton", TypeName = "decimal(6,2)")]
    [Range(0.1, 9999, ErrorMessage = "La capacidad debe ser mayor a 0 toneladas.")]
    public decimal CapacidadTon { get; set; }

    [Column("tipo_unidad")]
    [Required(ErrorMessage = "Seleccione el tipo de unidad.")]
    [StringLength(30)]
    public string TipoUnidad { get; set; } = string.Empty;

    [Column("estado")]
    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = Estados.Disponible;

    public List<Viaje> Viajes { get; set; } = new();
}
