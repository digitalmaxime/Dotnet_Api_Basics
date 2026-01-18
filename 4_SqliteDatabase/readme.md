# Database integration 


This demo features database integration using Sqlite and EF Core.

It introduces the following concepts:
- EF Core Object Relational Mapping
- EF Core's Migrations
- EF Core's Navigation properties
- Seeding data
- CRUD operations using DbContext

## Sqlite

_SQLite is a C-language library that implements a small, fast, self-contained SQL database engine. 
SQLite is the most used database engine in the world._

Sqlite provides a simple in memory database useful for testing and demo. 

The database is a single file (.db) that is easy to move and back up.

Simplicity: Requires zero configuration and no separate service installation.

## Tools and Packages

`Microsoft.EntityFrameworkCore.Sqlite`

`Microsoft.EntityFrameworkCore.Design` for migrations

`dotnet tool update --global dotnet-ef`

## Cookbook

#### Install EF Core tools and abovementioned packages

#### Define model classes

- Include Navigation properties

#### Define DbContext

- Include `OnConfiguring` and `OnModelCreating` methods
- Add `DbSet` properties

#### Add migrations
 
- `dotnet ef migrations add InitialCreate`
- `dotnet ef database update`

#### Define CRUD operations

- Write endpoints using DbContext methods

## Seeding data

### HasData()

_Model Managed Data_ defined in DbContext's `OnModelCreating` 

It is part of migrations: Generates SQL INSERTs in migration files.

Runs only when migrations are applied.

Requires hard-coded, manual primary keys.

e.g. `modelBuilder.Entity<Product>().HasData(new Product { Id = 1, Name = "Product 1" });`

Good for:
- Static metadata data
- Data that is essential to the system's structure and rarely changes.
- Versioned data changes 
- Small datasets

### UseSeeding()

Introduced in EF Core 9

_General purpose data seeding_ defined in DbContext's `OnConfiguring`

Runs on EnsureCreated(), Migrate(), and ef database update

Introduced to solve the "bloat" and "static logic" issues of `HasData`. It is the recommended approach for demo purposes. 

It allows for complex logic and automatic primary key generation.

Does not affect migration files or snapshots (doesn't clutter migrations)

Best Practice: Ensure your code is idempotent because this method runs every time the database is initialized or migrated

