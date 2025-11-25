# 🏗️ Firmeza - Construction Materials Management System

Welcome to **Firmeza**, a comprehensive Full-Stack solution designed for the management and sale of construction materials. This project implements a modern and scalable architecture, separating the administration logic (Backend MVC) from the customer shopping experience (Frontend React + Web API).

---

## 📐 System Architecture

The project follows **Clean Architecture** principles and **DDD** (Domain-Driven Design), ensuring separation of concerns, maintainability, and scalability.

### 🧩 Solution Structure

The solution is divided into multiple layers and projects:

#### 1. **Core**
- **`AdminDashboard.Domain`**: Contains Entities, Value Objects, and pure business logic. It has no external dependencies.
- **`AdminDashboard.Application`**: Defines Use Cases, DTOs (Data Transfer Objects), and Interfaces. It orchestrates application logic without knowing implementation details.

#### 2. **Infrastructure**
- **`AdminDashboard.Infrastructure`**: Persistence implementation using **Entity Framework Core**, repositories, and external services.
- **`AdminDashboard.Identity`**: Authentication and authorization management using **ASP.NET Core Identity**.
- **`SmtpSettings`**: Encapsulated service for email sending.
- **`SalePDF`**: Library dedicated to generating sales receipts in PDF format.
- **`ExcelImporter`**: Module for bulk data import from Excel.

#### 3. **Presentation (Backend & API)**
- **`AdminDashboard.Web` (ASP.NET Core MVC)**: 
  - Server-side rendered Administration Panel (Razor Pages).
  - Complete management of inventory, users, sales, and reports.
  - Cookie/Identity-based authentication.
- **`Firmeza.WebApi` (ASP.NET Core Web API)**:
  - RESTful API serving the client frontend.
  - Secure authentication via **JWT (JSON Web Tokens)**.
  - Endpoints for the catalog, shopping cart, and sales registration.

#### 4. **Frontend (Client)**
- **`Front-end` (React + Vite)**:
  - Modern and reactive Single Page Application (SPA).
  - Responsive design with custom CSS and Tailwind (optional).
  - Global state management with **Zustand**.
  - API communication via **Axios**.

---

## 🚀 Technologies Used

### Backend (.NET 8)
- **Framework**: ASP.NET Core 8
- **ORM**: Entity Framework Core (SQL Server)
- **Auth**: ASP.NET Core Identity + JWT Bearer
- **Documentation**: Swagger / OpenAPI
- **Utilities**: AutoMapper, iText7 (PDF), EPPlus (Excel)

### Frontend (React)
- **Build Tool**: Vite
- **Framework**: React 18
- **Routing**: React Router DOM
- **State**: Zustand
- **HTTP Client**: Axios
- **Styles**: CSS Modules / Custom CSS

---

## 💾 Seed Data

The system includes an **Automatic Seeding** mechanism that populates the database with initial data for testing and development. This data is loaded automatically when the application starts for the first time.

### 👥 Pre-loaded Users

| Role | Email | Password | Access |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@admindashboard.com` | `Admin123!` | Admin Panel (`:5001`) |
| **Client** | `client@admindashboard.com` | `Client123!` | Online Store (`:5173`) |

> **Note**: The system automatically redirects the user to their corresponding platform (Admin or Store) based on their role upon login.

### 📦 Product Catalog
The system starts with **10 real construction products** distributed across **4 categories**:
- **Cement & Mortars**: Portland Cement, Premixed Mortar.
- **Bricks & Blocks**: Solid Brick, Block #4, Block #5.
- **Sand & Gravel**: Washed Sand, Gravel #67.
- **Wood**: Plywood, Pine Wood, Rough Lumber.

---

## 🛠️ Setup & Installation

### Prerequisites
- .NET SDK 8.0
- Node.js (v18 or higher)
- SQL Server (LocalDB or full instance)

### Environment Variables
The project uses a `.env` file in the root for sensitive configuration (SMTP, JWT, Connections).
Ensure you configure:
- `JWT_KEY`: Secret key for token signing.
- `SMTP_*`: Credentials for email sending (Gmail/Outlook).

### ⚡ Quick Start

We have included an automated script to start all services simultaneously:

```bash
./start-all.sh
```

This script will launch:
1. **WebApi** at `http://localhost:5000`
2. **AdminDashboard** at `http://localhost:5001`
3. **Frontend React** at `http://localhost:5173`

### Manual Execution

If you prefer to run each service separately:

**1. Backend API:**
```bash
dotnet run --project Firmeza.WebApi/Firmeza.WebApi.csproj
```

**2. Admin Panel:**
```bash
dotnet run --project AdminDashboard.Web/AdminDashboard.Web.csproj
```

**3. Frontend React:**
```bash
cd Front-end
npm install
npm run dev
```

---

## ✨ Key Features

### 🏢 Admin Panel
- **Dashboard**: Real-time sales and stock metrics.
- **Product Management**: Full CRUD with image support.
- **Import/Export**: Bulk upload from Excel and PDF reports.
- **Users**: Role and permission management.

### 🛒 Client Store (React)
- **Interactive Catalog**: Product filtering and search.
- **Shopping Cart**: Persistent state management.
- **Secure Checkout**: Data validation and order generation.
- **Receipts**: Automatic PDF invoice sent via email and direct download.
- **Responsive**: Design adapted for mobile, tablets, and desktop.

---

© 2025 Firmeza S.A.S. - All rights reserved.
