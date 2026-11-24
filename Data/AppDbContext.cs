using LogisticaTrailers.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticaTrailers.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Trailer> Trailers => Set<Trailer>();
    public DbSet<Conductor> Conductores => Set<Conductor>();
    public DbSet<Viaje> Viajes => Set<Viaje>();
    public DbSet<Carga> Cargas => Set<Carga>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Un viaje no puede quedar huérfano: no se permite borrar un cliente, trailer
        // o conductor que ya tenga viajes registrados.
        mb.Entity<Viaje>()
            .HasOne(v => v.Cliente).WithMany(c => c.Viajes)
            .HasForeignKey(v => v.IdCliente).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Viaje>()
            .HasOne(v => v.Trailer).WithMany(t => t.Viajes)
            .HasForeignKey(v => v.IdTrailer).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Viaje>()
            .HasOne(v => v.Conductor).WithMany(c => c.Viajes)
            .HasForeignKey(v => v.IdConductor).OnDelete(DeleteBehavior.Restrict);

        // Si se elimina un viaje, se eliminan sus cargas.
        mb.Entity<Carga>()
            .HasOne(c => c.Viaje).WithMany(v => v.Cargas)
            .HasForeignKey(c => c.IdViaje).OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Trailer>().HasIndex(t => t.Placas).IsUnique();
        mb.Entity<Conductor>().HasIndex(c => c.Licencia).IsUnique();
    }
}
