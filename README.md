# VehicleX Backend

ASP.NET Core Web API backend for the Vehicle Parts Selling and Inventory Management 

This implementation focuses on the assigned customer workflow features:

- Customers can book service appointments.
- Customers can request unavailable vehicle parts.
- Customers can review services.

## Architecture

The solution follows Clean Architecture with four projects:

- `VehicleX.Domain`: entities and enums.
- `VehicleX.Application`: DTOs, interfaces, and services.
- `VehicleX.Infrastructure`: EF Core `DbContext`, repositories, and migrations.
- `VehicleX.API`: controllers, middleware, and application startup.


## Database

Database provider: PostgreSQL

