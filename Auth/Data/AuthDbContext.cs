using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;

namespace Auth.Data
{
    public class AuthDbContext:IdentityDbContext
    {
        public AuthDbContext(DbContextOptions options):base(options)
        {
        }
    }
}
