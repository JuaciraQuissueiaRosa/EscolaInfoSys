using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using EscolaInfoSys.Api.Services;
using EscolaInfoSys.Data.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Servi�os ---
// Controllers
builder.Services.AddControllers();

// Banco de Dados compartilhado (Entity Framework Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (login com mesmas credenciais da Web)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Servi�os customizados


builder.Services.AddScoped<IStudentExclusionStatusService, StudentExclusionStatusService>();
builder.Services.RegisterRepositories();





// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

// CORS (para app m�vel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




// Swagger com suporte a JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EscolaInfoSys API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Put the JWT Token on format: Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<EscolaInfoSys.Api.Services.IAccountService, EscolaInfoSys.Api.Services.AccountService>();


var app = builder.Build();

// --- Pipeline de middlewares ---

// Redireciona para HTTPS
app.UseHttpsRedirection();

// CORS deve vir antes da autentica��o e autoriza��o
app.UseCors("AllowAll");

// Autentica��o
app.UseAuthentication();

// Autoriza��o
app.UseAuthorization();

// Swagger (apenas em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear controllers (endpoints)
app.MapControllers();

app.Run();
