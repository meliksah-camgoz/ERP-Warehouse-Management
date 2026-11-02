# ERP Warehouse Management System

A web-based ERP and warehouse management application designed to simplify inventory operations, product tracking, warehouse management, and asset management.

Built with **ASP.NET Core, C#, Angular, and Microsoft SQL Server**, this project aims to provide a centralized solution for managing warehouse and inventory processes.

## 🚀 Features

- **Product Management** – Create, list, update, and manage products.
- **Category Management** – Organize products into categories.
- **Warehouse Management** – Manage warehouses and their operational status.
- **Stock Management** – Track stock quantities and perform stock entry and exit operations.
- **Stock Movement History** – Maintain historical records of stock transactions.
- **Asset Management** – Register and manage company assets.
- **Excel Export** – Export asset information to Excel.
- **RESTful API** – Backend services built with ASP.NET Core.
- **Database Management** – Store and manage data using Entity Framework Core and SQL Server.

## 🛠️ Technologies

### Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Clean Architecture
- Swagger / OpenAPI
- ClosedXML

### Frontend
- Angular
- TypeScript
- HTML5
- CSS3

### Development Tools
- Visual Studio
- Visual Studio Code
- SQL Server LocalDB
- Git & GitHub

## 🏗️ Architecture

The backend follows a layered Clean Architecture approach to separate business logic, application services, infrastructure, and API responsibilities.

```text
ERP-Warehouse-Management/
│
├── src/
│   ├── MiniERP.Domain/
│   ├── MiniERP.Application/
│   ├── MiniERP.Infrastructure/
│   └── MiniERP.Api/
│
├── frontend/
│
├── .gitignore
└── README.md
```

## ⚙️ Getting Started

### Prerequisites

Make sure you have the following installed:

- .NET SDK 10.0
- Node.js and npm
- Angular CLI
- Microsoft SQL Server or SQL Server LocalDB

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/ERP-Warehouse-Management.git
cd ERP-Warehouse-Management
```

### 2. Configure the Database

Update the connection string in your backend configuration to match your SQL Server environment.

### 3. Apply Database Migrations

```bash
dotnet ef database update
```

### 4. Run the Backend

```bash
dotnet run --project src/MiniERP.Api --urls http://localhost:5252
```

API and Swagger documentation:

```text
http://localhost:5252/swagger
```

### 5. Run the Frontend

Open a new terminal:

```bash
cd frontend
npm install
ng serve
```

Open the application:

```text
http://localhost:4200
```

## 📌 Project Status

**Under Development**

The project is actively being developed. New modules, improvements, and additional features will be introduced over time.

## 🎯 Future Improvements

- User authentication and authorization
- Role-based access control
- Dashboard with inventory statistics
- Advanced reporting and analytics
- Excel import for inventory data
- Improved responsive UI
- Enhanced stock movement reports

## 👨‍💻 Developer

Melikşah Camgöz

https://meliksahdev.netlify.app/
---

⭐ If you find this project interesting, feel free to star the repository!
