using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EscolaInfoSys.Api.Services;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Api.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// --- Serviços ---
// Controllers
builder.Services.AddControllers();

// Banco de Dados compartilhado (Entity Framework Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (login com mesmas credenciais da Web)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = new JwtSettings
        {
            Key = "uU92GHfaADjsklQWpeRtuNvcxwPoiuyT",         // ?? Chave segura (não pública)
            Issuer = "EscolaInfoSys.Api",                    // ?? Emissor esperado
            Audience = "EscolaInfoSys"                // ?? Público esperado
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero, // ? sem tolerância de horário


             RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });


// CORS (para permitir acesso do app móvel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Repositórios e serviços
builder.Services.RegisterRepositories();
builder.Services.AddScoped<IStudentExclusionStatusService, StudentExclusionStatusService>();
builder.Services.AddScoped<IApiAccountService, ApiAccountService>();
builder.Services.AddScoped<IEmailSender, ApiEmailSender>();


builder.Configuration["Mail:From"] = "noreply@EscolaInfoSysApi.somee.com";
builder.Configuration["Mail:NameFrom"] = "Escola InfoSys";
builder.Configuration["Mail:Smtp"] = "smtp.EscolaInfoSysApi.somee.com";
builder.Configuration["Mail:Port"] = "25"; 
builder.Configuration["Mail:Password"] = "Student123!";



var app = builder.Build();

// --- Pipeline de middlewares ---
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

