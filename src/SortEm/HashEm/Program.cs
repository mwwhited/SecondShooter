using HashEm.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HashEm;

internal class Program
{

    //    // https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver16
    //    // Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=D:\Data\MyDB1.mdf

    //    // https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    // https://tomaskohl.com/code/2020-04-07/trying-out-litedb/

    static async Task Main(string[] args) =>
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                //config.AddCommandLine(args,
                //    //CommandLine.BuildParameters<TemplateEngineSettings>()
                //    //           .AddParameters<FileTemplatingSettings>()
                //    );
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<HashingDbContext>();

                services.Configure<HashingOptions>(options => context.Configuration.Bind(nameof(HashingOptions), options));
                services.AddHostedService<HashingService>();

                //services.TryAddSystemExtensions(context.Configuration);
                //services.TryAddHandlebarServices();
            })
            .StartAsync();
}
