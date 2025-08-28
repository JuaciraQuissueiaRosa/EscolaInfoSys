using System.Text;
using System.Text.Json.Serialization;
using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Services;
using EscolaInfoSysApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;



var builder = WebApplication.CreateBuilder(args);
// (A) Carrega o appsettings.json do MVC (mesmo Mail) via caminho relativo
var mvcAppSettings = Path.Combine(builder.Environment.ContentRootPath, "..", "EscolaInfoSys", "appsettings.json");
// ajuste "EscolaInfoSys" se a pasta/projeto tiver outro nome
if (File.Exists(mvcAppSettings))
{
    builder.Configuration.AddJsonFile(mvcAppSettings, optional: false, reloadOnChange: true);
}

// (B) DI do mesmo e-mail sender do MVC
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// (C) Base do site MVC p/ links de reset
if (string.IsNullOrWhiteSpace(builder.Configuration["Web:BaseUrl"]))
    builder.Configuration["Web:BaseUrl"] = "https://escolainfosys.somee.com";



builder.Configuration["Jwt:Issuer"] = "EscolaInfoSys";
        builder.Configuration["Jwt:Audience"] = "EscolaInfoSysApi";
        builder.Configuration["Jwt:Key"] = "GcHiMA0R1IeeIeVFkNWXTdg2lK27BfrXJcbBo9HRnElSwVUcQJyydgS5U1UQcRb5";



builder.Configuration["ConnectionStrings:DefaultConnection"] =
    "workstation id=ManagementSchoolDB.mssql.somee.com;packet size=4096;user id=JuRosa_SQLLogin_5;pwd=tzgogm1hw3;data source=ManagementSchoolDB.mssql.somee.com;persist security info=False;initial catalog=ManagementSchoolDB;TrustServerCertificate=True";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Identity (mesmo ApplicationUser do Web)
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


// ⚠️ Aqui: JWT como esquema padrão (autenticar e desafiar)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

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

//  isto evita redirects HTML quando falta auth em endpoints API
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
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

// builder.Services.RegisterRepositories();  // deixe comentado

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IMarkRepository, MarkRepository>();
builder.Services.AddScoped<IAbsenceRepository, AbsenceRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
builder.Services.AddScoped<IStudentExclusionRepository, StudentExclusionRepository>();

// Serviços usados
builder.Services.AddScoped<AbsenceStatsService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IEmailSender, NoOpEmailSender>();


builder.Services.AddScoped<AbsenceStatsService>();
        builder.Services.AddScoped<IAccountService, AccountService>();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger (blindado)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EscolaInfoSysApi", Version = "v1" });

    // evita conflito de tipos com mesmo nome (record/anonymous/inner classes)
    c.CustomSchemaIds(t => t.FullName!.Replace('+', '.'));

    // inclui SOMENTE actions que têm HttpMethod (evita controllers MVC sem [HttpGet]/[HttpPost])
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.HttpMethod == null) return false;

        // (opcional) Se quiser garantir que só entram controllers desta API:
        // var cad = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        // if (cad is null) return false;
        // return cad.ControllerTypeInfo.Namespace?.StartsWith("EscolaInfoSys.Api") == true;

        return true;
    });

    // Bearer no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ex.: Bearer eyJhbGciOi..."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference =
            new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
          Array.Empty<string>() }
    });
});

var app = builder.Build();

// página de erro detalhada enquanto depura
app.UseDeveloperExceptionPage();

// Swagger + correção de base URL (evita HTML/redirect em hosts/proxy)
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((doc, req) =>
    {
        doc.Servers = new List<OpenApiServer> {
            new OpenApiServer { Url = $"{req.Scheme}://{req.Host.Value}{req.PathBase}" }
        };
    });
});
app.UseSwaggerUI(c =>
{
    // caminho relativo funciona em http/https e subpaths
    c.SwaggerEndpoint("v1/swagger.json", "EscolaInfoSysApi v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();   
app.UseCors("maui");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();