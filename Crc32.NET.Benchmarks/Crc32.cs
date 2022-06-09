using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Force.Crc32;

namespace Crc32.NET.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class Crc32
    {
        private byte[] _input;
        private byte[] _destination;

        [Params(65536)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _input = new byte[Size];
            var random = new Random();
            random.NextBytes(_input);

            _destination = new byte[4];
        }

        [Benchmark(Baseline = true)]
        public byte[] Array()
        {
            var crc = new Crc32Algorithm();
            return crc.ComputeHash(_input);
        }

#if NETCOREAPP3_1
        [Benchmark]
        public void Span()
        {
            var crc = new Crc32Algorithm();
            crc.TryComputeHash(_input.AsSpan(), _destination.AsSpan(), out _);
        }
#endif
    }
}
