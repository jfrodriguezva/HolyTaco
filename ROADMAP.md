# Roadmap — HolyTac como herramienta completa

HolyTac arrancó como demo de comandas (Menu + Orders). El enfoque del producto cambió: la prioridad es
**marketing y reservaciones** — mostrar promociones vigentes y dejar reservar mesa fácilmente — dejando
las comandas como una pieza más del sistema, no la protagonista. Este documento traza cómo llegar a una
herramienta completa para operar el restaurante.

## Fase 1 — Promociones y Reservaciones (actual)

- Microservicios `Promotions` y `Reservations` (Clean Architecture, Dapper, Polly — mismo patrón que Menu/Orders).
- Cliente: páginas públicas `/promociones` y `/reservar`, home reorientado a marketing + booking.
- Staff: `/admin/promociones` y `/admin/reservaciones`, protegidas por login (ver Fase 2 — completada).
- Reservaciones con mesas reales, disponibilidad por horario y asignación automática sin traslapes (con lock transaccional para evitar dobles reservas concurrentes).
- Promociones informativas (no se conectan aún al cálculo del carrito de Orders — ver Fase 3).

## Fase 2 — Seguridad ✅ completada

- Autenticación JWT emitida y validada en el Gateway (`POST /api/auth/login`), aplicada a las mutaciones de staff de cada microservicio vía `AuthenticationOptions` de Ocelot.
- Roles `Mesero`, `Cocina` y `Gerente`: cualquier staff autenticado puede confirmar/cancelar reservaciones y avanzar/cancelar pedidos; crear/desactivar promociones y dar de alta platillos requiere `Gerente` (`RouteClaimsRequirement` por ruta).
- `/admin/reservaciones` y `/admin/promociones` exigen sesión de staff en el frontend (`RequireStaffAuth`), con redirección a `/login`.
- Rate limiting (`Microsoft.AspNetCore.RateLimiting`): límite global de 120 req/10s por IP en el Gateway, más 5 intentos/min específicamente sobre `/api/auth/login`.
- CORS restringido a una lista de orígenes conocidos (`Cors:AllowedOrigins` en `appsettings.json`), ya no `AllowAnyOrigin`; y solo configurado en el Gateway (los microservicios ya no lo necesitan, nunca los toca un navegador directamente).
- Pendiente real de esta fase: hoy las cuentas de staff viven hardcodeadas en `appsettings.json` del Gateway (sin alta/baja de usuarios ni hash gestionado en base de datos) — suficiente para operar un solo restaurante, pero el primer paso obligado si esto crece a multi-sucursal o rotación de personal frecuente.

## Fase 3 — Comandas evolucionadas

- Pantalla de cocina/barra en tiempo real (SignalR) para el avance de estatus de `Order`.
- Estado de mesas unificado: `Reservations` marca una mesa como reservada/ocupada y `Orders` la puede consultar (hoy son bounded contexts independientes; se integrarían vía eventos o un servicio de "Tables" compartido).
- `Promotions` conectado a `Orders`: aplicar el descuento real de una promoción vigente al calcular el total del pedido.

## Fase 4 — Marketing avanzado

- Confirmación de reservación por email/SMS.
- Códigos de cupón ligados a una `Promotion` (un solo uso, por cliente, etc.).
- Programa de lealtad / puntos por visita.

## Fase 5 — Operación y calidad

- Health checks por servicio (`/health`) y agregación en el Gateway.
- Logging estructurado + OpenTelemetry (trazas entre Gateway → Menu/Orders/Reservations/Promotions).
- CI/CD (build + tests en cada push) y pipeline de migración de base de datos versionada (hoy son scripts SQL manuales).
- Tests unitarios de dominio y de los handlers de MediatR; tests de integración de los repositorios Dapper.
