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
using Microsoft.Diagnostics.Tracing.Stacks;

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
	Console.WriteLine($"Got a trace on machine [{trace.MachineName}], with {trace.Events.Count()} events.");
#if false
	Console.WriteLine($"The trace is of type {trace.GetType()}."); // TraceLog
	Console.WriteLine($"and the events of type {trace.Events.GetType()}."); // TraceEvents
#endif
	Console.WriteLine($"and {trace.CallStacks.Count()} callstacks.");
	Console.WriteLine($"Got a trace with {trace.Processes.Count()} processes.");
	Console.WriteLine($"At a sample interval of {trace.SampleProfileInterval.GetType()}.");

	var evstacks = new TraceEventStackSource(trace.Events);
	Console.WriteLine($"got a stacksource of type {evstacks.GetType()}.");
	var ctree = new CallTree(ScalingPolicyKind.TimeMetric);
	ctree.StackSource = evstacks;
	// Console.WriteLine($"The callstack got at its tip {ctree.Root}.");  // this dumps root on xml format
	Console.WriteLine($"The callstack's root's name: {ctree.Root.Name}, which has {ctree.Root.Callees.Count} callees.");

#if false
	//Console.WriteLine($"The stack trace as a string is {trace.CallStacks.ToString()}.");
	//foreach (var s in trace.CallStacks.Take(20)) {
	foreach (var s in trace.CallStacks) {
	  Console.WriteLine($"stack: {s.CodeAddress.FullMethodName}");
	}
#endif

#if false
#if false
	foreach (var p in trace.Processes) {
	  Console.WriteLine($"Got a process with name {p.Name}, which ran for {p.CPUMSec} msec.");
	}
#else
	//foreach (var e in trace.Events.Take(20)) {
	foreach (var e in trace.Events) {
  	  Console.WriteLine($"Event: {e.EventName}.");
	}
#endif
#endif
        // Console.ReadKey();
    }
}