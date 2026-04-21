using RetailCore.Repositories.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Database
var dbOptions = builder.Configuration
    .GetSection("Database")
    .Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database config missing");

builder.Services.AddRepositories(dbOptions);
builder.Services.AddServices();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
