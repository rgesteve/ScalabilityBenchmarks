// from https://adamsitnik.com/PerfCollectProfiler/
using System;
using System.IO;

using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    // [PerfCollectProfiler(performExtraBenchmarksRun: false)]
    public class IntroPerfCollectProfiler
    {
        private readonly string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        private readonly string content = new string('a', 100_000);

        [Benchmark]
        public void WriteAllText() => File.WriteAllText(path, content);

        [GlobalCleanup]
        public void Delete() => File.Delete(path);
    }
}
