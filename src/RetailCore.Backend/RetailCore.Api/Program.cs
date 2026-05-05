using RetailCore.Api.Middleware;
using RetailCore.Repositories.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var dbOptions = builder.Configuration
    .GetSection("Database")
    .Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database config missing");

builder.Services.AddRepositories(dbOptions);
builder.Services.AddServices();
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
