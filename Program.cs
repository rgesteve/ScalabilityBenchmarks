using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Builder;
//using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;  // needed for Parser.InvokeAsync

#if false
using GHConsole2;
#else
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
#if false
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true, true)
        .Build();

        Console.WriteLine($"****** Hello, world! {config["name"]} ****");
#else
	//await BuildCommandLine().Build().InvokeAsync(args);

	var parser = BuildCommandLine().Build();
      await parser.InvokeAsync(args);

#endif
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
      Console.WriteLine($"***** From the root command, got argument {name}...");
    }

}

#endif