using System.Text.Json.Serialization;
using AosAdjutant.Api.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false))
    );
builder.Services.AddProblemDetails();

builder.Services.AddHttpLogging(opts => { });

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration["AosAdjutant:DbContextConnectionString"])
);

//builder.Services.AddDbContext<ApplicationDbContext>(opt =>
//    opt.UseNpgsql(builder.Configuration.GetConnectionString("aosadjutant"))
//);

var app = builder.Build();

app.UseHttpLogging();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
