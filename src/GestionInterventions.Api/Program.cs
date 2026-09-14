using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Clients.Commands.RegisterClient;
using GestionInterventions.Infrastructure.Persistence;
using GestionInterventions.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using GestionInterventions.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using GestionInterventions.Api.Services;

var builder = WebApplication.CreateBuilder(args);


// =========================
// Configuration JWT
// =========================
var jwtSettings = builder.Configuration.GetSection("Jwt");

var key = jwtSettings["Key"]
    ?? throw new InvalidOperationException("La clé JWT est introuvable.");


// =========================
// Controllers
// =========================
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrez votre token JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



// =========================
// EF Core + SQLite
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



// =========================
// MediatR
// =========================
// MediatR — scanne l'assembly Application pour trouver tous les Handlers automatiquement
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterClientCommand).Assembly));



// =========================
// ASP.NET Identity
// =========================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                ),

                ClockSkew = TimeSpan.Zero
            };
    });



// =========================
// Authorization
// =========================
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();



// =========================
// Repositories + Services
// =========================
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEquipementRepository, EquipementRepository>();
builder.Services.AddScoped<IDemandeInterventionRepository, DemandeInterventionRepository>();
builder.Services.AddScoped<ITechnicienRepository, TechnicienRepository>();
builder.Services.AddScoped<IInterventionRepository, InterventionRepository>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

// =========================
// Seed Identity
// =========================
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
}


// =========================
// Middlewares
// =========================
app.UseMiddleware<GestionInterventions.Api.Middlewares.ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();