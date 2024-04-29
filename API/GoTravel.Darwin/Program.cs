using System.Reflection;
using GoTravel.Darwin.Domain.Data;
using GoTravel.Darwin.Services.ServiceCollections;

var builder = WebApplication.CreateBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environmentName}.json", false)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Services
    .AddLogCollection()
    .AddEFCore<GoTravelDarwinContext>(builder.Configuration.GetSection("Database"))
    .AddHealthChecksCollection()
    .AddDarwinServices()
    .AddHangfireCollection(builder.Configuration.GetSection("Hangfire"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHealthChecksCollection();
app.UseHangfire(builder.Configuration.GetSection("Hangfire"));

app.Run();