# Roadmap — HolyTac como herramienta completa

HolyTac arrancó como demo de comandas (Menu + Orders). El enfoque del producto cambió: la prioridad es
**marketing y reservaciones** — mostrar promociones vigentes y dejar reservar mesa fácilmente — dejando
las comandas como una pieza más del sistema, no la protagonista. Este documento traza cómo llegar a una
herramienta completa para operar el restaurante.

## Fase 1 — Promociones y Reservaciones (actual)

- Microservicios `Promotions` y `Reservations` (Clean Architecture, Dapper, Polly — mismo patrón que Menu/Orders).
- Cliente: páginas públicas `/promociones` y `/reservar`, home reorientado a marketing + booking.
- Staff: `/admin/promociones` y `/admin/reservaciones` **sin autenticación** (ver Fase 2).
- Reservaciones con mesas reales, disponibilidad por horario y asignación automática sin traslapes.
- Promociones informativas (no se conectan aún al cálculo del carrito de Orders — ver Fase 3).

## Fase 2 — Seguridad

- Autenticación (JWT) para todo `/admin/**` y para las mutaciones (`POST`/`PUT`/`DELETE`) de cada microservicio, verificada en el Gateway.
- Roles: mesero, cocina/barra, gerente — cada uno ve solo lo que le corresponde.
- Rate limiting y CORS restringido a dominios conocidos (hoy `AllowAnyOrigin` es solo para desarrollo).

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
