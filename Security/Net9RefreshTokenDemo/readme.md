# Subject

Securing The .NET 9 App: Signup, Login, JWT, Refresh Tokens, and Role Based Access with PostgreSQL

JSON Web Tokens are an open, industry standard RFC 7519 method for representing claims securely between two parties.

## table of content
1) [Add required packages](#packages)
2) [Scaffolding the project](#scaffolding-the-project)
3) [Database](#database)
3) [Models](#models)
3) [JWT Settings](#jwt-settings)
3) [JWT Secret](#jwt-secret)
3) [Authentication Configurations](#authentication-configurations)
7) [Migrations](#migrations)
9) [Reference](#reference)

## Packages
VS code (with c# dev kit extension),
PostgreSql running in docker,
.NET 9,
Entity Framework Core,
Identity

- `Npgsql.EntityFrameworkCore.PostgreSQL` postgresql
- `Microsoft.EntityFrameworkCore.Design` to work with the ef migrations
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` for the identity
- `Microsoft.AspNetCore.Authentication.JwtBearer` to generate the jwt tokens

Dotnet CLI `dotnet add package <packageName>`

## Scaffolding the project

```
# create new solution file
dotnet new sln -o NetRefreshTokenDemo

cd NetRefreshTokenDemo

# create new controllers api project
dotnet new webapi --use-controllers -o NetRefreshTokenDemo.Api

# registering NetRefreshTokenDemo.Api project to the solution
dotnet sln add .\NetRefreshTokenDemo.Api\

# open the current folder in the vs code
code .
```

## Database

Add the connection string in the appsettings.json .
```
"ConnectionStrings": {
    "default": "Host=localhost; Port=5432; Database=NetRefreshTokenDemo;
                Username=postgres;Password=your_strong_password"
  }
```

create a dbcontext that derives from `IdentityDbContext<ApplicationUser>`

using `Microsoft.AspNetCore.Identity.EntityFrameworkCore;`

create a `DbSet<TokenInfo> TokenInfos` for refresh token

## Models

When we use the aspnetcore identity, lots of classes related to identity gets generated during the migration. __AspNetUsers__ is One of them, which have lots of default columns. If you want to store additional information about the user, you need to create a class named ApplicationUser and inherit the class named __IdentityUser__.

```
// Models/ApplicationUser.cs

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
```

## JWT Settings

ValidIssuer - 
Defines the issuer of the token. This is usually the URL or identifier of the authentication server (e.g., an identity provider like Azure AD, IdentityServer, or your custom auth service). It adds the iss claims in the token payload.

## JWT Secret

We also need to store __secret-key__. We are not going to store it in the appsettings. We will store it in the secret manager for local development. But for the production you need to use something like azure key vault or its other cloud counter parts.

- First of all we need to initialize the user-secrets in our project. Make sure you are in the projects directory.
`cd Net9RefreshTokenDemo.Api`
- Now initialize the user- secrets.
`dotnet user-secrets init`
- Store the jwt-secret in user-secrets.
`dotnet user-secrets set "JWT:secret" "your-32-characters-long-super-strong-jwt-secret-key"`
- You can find the secrets in following location.
    - in windows
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
    - in mac/linux
~/.microsoft/usersecrets/<user_secrets_id>/secrets.json

## Authentication Configurations
```
// Authentication
builder.Services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
 )
   .AddJwtBearer(options =>
     {
         options.SaveToken = true;
         options.RequireHttpsMetadata = false;
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidAudience = builder.Configuration["JWT:ValidAudience"],
             ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
             ClockSkew = TimeSpan.Zero,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"]))
         };
     }
    );
```

## Migrations

CLI `dotnet ef migrations add` and  `dotnet ef database update`

## Reference

reference : 
- [medium article](https://medium.com/codex/securing-the-net-9-app-signup-login-jwt-refresh-tokens-and-role-based-access-with-postgresql-43df24fd0ba2)
- [author's github](https://github.com/rd003/NetRefreshTokenDemo)
