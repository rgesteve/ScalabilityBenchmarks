using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GHConsole2;

public class TPLSingleProducer
{
    public static void RunConstrained(bool shouldConstrain)
    {
        var sw = new Stopwatch();
        const int ITERS = 600_000;
        var are = new AutoResetEvent(false);

        var opts = new ExecutionDataflowBlockOptions()
        {
            // SingleProducerConstrained = false /*true*/
            SingleProducerConstrained = shouldConstrain
        };

        var ab = new ActionBlock<int>(i => { if (i == ITERS) are.Set(); }, opts);
        while (true)
        {
            sw.Restart();
            for (int i = 0; i <= ITERS; i++) ab.Post(i);
            are.WaitOne();
            sw.Stop();
            Console.WriteLine($"Messages/sec {(ITERS / sw.Elapsed.TotalSeconds)}.");
        }

        Console.WriteLine("Done!");
    }
}
