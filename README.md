# ShopApp — 3-Tier Architecture

A full e-commerce application built with **ASP.NET Core 8**, separated into three independent class libraries following clean architectural principles.

---

## 🏗 Solution Structure

```
ShopApp.sln
│
├── ShopApp.DAL/                 ← Data Access Layer (Class Library)
│   ├── Models/                  │  AppUser, Category, Product, Address, Order, OrderItem
│   ├── Data/                    │  ApplicationDbContext, DbSeeder
│   ├── Repositories/
│   │   ├── Interfaces/          │  IGenericRepository<T>, ICategoryRepository,
│   │   │                        │  IProductRepository, IAddressRepository, IOrderRepository
│   │   ├── GenericRepository.cs │  Base CRUD implementation
│   │   ├── CategoryRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── AddressRepository.cs
│   │   └── OrderRepository.cs
│   └── Extensions/
│       └── DependencyInjection.cs  ← AddDAL(configuration)
│
├── ShopApp.BLL/                 ← Business Logic Layer (Class Library)
│   ├── DTOs/                    │  All Data Transfer Objects (no EF entities exposed to PL)
│   ├── Mappers/                 │  Entity → DTO mapping (manual, zero dependencies)
│   ├── Services/
│   │   ├── Interfaces/          │  ICategoryService, IProductService, IAddressService,
│   │   │                        │  ICartService, IOrderService
│   │   ├── CategoryService.cs
│   │   ├── ProductService.cs
│   │   ├── AddressService.cs
│   │   ├── CartService.cs       ← Session-based cart
│   │   └── OrderService.cs      ← Atomic checkout transaction
│   └── Extensions/
│       └── DependencyInjection.cs  ← AddBLL()
│
└── ShopApp.PL/                  ← Presentation Layer (ASP.NET Core MVC)
    ├── Controllers/             │  CatalogController, CartController, OrdersController
    ├── Areas/Admin/Controllers/ │  CategoriesController, ProductsController, OrdersController
    ├── ViewModels/              │  All ViewModels (built from DTOs, not entities)
    ├── Views/                   │  Razor views (Bootstrap 5)
    └── Program.cs               ← DI wiring: AddDAL() + AddBLL() + Identity + Session
```

---

## 🔗 Dependency Flow

```
PL  →  BLL  →  DAL  →  Database
```

- **PL** knows only BLL interfaces — never touches EF entities directly
- **BLL** orchestrates business logic, maps entities to DTOs
- **DAL** owns all database access and EF Core

---

## ⚙️ Setup & Run

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### 1. EF Core Migrations (run from solution root)
```bash
dotnet ef migrations add InitialCreate --project ShopApp.DAL --startup-project ShopApp.PL
dotnet ef database update             --project ShopApp.DAL --startup-project ShopApp.PL
```

### 2. Run
```bash
cd ShopApp.PL
dotnet run
```

### 3. Default Admin Credentials
| Field    | Value            |
|----------|------------------|
| Email    | admin@shop.com   |
| Password | Admin@123        |

---

## 💉 Dependency Injection Architecture

All DI registrations are handled via **extension methods** in each layer:

```csharp
// Program.cs — clean single-responsibility registration
builder.Services.AddDAL(builder.Configuration);  // registers DbContext + Repositories
builder.Services.AddBLL();                        // registers all Services
```

### DAL Registrations (`AddDAL`)
| Interface               | Implementation        | Lifetime |
|-------------------------|-----------------------|----------|
| `ICategoryRepository`   | `CategoryRepository`  | Scoped   |
| `IProductRepository`    | `ProductRepository`   | Scoped   |
| `IAddressRepository`    | `AddressRepository`   | Scoped   |
| `IOrderRepository`      | `OrderRepository`     | Scoped   |

### BLL Registrations (`AddBLL`)
| Interface          | Implementation     | Lifetime |
|--------------------|--------------------|----------|
| `ICategoryService` | `CategoryService`  | Scoped   |
| `IProductService`  | `ProductService`   | Scoped   |
| `IAddressService`  | `AddressService`   | Scoped   |
| `ICartService`     | `CartService`      | Scoped   |
| `IOrderService`    | `OrderService`     | Scoped   |

---

## 🛒 Checkout — Atomic Transaction

The `OrderService.CheckoutAsync` method performs all 5 steps inside a single DB transaction:

```
1. Resolve / create shipping address
2. Validate stock for every cart item
3. Create Order record
4. Create OrderItem records
5. Decrease product StockQuantity
→ COMMIT (or ROLLBACK on any failure)
```

---

## 📦 Tech Stack
| Layer | Technology |
|-------|-----------|
| ORM   | Entity Framework Core 8 (Code First, SQL Server) |
| Auth  | ASP.NET Core Identity |
| UI    | Bootstrap 5 + Bootstrap Icons |
| Cart  | ASP.NET Core Session |
| DI    | Microsoft.Extensions.DependencyInjection (built-in) |
