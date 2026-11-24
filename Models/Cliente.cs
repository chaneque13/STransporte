using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticaTrailers.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [Column("correo")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string? Correo { get; set; }

    [Column("direccion")]
    [StringLength(150)]
    public string? Direccion { get; set; }

    public List<Viaje> Viajes { get; set; } = new();
}
