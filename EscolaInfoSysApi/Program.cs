using System.Text;
using System.Text.Json.Serialization;
using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // >>> ADICIONE ISTO AQUI: configura JWT sem appsettings.json
        builder.Configuration["Jwt:Issuer"] = "EscolaInfoSys";
        builder.Configuration["Jwt:Audience"] = "EscolaInfoSys.Mobile";
        builder.Configuration["Jwt:Key"] = "GcHiMA0R1IeeIeVFkNWXTdg2lK27BfrXJcbBo9HRnElSwVUcQJyydgS5U1UQcRb5";

        // DbContext (mesmo do Web)
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Identity (mesmo ApplicationUser do Web)
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT
        var jwt = builder.Configuration.GetSection("Jwt");
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
                };
            });

        builder.Services.AddAuthorization();

        // CORS p/ .NET MAUI (abre no início; depois afina domínio)
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("maui", p => p
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });

        // Repositórios + serviços do Web
        builder.Services.RegisterRepositories(); // EscolaInfoSys.Data.Repositories.RepositoryConfig

        // Se ainda não estiver registrado por RegisterRepositories(), registra manualmente:
        builder.Services.AddScoped<AbsenceStatsService>();
        builder.Services.AddScoped<IAccountService, AccountService>();

        // Controllers + JSON (evitar ciclos; ignorar nulls)
        builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.AddEndpointsApiExplorer();

        // Swagger com Bearer
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EscolaInfoSysApi", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT como: Bearer {seu_token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseCors("maui");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
