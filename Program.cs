using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;  // needed for Parser.InvokeAsync

using Microsoft.Extensions.Hosting;

namespace GHConsole2;

public class Program
{

    public static async Task Main(string[] args)
    {
#if false
      IHost host = Host.CreateDefaultBuilder(args)
                       .ConfigureServices(services => {
                                            services.AddHostedService<Worker>();
                                          })
                       .Build();

      host.Run();
#else

	//await BuildCommandLine().Build().InvokeAsync(args);

	var cmdBuilder = BuildCommandLine()
	.UseHost( _ => {
	  return /* Host.CreateDefaultBuilder() */ new HostBuilder()
	    .ConfigureHostConfiguration(
	      configHost => { configHost.SetBasePath(Directory.GetCurrentDirectory()); }
	    )
	    .ConfigureAppConfiguration(
	      (_, config) => {
	        //config.Sources.Clear();
	        //config.AddJsonFile(Path.Combine(System.AppContext.BaseDirectory, "appsettings.json"), optional : true);
		config.AddJsonFile("appsettings.json", true, true);
	      });
	});
//	.UseDefaults();

	var parser = cmdBuilder.Build();
      await parser.InvokeAsync(args);

#endif
    }

    static CommandLineBuilder BuildCommandLine()
    {
      var root = new RootCommand(@"$ dotnet run --name 'Joe'") {
        new Option<string>("--name") { IsRequired = true }
      };

      root.Handler = CommandHandler.Create(Run);
      return new CommandLineBuilder(root);
    }

    static void Run(IHost host, string name)
    {
      var configuration = host.Services.GetRequiredService<IConfiguration>();
      var nameFromConf = configuration["name"];
      Console.WriteLine($"***** From the root command, got argument {name}, and from config {nameFromConf}...");
    }

}
