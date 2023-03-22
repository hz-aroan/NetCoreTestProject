using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Features.Events;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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

                services.AddScoped<IDispatcher, Dispatcher>();
                CqAutoRegister.BuildCqTypes(services,typeof(GetAllAvailableEventsQry).Assembly);

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
