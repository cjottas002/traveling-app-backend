# traveling-app-backend

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
- Docker + Docker Compose

## Estructura del proyecto

```
src/
  TravelingApp.Domain/           Entidades de dominio
  TravelingApp.Application/      Handlers CQRS, DTOs, interfaces, behaviors, config
  TravelingApp.Infraestructure/  DbContext, migraciones, servicios (Redis, seed)
  TravelingApp.Ui/               Entry point: controllers, middleware, DI, Swagger
test/
  TravelingApp.UnitTest/         Tests unitarios con NUnit
```

## Requisitos

- .NET 10 SDK
- PostgreSQL (local o Docker)
- Redis (local o Docker)

## Configuracion

Los secretos (connection string, JWT key) se configuran via User Secrets o variables de entorno.
El archivo `appsettings.Development.json` contiene los valores para desarrollo local.

## Ejecucion

```bash
dotnet run --project src/TravelingApp.Ui
```

La API estara disponible en `http://localhost:5000` con Swagger UI en la raiz.

## Docker

```bash
cd TravelingApp.Compose
docker compose up
```

Servicios: API (.NET 10), PostgreSQL 17, Redis 7.

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
