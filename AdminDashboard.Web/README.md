# Firmeza Admin Dashboard

ASP.NET Core Razor Pages application for administrative management of the Firmeza e-commerce platform. This web application provides a comprehensive interface for managing products, clients, categories, and sales.

## Technologies Used

- **ASP.NET Core 8.0** - Web framework
- **Razor Pages** - Server-side rendering
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Database
- **ASP.NET Core Identity** - Authentication and authorization
- **Bootstrap 5** - Frontend framework
- **AutoMapper** - Object-to-object mapping
- **EPPlus** - Excel import/export functionality
- **PDF Generation** - Sales receipt generation
- **DotNetEnv** - Environment variable management

## Project Architecture

```
AdminDashboard.Web/
├── Pages/                    # Razor Pages
│   ├── Account/              # Authentication pages
│   │   ├── Login.cshtml      # User login
│   │   ├── Register.cshtml   # User registration
│   │   └── Logout.cshtml     # User logout
│   ├── Admin/                # Admin area (requires authentication)
│   │   ├── Index.cshtml      # Admin dashboard home
│   │   ├── _AdminLayout.cshtml  # Admin sidebar layout
│   │   ├── Products/         # Product management
│   │   │   ├── Index.cshtml       # Product list
│   │   │   ├── Create.cshtml      # Create product
│   │   │   ├── Edit.cshtml        # Edit product
│   │   │   ├── Details.cshtml     # Product details
│   │   │   └── Delete.cshtml      # Delete confirmation
│   │   ├── Categories/       # Category management
│   │   │   ├── Index.cshtml       # Category list
│   │   │   ├── Create.cshtml      # Create category
│   │   │   ├── Edit.cshtml        # Edit category
│   │   │   ├── Details.cshtml     # Category details
│   │   │   └── Delete.cshtml      # Delete confirmation
│   │   ├── Users/            # Client management
│   │   │   ├── Index.cshtml       # Client list
│   │   │   ├── Create.cshtml      # Create client
│   │   │   ├── Edit.cshtml        # Edit client
│   │   │   ├── Details.cshtml     # Client details
│   │   │   └── Delete.cshtml      # Delete confirmation
│   │   ├── Sales/            # Sales management
│   │   │   ├── Index.cshtml       # Sales list
│   │   │   ├── Create.cshtml      # Create sale
│   │   │   ├── Edit.cshtml        # Edit sale
│   │   │   └── Details.cshtml     # Sale details with items
│   │   ├── Reports/          # Reporting section
│   │   │   └── Index.cshtml       # Generate reports
│   │   └── ExcelImporter/    # Excel data import
│   │       └── Index.cshtml       # Import interface
│   ├── Shared/               # Shared layouts and partials
│   │   ├── _Layout.cshtml    # Main layout
│   │   └── _ValidationScriptsPartial.cshtml
│   └── Index.cshtml          # Public home page
├── Program.cs                # Application entry point
├── Routing/                  # Custom routing configuration
└── wwwroot/                  # Static files
    ├── css/                  # Stylesheets
    ├── js/                   # JavaScript files
    ├── lib/                  # Third-party libraries
    └── recibos/              # Generated PDF receipts
```

## Features and Views

### Public Pages

- **Home (`/`)** - Landing page with application overview

### Authentication Pages (`/Account`)

- **Login** - User authentication with email and password
- **Register** - New user registration
- **Logout** - Session termination

### Admin Dashboard (`/Admin`)

All admin pages require authentication and appropriate roles.

#### Dashboard Home (`/Admin`)
- Overview of system statistics
- Quick access to all management sections
- Recent activity summary

#### Product Management (`/Admin/Products`)
- **Index** - Paginated product list with search and filter
- **Create** - Add new products with images, pricing, and inventory
- **Edit** - Update product information and stock levels
- **Details** - View complete product information
- **Delete** - Remove products from catalog
- **Export** - Export products to Excel format

#### Category Management (`/Admin/Categories`)
- **Index** - List all product categories
- **Create** - Add new categories
- **Edit** - Modify category details
- **Details** - View category information
- **Delete** - Remove categories

#### Client Management (`/Admin/Users`)
- **Index** - Client directory with search functionality
- **Create** - Register new clients
- **Edit** - Update client profiles and credentials
- **Details** - View complete client information
- **Delete** - Remove client accounts
- **Export** - Export clients to Excel format

#### Sales Management (`/Admin/Sales`)
- **Index** - Sales history with filtering by date, client, and status
- **Create** - Create new sales transactions
- **Edit** - Modify pending sales
- **Details** - View sale details including items, totals, and VAT
- **PDF Receipt** - Generate and download PDF receipts
- Sales automatically calculate totals, VAT, and generate receipts

#### Reports (`/Admin/Reports`)
- Sales reports by date range
- Product performance analytics
- Client purchase history

#### Excel Importer (`/Admin/ExcelImporter`)
- Bulk import products from Excel files
- Data validation and error reporting
- Template download for correct format

## How to Run

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL database
- Web API running (dependency)
- Environment variables configured

### Running Locally

1. **Navigate to the project directory:**
   ```bash
   cd AdminDashboard.Web
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

5. **Access the application:**
   - Web Dashboard: `http://localhost:5001`
   - Login with admin credentials or register a new account

### Running with Docker

The Admin Dashboard is configured to run as part of the Docker Compose stack. See the main README for instructions.

### Default Admin Account

If seeders are enabled (`RUN_SEEDERS=1`), a default admin account is created:
- **Email:** admin@firmeza.com
- **Password:** Admin123!
- **Role:** Admin

## Security Features

- **ASP.NET Core Identity** - Complete user management system
- **Cookie-based Authentication** - Secure session management
- **Role-based Authorization** - Admin and User roles
- **Password Hashing** - SHA256 encrypted password storage
- **CSRF Protection** - Built-in anti-forgery tokens
- **Security Headers** - X-Content-Type-Options, X-Frame-Options, X-XSS-Protection

## Integration with Web API

The Admin Dashboard communicates with the Firmeza Web API for:
- User authentication and role management
- Data persistence and retrieval
- Business logic execution
- PDF receipt generation

## Clean Architecture

This application follows Clean Architecture with:

- **Domain Layer** - Business entities and rules
- **Application Layer** - Use cases, DTOs, and service interfaces
- **Infrastructure Layer** - Data access and external services
- **Identity Layer** - Authentication and user management
- **Web Layer** - Razor Pages and UI logic

## Development Notes

- All admin pages use the `_AdminLayout.cshtml` for consistent navigation
- Custom routing is configured in `Routing/AppRoutingExtensions.cs`
- Static files and receipts are stored in `wwwroot/`
- Excel export uses EPPlus library (NonCommercial license)
