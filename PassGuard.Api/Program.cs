using Microsoft.EntityFrameworkCore;
using PassGuard.Api.Database;
using PassGuard.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Ajout des Controllers
builder.Services.AddControllers();

// Ajouts des Repositories
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<DataRepository>();

// Service de la base de données
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDBContext")));

// Ajout des Cors Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Mappage des controllers
app.MapControllers();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.Run();