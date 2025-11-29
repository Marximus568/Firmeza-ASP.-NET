# Firmeza E-Commerce Platform

Full-stack e-commerce application built with ASP.NET Core and React. This platform provides a complete solution for managing products, clients, and sales transactions with separate interfaces for customers and administrators.

## Project Overview

Firmeza is a modern e-commerce platform consisting of three main components:

1. **Web API** - RESTful API backend with JWT authentication
2. **Admin Dashboard** - ASP.NET Core Razor Pages management interface
3. **Frontend** - React SPA for customer shopping experience

## Architecture

```
Firmeza-ASP.-NET/
├── Firmeza.WebApi/              # RESTful API (ASP.NET Core)
├── AdminDashboard.Web/          # Admin interface (Razor Pages)
├── Front-end/                   # Customer interface (React + Vite)
├── AdminDashboard.Application/  # Application layer (use cases, DTOs)
├── AdminDashboard.Domain/       # Domain layer (entities, business rules)
├── AdminDashboard.Infrastructure/ # Infrastructure (data access, repositories)
├── AdminDashboard.Identity/     # Identity and authentication
├── ExcelImporter/               # Excel import/export services
├── SalePDF/                     # PDF generation services
├── SmtpSettings/                # Email notification services
├── Firmeza.Test/                # Unit and integration tests
├── compose.yaml                 # Docker Compose configuration
├── .env                         # Environment variables
└── README.md                    # This file
```

## Technologies Used

### Backend
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **ASP.NET Core Identity** - Authentication
- **JWT** - Token-based authentication
- **AutoMapper** - Object mapping
- **BCrypt.Net** - Password hashing
- **EPPlus** - Excel operations
- **PDF Generation** - Receipt generation

### Frontend
- **React 19** - UI library
- **Vite 7** - Build tool
- **React Router 7** - Routing
- **Zustand 5** - State management
- **Axios** - HTTP client

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **xUnit** - Testing framework
- **Nginx** - Web server (production)

## Getting Started with Docker Compose

The easiest way to run the entire application is using Docker Compose, which orchestrates all services including the database.

### Prerequisites

- **Docker** 20.10 or later
- **Docker Compose** 2.0 or later
- At least 4GB of available RAM
- Ports 5000, 5001, 5173, and 5432 available

### Environment Configuration

1. **Create or verify the `.env` file** in the project root:

```env
# Database Configuration
DB_USER=your_db_user
DB_PASS=your_db_password
DB_NAME=firmeza_db
DB_PORT=5432
POSTGRES_USER=your_db_user

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-min-32-chars
JWT_ISSUER=FirmezaAPI
JWT_AUDIENCE=FirmezaClients

# Application Configuration
RUN_SEEDERS=1

# SMTP Configuration (optional)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASS=your-app-password
SMTP_FROM=noreply@firmeza.com
```

> **Important:** Replace placeholder values with your actual configuration.

### Starting the Application

1. **Clone the repository** (if you haven't already):
   ```bash
   git clone <repository-url>
   cd Firmeza-ASP.-NET
   ```

2. **Build and start all services:**
   ```bash
   docker-compose up --build
   ```

   This command will:
   - Build Docker images for all services
   - Start PostgreSQL database
   - Run unit tests
   - Start the Web API
   - Start the Admin Dashboard
   - Start the Frontend

3. **Wait for all services to be ready.** You should see output indicating all services are running.

### Accessing the Application

Once all services are running, you can access:

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost:5173 | Customer shopping interface |
| **Admin Dashboard** | http://localhost:5001 | Administrative management panel |
| **Web API** | http://localhost:5000 | RESTful API |
| **Swagger UI** | http://localhost:5000/swagger | API documentation |
| **PostgreSQL** | localhost:5432 | Database (internal) |

### Default Admin Credentials

If seeders are enabled (`RUN_SEEDERS=1`), a default admin account is created:

- **Email:** admin@firmeza.com
- **Password:** Admin123!
- **Role:** Admin

### Stopping the Application

To stop all services:

```bash
docker-compose down
```

To stop and remove all data (including database):

```bash
docker-compose down -v
```

## Service Dependencies

The Docker Compose stack starts services in the following order:

```
postgres → tests → api → admin → frontend
```

- **postgres** - Database starts first
- **tests** - Unit tests run and must pass
- **api** - Web API starts after successful tests
- **admin** - Admin Dashboard starts after API
- **frontend** - Customer frontend starts last

## Running Individual Services Locally

Each service can also be run independently. See the individual README files for detailed instructions:

- [Web API README](./Firmeza.WebApi/README.md)
- [Admin Dashboard README](./AdminDashboard.Web/README.md)
- [Frontend README](./Front-end/README.md)

## Database Migrations

Migrations are automatically applied when services start via Docker Compose. For manual migration management:

```bash
# From Web API directory
cd Firmeza.WebApi
dotnet ef database update

# From Admin Dashboard directory
cd AdminDashboard.Web
dotnet ef database update
```

## Running Tests

Tests are automatically run as part of the Docker Compose startup. To run tests manually:

```bash
# Run all tests
dotnet test

# Run tests from specific project
cd Firmeza.Test
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Development Workflow

### Making Changes

1. **Modify code** in your preferred IDE
2. **Rebuild affected service:**
   ```bash
   docker-compose up --build <service-name>
   ```
   Replace `<service-name>` with: `api`, `admin`, or `front-end`

3. **View logs** for debugging:
   ```bash
   docker-compose logs -f <service-name>
   ```

### Development with Hot Reload

For active development with hot reload:

1. **Stop Docker Compose** (if running)
2. **Start database only:**
   ```bash
   docker-compose up postgres
   ```
3. **Run services locally** following individual README instructions

## Project Documentation

- **Web API Documentation:** See [Firmeza.WebApi/README.md](./Firmeza.WebApi/README.md)
- **Admin Dashboard Documentation:** See [AdminDashboard.Web/README.md](./AdminDashboard.Web/README.md)
- **Frontend Documentation:** See [Front-end/README.md](./Front-end/README.md)

## Features

### Customer Features (Frontend)
- Browse product catalog
- Search and filter products
- Shopping cart management
- User registration and authentication
- Order placement
- Order history

### Admin Features (Dashboard)
- Product management (CRUD)
- Category management
- Client management
- Sales tracking and reporting
- PDF receipt generation
- Excel import/export
- User role management

### API Features
- RESTful endpoints
- JWT authentication
- Role-based authorization
- Swagger documentation
- CORS support
- Automatic data validation

## Clean Architecture

The project follows Clean Architecture principles:

- **Domain Layer** - Business entities and core logic
- **Application Layer** - Use cases and business workflows
- **Infrastructure Layer** - Data access and external services
- **Identity Layer** - Authentication and authorization
- **Presentation Layer** - Web API and Web interfaces

## Security

- JWT token-based authentication
- SHA256 password hashing
- Role-based access control (Admin, User)
- CORS protection
- Anti-forgery tokens
- Security headers (X-Frame-Options, X-XSS-Protection, etc.)
- HTTPS redirection

## Troubleshooting

### Port Already in Use

If you get port conflict errors:

```bash
# Change ports in compose.yaml
# Or stop conflicting services
sudo lsof -i :5000  # Find process using port
sudo kill -9 <PID>  # Kill the process
```

### Database Connection Issues

```bash
# Check database is running
docker-compose ps

# View database logs
docker-compose logs postgres

# Reset database
docker-compose down -v
docker-compose up postgres
```

### Service Won't Start

```bash
# View service logs
docker-compose logs <service-name>

# Rebuild specific service
docker-compose up --build <service-name>

# Remove all containers and rebuild
docker-compose down
docker-compose up --build
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests
5. Submit a pull request

## License

This project is for educational/commercial purposes.

## Support

For issues and questions:
- Check individual service README files
- Review Docker Compose logs
- Check API documentation at `/swagger`
