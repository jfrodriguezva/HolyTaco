# HolyTac — Tacos & Bar

App de restaurante (taquería + bar/antro) estilo León: pedidos desde mesa, menú por categorías y coctelería, con backend de microservicios en .NET y frontend en Next.js.

## Diseño

Paleta elegida: **Terracota Taquería** (terracota, crema hueso, verde agave, naranja chile). Tipografía: `Fraunces` (display/elegante) + `Work Sans` (texto).

## Arquitectura

Clean Architecture por microservicio, orquestados detrás de un API Gateway (Ocelot):

```
Next.js (frontend/holytac-web)
        │  HTTP
        ▼
  HolyTac.Gateway (Ocelot)  ── /api/menu/**  ──▶  HolyTac.Menu.Api    (Menu.Domain / Application / Infrastructure)
        │                    ── /api/orders/** ──▶ HolyTac.Orders.Api  (Orders.Domain / Application / Infrastructure)
        ▼
  SQL Server (HolyTacMenuDb, HolyTacOrdersDb)
```

- **Domain**: entidades ricas (`MenuItem`, `Order`, `OrderItem`), sin dependencias externas.
- **Application**: casos de uso con **MediatR** (commands/queries), contratos de repositorios y de clientes externos.
- **Infrastructure**: **Dapper** para acceso a datos, políticas de **Polly** para resiliencia (reintentos ante fallos transitorios de SQL Server, y retry + circuit breaker + timeout para las llamadas HTTP de Orders hacia Menu).
- **Api**: controllers finos que solo despachan a MediatR.
- **Gateway**: Ocelot enruta `/api/menu/*` y `/api/orders/*` a cada microservicio, con QoS (circuit breaker) vía `Ocelot.Provider.Polly`.

`Orders` no accede a la base de datos de `Menu`: valida cada producto llamando al microservicio de Menú vía HTTP (bounded contexts independientes).

## Autenticación de staff

El Gateway emite y valida JWT (`POST /api/auth/login`) para las acciones de staff — confirmar/cancelar
reservaciones, crear/desactivar promociones, dar de alta platillos. Las lecturas (menú, promociones,
disponibilidad) y la creación de pedidos/reservaciones por parte del cliente siguen siendo públicas.

Cuentas demo (configuradas en `appsettings.json` del Gateway, solo para desarrollo):

| Usuario    | Password       | Rol      | Puede |
|------------|----------------|----------|-------|
| `mesero1`  | `Mesero123!`   | Mesero   | Confirmar/cancelar reservaciones, avanzar/cancelar pedidos |
| `cocina1`  | `Cocina123!`   | Cocina   | Avanzar/cancelar pedidos |
| `gerente1` | `Gerente123!`  | Gerente  | Todo lo anterior + crear/desactivar promociones, dar de alta platillos |

El login está limitado a 5 intentos/minuto (protección básica contra fuerza bruta), y el Gateway
solo acepta CORS desde los orígenes listados en `Cors:AllowedOrigins` (por defecto `:3000` y `:3001`).

## Requisitos

- .NET SDK 10
- Node.js 20+
- SQL Server (local, Docker o Azure SQL)

## Cómo correrlo

### 1. Base de datos

Con Docker:

```bash
docker compose up -d
```

Si ya tienes una instancia local de SQL Server (como en este entorno de desarrollo), no necesitas Docker: los `appsettings.json` usan `Trusted_Connection=True` (autenticación de Windows) contra `Server=localhost` por defecto. Si prefieres SQL Auth con Docker, cambia la cadena de conexión a `User Id=sa;Password=...` como en `docker-compose.yml`.

Luego ejecuta los scripts en orden (con `sqlcmd`, Azure Data Studio o SSMS):

```
database/scripts/01_create_menu_db.sql
database/scripts/02_create_orders_db.sql
database/scripts/03_seed_menu.sql
```

> Si usas `sqlcmd` en Windows, ejecuta el seed con `-f 65001` (codepage UTF-8) para que los acentos (ñ, é, etc.) no se guarden mal:
> `sqlcmd -S localhost -E -i database/scripts/03_seed_menu.sql -f 65001`

### 2. Backend

Cada servicio tiene su propia cadena de conexión en `appsettings.json` (por defecto apunta a `localhost,1433` / usuario `sa` / password `YourStrong!Passw0rd` — cámbiala si usas otra).

```bash
dotnet run --project src/Services/Menu/HolyTac.Menu.Api      # http://localhost:5101
dotnet run --project src/Services/Orders/HolyTac.Orders.Api  # http://localhost:5102
dotnet run --project src/Gateway/HolyTac.Gateway              # http://localhost:5100
```

O toda la solución con:

```bash
dotnet build HolyTac.slnx
```

### 3. Frontend

```bash
cd frontend/holytac-web
cp .env.local.example .env.local   # NEXT_PUBLIC_API_BASE_URL apunta al Gateway (5100)
npm install
npm run dev
```

Abre `http://localhost:3000`.

## Estructura

```
src/
  BuildingBlocks/HolyTac.SharedKernel      # Entity, IDbConnectionFactory
  Services/
    Menu/    HolyTac.Menu.{Domain,Application,Infrastructure,Api}
    Orders/  HolyTac.Orders.{Domain,Application,Infrastructure,Api}
  Gateway/HolyTac.Gateway                  # Ocelot
database/scripts/                          # DDL + seed SQL Server
frontend/holytac-web/                      # Next.js (App Router) + Tailwind v4
```

## Próximos pasos sugeridos

- Panel de cocina/barra en tiempo real (SignalR) para el avance de estatus de pedidos.
- CRUD completo de menú (editar/deshabilitar platillos) desde un panel admin.
- Tests unitarios de los handlers de MediatR y de las entidades de dominio.
