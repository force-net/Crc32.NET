using System;
using BenchmarkDotNet.Running;

namespace Crc32.NET.Benchmarks
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new StandardConfig());
    }
}
