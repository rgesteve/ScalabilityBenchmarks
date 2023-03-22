using System;
using System.IO;
using System.Reflection;
using System.Threading;

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

using Microsoft.Extensions.Hosting;

namespace Analyzer;

public class Program
{
    public static async Task Main(string[] args)
    {
    
        var cmdBuilder = BuildCommandLine()
        .UseHost(_ =>
        {
            return /* Host.CreateDefaultBuilder() */ new HostBuilder()
          .ConfigureAppConfiguration(
            (_, config) =>
              {
                  config.Sources.Clear();
                  //config.AddJsonFile(Path.Combine(System.AppContext.BaseDirectory, "appsettings.json"), optional: true);
                  //config.AddJsonFile("appsettings.json", true, true);
              });
        });
        //	.UseDefaults();

        var parser = cmdBuilder.Build();
        await parser.InvokeAsync(args);
    }

    static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(@"$ dotnet run --inputfile <filename>") {
	  new Option<string>("--inputfile") { IsRequired = true }
        };

        root.Handler = CommandHandler.Create(Run);
        return new CommandLineBuilder(root);
    }

    static void Run(IHost host, string inputfile)
    {
        Console.WriteLine($"***** From the root command, got argument {inputfile}...");
	if (!File.Exists(inputfile) ) {
           Console.WriteLine($"***** {inputfile} does not exist...");
	   return;
	}
	var trace = TraceLog.OpenOrConvert(TraceLog.CreateFromEventPipeDataFile(inputfile));
	Console.WriteLine($"Got a trace with {trace.Events.Count()} events.");
        Console.ReadKey();
    }
}