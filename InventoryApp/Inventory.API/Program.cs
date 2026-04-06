using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Inventory.API.Middlewares;
using Inventory.Core.Config;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.DTOs.Validators;
using Inventory.Core.Interfaces;
using Inventory.Infra.Data;
using Inventory.Infra.Repositories;
using Inventory.Infra.Services;
using Inventory.Infra.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuring Serilog
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InventoryDbContext>(options=> options.UseSqlite(connectionString));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
        .SelectMany(v=> v.Errors)
        .Select(e=> e.ErrorMessage)
        .ToList();

        var response = new ApiResponse<object?>(false,"validationErrors", null, errors);
        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddValidatorsFromAssemblyContaining<ProductrequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<AdminSettings>(
    builder.Configuration.GetSection(AdminSettings.SectionName));


builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options=>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    }
)
.AddEntityFrameworkStores<InventoryDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName)
.Get<JwtSettings>();


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
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudiences = [jwtSettings.Audience],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// Adding Rate Limiting
builder.Services.AddRateLimiter(options=>
{
    options.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Send Standard Response on Rejection
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync( new ApiResponse<object>
        {
            Success = false,
            Message = "Too many requests. Please try again."
        }, token);
    };
});

var app = builder.Build();
// Seeding Admin
using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await ContextSeed.SeedRoleAndAdminAsync(services);
    }catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex,"An error occured while seeding database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // defaults to /openapi/v1.json
    app.MapOpenApi();
    // defaults to /scalar/v1
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();