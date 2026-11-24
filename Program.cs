using LogisticaTrailers.Components;
using LogisticaTrailers.Data;
using LogisticaTrailers.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// En Blazor Server se recomienda IDbContextFactory: cada operación usa su propio DbContext
// de vida corta, evitando problemas de concurrencia dentro del circuito del usuario.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ViajeService>();

var app = builder.Build();

// Crea la base de datos (si no existe) y carga datos de ejemplo la primera vez.
await DbSeeder.InicializarAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
