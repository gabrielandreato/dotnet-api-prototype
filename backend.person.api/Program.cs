using System.Reflection;
using backend.person.api.Services;
using backend.person.api.Services.Interfaces;
using backend.person.datalibrary.DataContext;
using backend.person.datalibrary.Repository;
using backend.person.datalibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

try
{
    Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

var connectionString = builder.Configuration.GetConnectionString("PersonDataStringConnection");

// Add services to the container.
builder.Services.AddDbContext<IPersonDataContext, PersonDataContext>(opt =>
    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


#region Repository

builder.Services.AddTransient<IPersonRepository, PersonRepository>();

#endregion

#region Services

builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<ICostumerService, CostumerService>();

#endregion

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Person API",
        Description = "An ASP.NET Core Web API for managing Registration of People",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Andreato",
            Url = new Uri("https://www.linkedin.com/in/gabriel-andreato-97bb8a165/")
        },
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();

// app.UseSerilogRequestLogging();


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();

} catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally
{
    Log.CloseAndFlush();
}
namespace backend.person.api
{
    public partial class Program { }
}
