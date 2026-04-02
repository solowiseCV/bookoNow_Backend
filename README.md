# BookNow API

BookNow is a comprehensive mechanic workshop booking and discovery platform. This repository contains the backend API built with .NET 8, following Clean Architecture principles to ensure scalability, maintainability, and testability.

## Features

- **Workshop Management**: Create, update, and discover mechanic workshops.
- **Shop & Spare Parts**: Spare part sellers can create shops, list products, and manage orders.
- **Appointment Booking**: Client-side appointment booking with real-time mechanic location tracking.
- **Secure Authentication**: JWT-based authentication with support for Roles (Client, Mechanic, SparePartSeller, Admin) and Google OAuth.
- **Payment Integration**: Seamless payment processing via Paystack for appointments, orders, and workshop subscriptions.
- **Media Support**: Built-in support for image uploads (Hero and Gallery images) via Cloudinary.
- **API Documentation**: Fully documented interactive API using Swagger with concise annotations.

##  Architecture

The project follows the **Clean Architecture** pattern:

1. **BookNow.Domain**: Contains core entities, enums, and domain logic. (No dependencies)
2. **BookNow.Application**: Contains business logic, MediatR commands/queries, DTOs, and interfaces.
3. **BookNow.Infrastructure**: Implemented interfaces for persistence (EF Core), external services (Paystack, Cloudinary), and authentication.
4. **BookNow.Presentation**: The entry point (Web API) that handles HTTP requests and provides the Swagger UI.

##  Tech Stack

- **Framework**: .NET 8 (ASP.NET Core API)
- **Database**: SQL Server with Entity Framework Core
- **Messaging**: MediatR (CQRS Pattern)
- **Security**: ASP.NET Core Identity & JWT Bearer Authentication
- **Documentation**: Swashbuckle (Swagger) with Annotations
- **Cloud Services**: Cloudinary (Images), Paystack (Payments)

##  Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or instance)

### Installation & Setup

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd BookNow
   ```

2. **Configure appsettings.json**:
   Update `BookNow.Presentation/appsettings.json` with your connection strings and service keys (JWT, Cloudinary, Paystack, Google Auth).

3. **Restore & Build**:
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Apply Migrations**:
   ```bash
   cd BookNow.Presentation
   dotnet ef database update
   ```

5. **Run the API**:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (or your configured port). Access the Swagger UI at `/swagger` for interactive documentation.

##  API Documentation

All API responses are standardized into a consistent format:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": []
}
```

Detailed endpoint descriptions and request/response models are available in the **Swagger UI**.
