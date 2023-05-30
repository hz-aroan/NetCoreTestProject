using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Features.Events;
using LIB.Domain.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Diagnostics;
using System.IO;

namespace Test;

internal class TestHostConfiguration
{
    public static IHost Configure(Action<HostBuilderContext, IServiceCollection>? configureServicesCallback = null)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configBuilder =>
            {
                var basePath = GetBasePath();
                configBuilder.SetBasePath(basePath);
                configBuilder.AddJsonFile(System.IO.Path.Combine(basePath,"appsettings.json"));
            })
            .ConfigureServices((ctx, services) =>
            {
                var connectionsStringPattern = ctx.Configuration.GetConnectionString("MainDatabase");
                var mainConnectionString = ResolveConnectionStringPattern(connectionsStringPattern);
                services.AddPooledDbContextFactory<MainDbContext>(options => options.UseSqlServer(mainConnectionString));

                services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly(LIB.Domain.AssemblyReference.Assembly));
                services.AddSingleton<ICurrencyHandlingService, CurrencyHandlingService>();
                services.AddScoped<IEFWrapper, EFWrapper>();
                services.AddScoped<IBasketHandlingService, BasketHandlingService>();

                configureServicesCallback?.Invoke(ctx, services);
            })
            .Build();
        
        return host;
    }


    private static String ResolveConnectionStringPattern(String connStringPattern)
    {
         var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
         var fullPath = Path.GetFullPath(Path.Combine(directoryPath, "../../../TestDB"));
         var finalFileName = $"{fullPath}\\HexaIOTest.mdf";

         if (File.Exists(finalFileName) == false)
             throw new Exception($"MDF file was not found {finalFileName}");

         var result = connStringPattern.Replace("[MDF_FILE_NAME_HERE]", finalFileName);
        return result;
    }
    
    private static String GetBasePath()
    {
        using var processModule = Process.GetCurrentProcess().MainModule;
        return Path.GetDirectoryName(processModule?.FileName);
    }
}
