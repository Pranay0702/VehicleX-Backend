# VehicleX Backend

Clean Architecture ASP.NET Core Web API for VehicleX coursework.

This branch implements **Feature 1: Sales Management** only.

## Implemented in this branch

- Create sales invoice
- Validate customer and part existence
- Validate available stock before sale
- Reduce stock after successful sale
- Calculate invoice totals
- Structured API responses
- Global exception handling middleware
- Input validation with proper HTTP status codes
- Swagger enabled and auto-open in development

## Architecture

- `VehicleX.Domain`: Entities
- `VehicleX.Application`: DTOs, interfaces, services, service result model
- `VehicleX.Infrastructure`: EF Core DbContext, repositories, migrations
- `VehicleX.API`: Controllers, middleware, startup/DI

## PostgreSQL Setup

1. Ensure PostgreSQL is running.
2. Create database:

```sql
CREATE DATABASE vehiclex_db;
```

3. Update connection string in `VehicleX.API/appsettings.json` if needed.

## Migration Commands

```bash
dotnet ef migrations add InitialSalesManagement --project VehicleX.Infrastructure --startup-project VehicleX.API --output-dir Migrations
dotnet ef database update --project VehicleX.Infrastructure --startup-project VehicleX.API
```

(Migration is already generated in this repo for this feature.)

## Run API

```bash
dotnet run --project VehicleX.API
```

Swagger opens automatically at:

- `http://localhost:5272/swagger`
- `https://localhost:7112/swagger`

## Sales API Endpoints

- `GET /api/sales/customers`
- `GET /api/sales/parts`
- `POST /api/sales/invoices`
- `GET /api/sales/invoices/{invoiceId}`

### Sample Create Invoice Request

```json
{
  "customerId": 1,
  "items": [
    {
      "partId": 1,
      "quantity": 2
    },
    {
      "partId": 2,
      "quantity": 1
    }
  ]
}
```

## Seed Data

Seed data is included via migration:

- Customers: `Ram Sharma`, `Sita Gurung`
- Parts: `Brake Pad Set`, `Air Filter`, `Engine Oil 5W-30`

