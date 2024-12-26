using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Net9RefreshTokenDemo.Api.Models;

public class AppDbContext: IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions options): base(options)
    {
        
    }

    public DbSet<TokenInfo> TokenInfos { get; set; }
    
}