using Auth.Configurations;
using Auth.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<AuthDbContext>();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true, // Set to true to enforce expiration time validation
                    ValidateLifetime = true,      // Set to true to validate token lifetime
                    ClockSkew = TimeSpan.FromMinutes(15),
                    ValidIssuers = new List<string>
                    {
                        "https://amcartstorageaccount.z10.web.core.windows.net",
                        "http://localhost:4200"
                    }

                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                               //.AllowCredentials()
                               //.WithExposedHeaders("Access-Control-Allow-Origin");
                    });
            });

            var app = builder.Build();

            //Ensure the database is created
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseAuthentication();

            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}