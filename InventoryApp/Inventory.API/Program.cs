using FluentValidation;
using FluentValidation.AspNetCore;
using Inventory.API.Middlewares;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.DTOs.Validators;
using Inventory.Core.Interfaces;
using Inventory.Infra.Data;
using Inventory.Infra.Repositories;
using Inventory.Infra.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

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
app.Run();