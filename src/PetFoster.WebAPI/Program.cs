using PetFoster.WebAPI.Extensions;
using Serilog;
using PetFoster.Species.Application.Extensions;
using PetFoster.Species.Presentation.Extensions;
using PetFoster.Volunteers.Infrastructure.BackgroundServices;
using PetFoster.Volunteers.Application.Extensions;
using PetFoster.Volunteers.Presentation.Extensions;
using PetFoster.Volunteers.Infrastructure.Extensions;
using PetFoster.Species.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")
    ?? throw new ArgumentNullException("Seq"))
    .CreateLogger();

builder.Services
    .AddVolunteersApplicationServices()
    .AddVolunteerInfrastructureServices(builder.Configuration)
    .AddVolunteersContracts();

builder.Services
    .AddSpeciesApplicationServices()
    .AddSpeciesInfrastructureServices(builder.Configuration)
    .AddSpeciesContracts();



builder.Services.AddHostedService<FilesCleanerBackgroundService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();

WebApplication app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;