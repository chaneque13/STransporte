using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticaTrailers.Models;

[Table("Carga")]
public class Carga
{
    [Key]
    [Column("id_carga")]
    public int IdCarga { get; set; }

    [Column("id_viaje")]
    public int IdViaje { get; set; }

    [Column("descripcion")]
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(150)]
    public string Descripcion { get; set; } = string.Empty;

    [Column("peso_kg", TypeName = "decimal(8,2)")]
    [Range(0.01, 999999, ErrorMessage = "El peso debe ser mayor a 0 kg.")]
    public decimal PesoKg { get; set; }

    [Column("valor_declarado", TypeName = "decimal(10,2)")]
    [Range(0, 99999999, ErrorMessage = "El valor declarado no puede ser negativo.")]
    public decimal ValorDeclarado { get; set; }

    public Viaje? Viaje { get; set; }
}
