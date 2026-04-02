# TravelingApp Backend

API REST para la gestion de viajes, desarrollada con .NET 10 y arquitectura Clean Architecture + CQRS.

## Tecnologias

- .NET 10 LTS (C#)
- CQRS con MediatR
- Entity Framework Core + PostgreSQL
- ASP.NET Core Identity (autenticacion JWT)
- FluentValidation
- AutoMapper
- Redis (cache distribuido)
- Swagger/OpenAPI con Swashbuckle
- Docker + Docker Compose (Alpine)
- CI/CD con GitHub Actions

## Estructura del proyecto

```
src/
  TravelingApp.Domain/           Entidades de dominio
  TravelingApp.Application/      Handlers CQRS, DTOs, interfaces, behaviors, config
  TravelingApp.Infraestructure/  DbContext, migraciones, servicios (Redis, seed)
  TravelingApp.API/              Entry point: controllers, middleware, DI, Swagger
test/
  TravelingApp.UnitTest/         Tests unitarios con NUnit
.deploy/
  docker-compose.yml             Servicios base (app, PostgreSQL, Redis)
  docker-compose.override.yml    Configuracion (puertos, env, volumes, resources)
  .env                           Variables de entorno
.github/workflows/
  ci-cd.yml                      Pipeline CI/CD
```

## Requisitos

- .NET 10 SDK
- PostgreSQL (local o Docker)
- Redis (local o Docker)

## Configuracion

Los secretos (connection string, JWT key) se configuran via variables de entorno o `appsettings.Development.json`.
- PostgreSQL: `postgres` / `postgres` en puerto 5432
- Redis: `localhost:6379`

## Ejecucion local

```bash
dotnet run --project src/TravelingApp.API
```

La API estara disponible en `http://localhost:5090` con Swagger en `/swagger`.

## Docker

```bash
cd .deploy
docker compose up --build -d
```

| Servicio | Imagen | Puerto |
|----------|--------|--------|
| traveling-app | Alpine (.NET 10) | 5090 |
| postgres-traveling-app | postgres:18.3-alpine | 5434 |
| redis-traveling-app | redis:8.6.2-alpine | 6380 |

## Tests

```bash
dotnet test
```

## Endpoints principales

| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| POST | /api/Account/Login | No | Login, devuelve JWT |
| POST | /api/Account/Register | No | Registro de usuario |
| GET | /api/User/List | Bearer | Lista usuarios paginada |
| GET | /api/Destination/List | Bearer | Lista destinos paginada |
| GET | /api/Destination/GetById/{id} | Bearer | Detalle de destino |
| POST | /api/Destination/Create | Admin | Crear destino |
| PUT | /api/Destination/Update | Admin | Editar destino |
| DELETE | /api/Destination/Delete/{id} | Admin | Borrar destino |

## Credenciales seed

- Usuario: `admin` / `Admin123!` (rol Admin)
- Destinos: Tokio, Amazonas
