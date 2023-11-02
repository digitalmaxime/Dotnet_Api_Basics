<h2>Run MySql Server with Docker</h2>

`docker run --name CrudDemo -e MYSQL_ROOT_PASSWORD=test123 --publish 1234:3306 -d mysql:latest`

<h2>Connect to MySql with Workbench client</h2>



<h2>EfCore Dependencies</h2>

``dotnet add package Microsoft.EntityFrameworkCore.Design``

``dotnet add package Microsoft.EntityFrameworkCore``

``dotnet add package  MySql.EntityFrameworkCore ``

<h2>Swagger Dependencies</h2>

`dotnet add  package Swashbuckle.AspNetCore -v 6.2.3`

<h2>Create 4 layers (folders to make it simple)</h2>

- Api
- Application
- Infrastructure
- Domain

<h2>Api Controller</h2>

- Controller class must implement ControllerBase
- in program.cs (or serviceCollection extensions) *service* should `.AddControllers();`
- and *app* should `.MapControllers();`
- Add Swagger in program.cs 
    - `builder.Services.AddEndpointsApiExplorer();` 
    - `builder.Services.AddSwaggerGen();`
    - `app.UseSwagger();`
    - `app.UseSwaggerUI();`
