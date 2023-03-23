using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using System.CommandLine;
//using System.CommandLine.Invocation;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;  // needed for Parser.InvokeAsync

//using Microsoft.Extensions.Hosting;

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
        .UseHost(_ =>
        {
            return /* Host.CreateDefaultBuilder() */ new HostBuilder()
          /*
            .ConfigureHostConfiguration(
              configHost => { configHost.SetBasePath(Directory.GetCurrentDirectory()); }
            )
            */
          .ConfigureAppConfiguration(
            (_, config) =>
              {
                  config.Sources.Clear();
                  config.AddJsonFile(Path.Combine(System.AppContext.BaseDirectory, "appsettings.json"), optional: true);
                  //config.AddJsonFile("appsettings.json", true, true);
              });
        });
        //	.UseDefaults();

        var parser = cmdBuilder.Build();
        await parser.InvokeAsync(args);

#endif
    }

    static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(@"$ dotnet run --maxpar 4") {
        new Option<int>("--maxpar") { IsRequired = true }
      };

        root.Handler = CommandHandler.Create(Run);
        return new CommandLineBuilder(root);
    }

    static void Run(IHost host, int maxpar)
    {
        Console.WriteLine("running here");
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var nameFromConf = configuration["name"];
	var myPID = Process.GetCurrentProcess().Id;
        Console.WriteLine($"***** From the root command (pid={myPID}), got argument {maxpar}, and from config {nameFromConf}...");

        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out _);
        var nProc = Environment.ProcessorCount;
        Console.WriteLine($"Ready to run on {nProc} processors, with runtime set to use at most {maxWorkerThreads} threads.");

        Console.ReadKey();
        RunWorkloads();
        Console.WriteLine("Done driver!");
    }

    static void RunWorkloads()
    {
        //TPLBenchmarks.SimpleSync();
        //TPLBenchmarks.SimpleAsync();
        //TPLBenchmarks.SimpleAsyncContinuation();
        //TPLBenchmarks.SimpleExecBlockConfig();
        //TPLSingleProducer.RunConstrained(true);
        //TPLFullPipeline.Run();
        PForPi.Run();

        Console.WriteLine("Ran all workloads!");
        //	Console.ReadKey();
    }

}
