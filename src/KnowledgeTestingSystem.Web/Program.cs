using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using KnowledgeTestingSystem.Web.Auth;
using KnowledgeTestingSystem.Web.Components;
using KnowledgeTestingSystem.Web.Data;
using KnowledgeTestingSystem.Web.Services;
using KnowledgeTestingSystem.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using InterpretatorService.Services;
using InterpretatorService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// === Razor Components + Blazor Server ===
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// === EF Core — единый DbContext ===
builder.Services.AddDbContext<KnowledgeTestingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KnowledgeTestingDb"),
                        sql => sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        null))
);

// === Authentication ===
builder.Services.AddAuthenticationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthService>();

// === Services ===
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<EvaluationApiService>();
builder.Services.AddScoped<IEvaluationEngine, EvaluationEngine>();
builder.Services.AddScoped<IInterpreterService, InterpreterService>();
builder.Services.AddScoped<AlgorithmApiService>();
builder.Services.AddScoped<IValueSubstitutionService, ValueSubstitutionService>();

// === JSON Serialization (NewtonsoftJson for MarkSubsystem) ===
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

builder.Services.AddRazorPages();

// === Authorization ===
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("ТребуетсяПреподаватель", policy => policy.RequireRole("Преподаватель"));
    options.AddPolicy("ТребуетсяОбучающийся", policy => policy.RequireRole("Обучающийся"));

    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// === Swagger / API Documentation ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "KnowledgeTestingSystem API",
        Version = "v1",
        Description = "API for Knowledge Testing System (merged from InterpretatorService, MarkSubsystem, SKAT)"
    });
});

var app = builder.Build();

// Создаем директорию для файлов кода при запуске
var codeFilesDir = Path.Combine(Directory.GetCurrentDirectory(), "code_files");
if (!System.IO.Directory.Exists(codeFilesDir))
    System.IO.Directory.CreateDirectory(codeFilesDir);

// Создаем директорию для логов при запуске
var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!System.IO.Directory.Exists(logsDir))
    System.IO.Directory.CreateDirectory(logsDir);

// === Swagger ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/login", permanent: false);
        return;
    }
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map controllers (API endpoints)
app.MapControllers();

app.Run();
