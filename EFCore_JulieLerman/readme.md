# Entity Framework Core 8

Based on Julia Lerman's course on Pluralsight, this simple project implements a Book library with Authors. 

This project uses Aspnet Core's Minimal API, Entity Framework Core 8 and MSSQL Server as the database (Microsoft SQL Server Express)

## Table of contents
1) [Packages](#packages)
2) [Context](#context)
3) [Model](#model)
4) [Migrations](#migrations)
5) [Reference](#reference)


## Packages
API Layer
- `Microsoft.EntityFrameworkCore.Design` for Ef migrations

Persistence Layer
- `Microsoft.EntityFrameworkCore.SqlServer` includes the `Microsoft.EntityFrameworkCore` package itself
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` contains types that will be useful to capture and report diagnostics from EF Core

Dotnet CLI `dotnet add package <packageName>`

## Configuration Considerations

Beware of the cyclical nature of the relationship between the entities.
  - E.g. Book has an Author, and Author has a collection of Books.
    - configure json options to ignore the loop e.g.
      ```
      builder.Services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
      ```
    - Use modelbuilder to configure the relationship between the entities
      ```
      modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);
      ```

DbContext configurations
- `options.EnableSensitiveDataLogging()` to log sensitive data
- `options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);` to disable tracking of entities by default

## Model


## Migrations

CLI `dotnet ef migrations add` and  `dotnet ef database update`

## Reference

reference : 
- [Pluralsight EF Core 8 Fundamentals](https://app.pluralsight.com/library/courses/ef-core-8-fundamentals/table-of-contents)
- [timdeschryver.dev/blog/how-to-test-your-api](https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#using-the-apiwebapplicationfactory-in-tests)