using Infrastructure.SQL.Main;

using LIB.Domain.Features.Events;
using LIB.Domain.Services.CQ;

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);



// ------------------------------------------------------------------
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

var mainConnectionString = builder.Configuration.GetConnectionString("MainDatabase");
builder.Services.AddPooledDbContextFactory<MainDbContext>(options => options.UseSqlServer(mainConnectionString));

builder.Services.AddScoped<IDispatcher, Dispatcher>();
CqAutoRegister.BuildCqTypes(builder.Services,typeof(GetAllAvailableEventsQry).Assembly);

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/{1}/Views/Components/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/Site/Shared/Layout/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/Site/Shared/Error/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/{1}/Views/{0}.cshtml");
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();


// ------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{    endpoints.MapAreaControllerRoute(
        name: "",
        areaName: "Site",
        pattern: "{controller=Home}/{action=Index}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


    endpoints.MapAreaControllerRoute(
        name: "default",
        areaName: "Site",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
