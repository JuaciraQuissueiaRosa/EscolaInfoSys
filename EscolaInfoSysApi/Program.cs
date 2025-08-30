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
using EscolaInfoSysApi.Hubs;



var builder = WebApplication.CreateBuilder(args);

// (A) Reaproveita appsettings do MVC (Mail etc.)
var mvcAppSettings = Path.Combine(builder.Environment.ContentRootPath, "..", "EscolaInfoSys", "appsettings.json");
if (File.Exists(mvcAppSettings))
    builder.Configuration.AddJsonFile(mvcAppSettings, optional: false, reloadOnChange: true);

// (B) E-mail: usa o SmtpEmailSender do MVC
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// (C) Base do site MVC (links de reset)
if (string.IsNullOrWhiteSpace(builder.Configuration["Web:BaseUrl"]))
    builder.Configuration["Web:BaseUrl"] = "https://escolainfosys.somee.com";

// ---- JWT / Identity / DB ----
builder.Configuration["Jwt:Issuer"] = "EscolaInfoSys";
builder.Configuration["Jwt:Audience"] = "EscolaInfoSysApi";
builder.Configuration["Jwt:Key"] = "GcHiMA0R1IeeIeVFkNWXTdg2lK27BfrXJcbBo9HRnElSwVUcQJyydgS5U1UQcRb5";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "workstation id=ManagementSchoolDB.mssql.somee.com;packet size=4096;user id=JuRosa_SQLLogin_5;pwd=tzgogm1hw3;data source=ManagementSchoolDB.mssql.somee.com;persist security info=False;initial catalog=ManagementSchoolDB;TrustServerCertificate=True"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 🔐 UMA configuração de Auth + JWT (apenas esta!)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
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

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
    o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
});

builder.Services.AddAuthorization();

// CORS p/ MAUI
builder.Services.AddCors(p => p.AddPolicy("maui", c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Repositórios/serviços
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IMarkRepository, MarkRepository>();
builder.Services.AddScoped<IAbsenceRepository, AbsenceRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
builder.Services.AddScoped<IStudentExclusionRepository, StudentExclusionRepository>();
builder.Services.AddScoped<AbsenceStatsService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<INotifier, SignalRNotifier>();

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EscolaInfoSysApi", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName!.Replace('+', '.'));
    c.DocInclusionPredicate((_, api) => api.HttpMethod != null);
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
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }}, Array.Empty<string>() }
    });
});

var app = builder.Build();

// (opcional) dev
// if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

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
    c.SwaggerEndpoint("v1/swagger.json", "EscolaInfoSysApi v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("maui");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notify");

// rota raiz útil para sanity check
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
