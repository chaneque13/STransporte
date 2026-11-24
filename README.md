# Sistema Web de Logística de Trailers

Aplicación **Blazor Server (.NET 10)** con **Entity Framework Core** y **SQL Server**, basada en el
documento *Planteamiento del Problema*.

> **Estado:** código escrito pero **no compilado ni probado** (el entorno donde se generó no tenía
> acceso a NuGet). Es normal que en la primera compilación aparezcan algunos errores menores; ver
> "Primer arranque".

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (el archivo `global.json` fija la
  versión 10 para que no se use otro SDK más nuevo instalado en el equipo)
- SQL Server (LocalDB, Express o completo). Con Visual Studio, LocalDB ya viene incluido.

## Primer arranque

```bash
cd LogisticaTrailers
dotnet restore
dotnet build
dotnet run
```

Abra la URL que muestre la consola (por ejemplo `https://localhost:7xxx`).

Al iniciar, la aplicación **crea la base de datos `LogisticaTrailers` automáticamente** y carga datos de
ejemplo (2 clientes, 3 trailers, 2 conductores). El script `Database/crear_base_de_datos.sql` es
opcional y equivale al modelo que crea la app.

### Cadena de conexión

Se configura en `appsettings.json` (`ConnectionStrings:DefaultConnection`). Por defecto usa LocalDB:

```
Server=(localdb)\MSSQLLocalDB;Database=LogisticaTrailers;Trusted_Connection=True;TrustServerCertificate=True
```

Para SQL Server Express o remoto, por ejemplo:

```
Server=localhost\SQLEXPRESS;Database=LogisticaTrailers;Trusted_Connection=True;TrustServerCertificate=True
Server=mi-servidor;Database=LogisticaTrailers;User Id=usuario;Password=clave;TrustServerCertificate=True
```

## Estructura

| Carpeta | Contenido |
|---|---|
| `Models/` | Entidades: `Cliente`, `Trailer`, `Conductor`, `Viaje`, `Carga` y constantes de estados |
| `Data/` | `AppDbContext` (relaciones) y `DbSeeder` (creación de BD y datos de ejemplo) |
| `Services/ViajeService.cs` | Reglas de negocio de viajes y cargas |
| `Components/Pages/` | Páginas: Inicio, Clientes, Trailers, Conductores, Viajes, Carga del viaje, Reportes |
| `Database/` | Script SQL opcional |

## Reglas de negocio implementadas

- La fecha de llegada debe ser posterior a la de salida.
- No se asigna un trailer **en mantenimiento**.
- La licencia del conductor debe estar vigente hasta la fecha de llegada del viaje.
- Un trailer o conductor no puede estar en dos viajes activos con fechas traslapadas.
- Flujo de estados del viaje: Programado → En tránsito → Entregado (o Cancelado). Al iniciar/terminar
  un viaje se actualiza el estado del trailer y del conductor.
- La carga total de un viaje no puede exceder la capacidad del trailer.
- No se puede eliminar un cliente, trailer o conductor que ya tenga viajes.

## Pendiente respecto al planteamiento

- **Autenticación y roles** (Administrador, Despachador, Conductor, Cliente): no incluida en esta versión.
  Todas las páginas son de acceso libre. Se recomienda agregarla con ASP.NET Core Identity.
- El rol "Cliente" de solo consulta depende de lo anterior.

## Migraciones (opcional)

Si el equipo quiere usar migraciones de EF Core en lugar de `EnsureCreated`:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Inicial
dotnet ef database update
```

y reemplazar `EnsureCreatedAsync()` en `DbSeeder` por `MigrateAsync()`.
