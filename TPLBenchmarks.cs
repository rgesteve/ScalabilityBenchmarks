using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GHConsole2;

public class TPLBenchmarks
{
    public static void SimpleSync()
    {
        Func<int, int> fun = x =>
        {
            Thread.Sleep(500);
            return x * x;
        };
        var tfBlock = new TransformBlock<int, int>(fun);
        for (int i = 0; i < 10; i++)
        {
            tfBlock.Post(i);
        }
        for (int i = 0; i < 10; i++)
        {
            int res = tfBlock.Receive();
            Console.WriteLine($"From index {i}: {res}...");
        }
        Console.WriteLine("Done!");
    }

    public static void SimpleAsync()
    {
        Func<int, int> fun = x =>
        {
            Thread.Sleep(500);
            return x * x;
        };
        var tfBlock = new TransformBlock<int, int>(fun);
        for (int i = 0; i < 10; i++)
        {
            tfBlock.Post(i);
        }
        for (int i = 0; i < 10; i++)
        {
            Task<int> resTask = tfBlock.ReceiveAsync();
            int res = resTask.Result;
            Console.WriteLine($"From index {i}: {res}...");
        }
        Console.WriteLine("Done!");
    }

    public static void SimpleAsyncContinuation()
    {
        Func<int, int> fun = x =>
        {
            Thread.Sleep(500);
            return x * x;
        };
        var tfBlock = new TransformBlock<int, int>(fun);
        for (int i = 0; i < 10; i++)
        {
            tfBlock.Post(i);
        }

	Action< Task<int> > whenReady = task => {
          int res = task.Result;
          Console.WriteLine($"In action: {res}...");
	};

	for (int i = 0; i < 10; i++)
        {
            Task<int> resTask = tfBlock.ReceiveAsync();
	    Task t = resTask.ContinueWith(whenReady);
	    t.Wait();  // is this necessary?
        }
        Console.WriteLine("Done!");
    }

    public static void SimpleExecBlockConfig()
    {
      var generator = new Random();
      Action<int> fun = n => {
        Thread.Sleep(generator.Next(1000));
	Console.WriteLine($"Just completed for {n}");
      };
      var actionBlock = new ActionBlock<int>(fun, new ExecutionDataflowBlockOptions {
	MaxDegreeOfParallelism = 2
      });
      for (int i = 0; i < 10; i++) {
        Console.WriteLine($"Posting {i}");
        actionBlock.Post(i);
      }
      actionBlock.Complete();
      actionBlock.Completion.Wait(); // does this not know how many tasks it's waiting on?
      Console.WriteLine("Done!");
    }
}