# Firmeza Web API

RESTful API built with ASP.NET Core for the Firmeza e-commerce application. This API provides secure endpoints for authentication, product management, client management, and sales operations.

## Technologies Used

- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Database
- **JWT (JSON Web Tokens)** - Authentication and authorization
- **Swagger/OpenAPI** - API documentation
- **AutoMapper** - Object-to-object mapping
- **BCrypt.Net** - Password hashing
- **DotNetEnv** - Environment variable management

## Project Architecture

```
Firmeza.WebApi/
├── Controllers/           # API endpoint controllers
│   ├── AuthController.cs          # Authentication (login, register, role assignment)
│   ├── ClientsController.cs       # Client CRUD operations
│   ├── ProductsController.cs      # Product CRUD operations
│   ├── SalesController.cs         # Sales transaction management
│   ├── SaleItemsController.cs     # Sale line items management
│   └── CustomerEmailController.cs # Email notification service
├── JwtConfiguration.cs    # JWT authentication setup
├── Program.cs             # Application entry point and DI configuration
├── appsettings.json       # Application configuration
└── wwwroot/               # Static files (receipts, uploads)
```

## API Endpoints

### Authentication Endpoints (`/api/auth`)

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | User login | No |
| POST | `/api/auth/assign-role` | Assign role to user | Admin |
| POST | `/api/auth/logout` | User logout | Yes |

### Products Endpoints (`/api/products`)

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/products` | Get all products | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| POST | `/api/products` | Create new product | Admin |
| PUT | `/api/products/{id}` | Update product | Admin |
| DELETE | `/api/products/{id}` | Delete product | Admin |

### Clients Endpoints (`/api/clients`)

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/clients` | Get all clients | Admin |
| GET | `/api/clients/{id}` | Get client by ID | Admin/Owner |
| POST | `/api/clients` | Create new client | No |
| PUT | `/api/clients/{id}` | Update client | Admin/Owner |
| DELETE | `/api/clients/{id}` | Delete client | Admin/Owner |

### Sales Endpoints (`/api/sales`)

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/sales` | Get all sales | Admin |
| GET | `/api/sales/{id}` | Get sale by ID | Admin/Owner |
| POST | `/api/sales` | Create new sale | Yes |
| PUT | `/api/sales/{id}` | Update sale | Admin |
| DELETE | `/api/sales/{id}` | Delete sale | Admin |

### Sale Items Endpoints (`/api/saleitems`)

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/saleitems` | Get all sale items | Admin |
| GET | `/api/saleitems/{id}` | Get sale item by ID | Admin |
| POST | `/api/saleitems` | Create sale item | Yes |
| PUT | `/api/saleitems/{id}` | Update sale item | Admin |
| DELETE | `/api/saleitems/{id}` | Delete sale item | Admin |

## How to Run

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL database
- Environment variables configured (see `.env` file)

### Running Locally

1. **Navigate to the project directory:**
   ```bash
   cd Firmeza.WebApi
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the API:**
   - API: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

### Running with Docker

The API is configured to run as part of the Docker Compose stack. See the main README for instructions.

## Configuration

The API requires the following environment variables:

- `DB_USER` - Database username
- `DB_PASS` - Database password
- `DB_NAME` - Database name
- `DB_PORT` - Database port
- `JWT_SECRET` - Secret key for JWT token generation
- `JWT_ISSUER` - JWT token issuer
- `JWT_AUDIENCE` - JWT token audience

## Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - BCrypt password encryption
- **CORS Policy** - Configured for React frontend
- **Sensitive Data Filter** - Global filter to prevent exposure of sensitive information
- **Role-based Authorization** - Admin and User roles

## API Documentation

When running in development mode, Swagger UI is available at `/swagger` for interactive API documentation and testing.

To authenticate in Swagger:
1. Use the `/api/auth/login` endpoint to get a JWT token
2. Click the "Authorize" button in Swagger UI
3. Enter `Bearer {your-token}` in the value field
4. Click "Authorize"

## Clean Architecture

This API follows Clean Architecture principles with clear separation of concerns:

- **Domain Layer** (`AdminDashboard.Domain`) - Business entities and rules
- **Application Layer** (`AdminDashboard.Application`) - Use cases, DTOs, interfaces
- **Infrastructure Layer** (`AdminDashboard.Infrastructure`) - Data access, external services
- **Identity Layer** (`AdminDashboard.Identity`) - Authentication and authorization
- **API Layer** (`Firmeza.WebApi`) - HTTP endpoints and API configuration
