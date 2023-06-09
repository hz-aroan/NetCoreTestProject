using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// ------------------------------------------------------------------
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

var mainConnectionString = builder.Configuration.GetConnectionString("MainDatabase");
builder.Services.AddPooledDbContextFactory<MainDbContext>(options => options.UseSqlServer(mainConnectionString));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(LIB.Domain.AssemblyReference.Assembly));


builder.Services.AddSingleton<ICurrencyHandlingService, CurrencyHandlingService>();
builder.Services.AddScoped<IEFWrapper, EFWrapper>();
builder.Services.AddScoped<IBasketHandlingService, BasketHandlingService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HexaIO Events WEB API",
        Description = "A Web API for managing HexaIO Events features",
    });
    options.EnableAnnotations();
});

builder.Services.AddMvc(options => options.Filters.Add(new ProducesAttribute("application/json")) );
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
