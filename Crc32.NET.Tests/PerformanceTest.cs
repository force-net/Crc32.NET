using System;
using System.Data.HashFunction;
using System.Diagnostics;

using NUnit.Framework;

using E = Crc32.Crc32Algorithm;

namespace Force.Crc32.Tests
{
	[TestFixture]
	public class PerformanceTest
	{
		[Test]
		public void ThroughputCHCrc32_By_tanglebones()
		{
			var data = new byte[65536];
			var random = new Random();
			random.NextBytes(data);
			long total = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				CH.Crc32.Crc.Crc32(data);
				total += data.Length;
			}

			stopwatch.Stop();
			Console.WriteLine("Throughput: {0:0.0} MB/s", total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
		}

		[Test]
		public void ThroughputCrc32_By_dariogriffo()
		{
			var data = new byte[65536];
			var random = new Random();
			random.NextBytes(data);
			long total = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				E.Compute(data);
				total += data.Length;
			}

			stopwatch.Stop();
			Console.WriteLine("Throughput: {0:0.0} MB/s", total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
		}

		[Test]
		public void ThroughputCrc32_By_Data_HashFunction_Crc()
		{
			var data = new byte[65536];
			var random = new Random();
			random.NextBytes(data);
			var crc = new CRC();
			long total = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				crc.ComputeHash(data);
				total += data.Length;
			}

			stopwatch.Stop();
			Console.WriteLine("Throughput: {0:0.0} MB/s", total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
		}

		[Test]
		public void ThroughputCrc32_By_Me()
		{
			var data = new byte[65536];
			var random = new Random();
			random.NextBytes(data);
			long total = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				Crc32Algorithm.Compute(data);
				total += data.Length;
			}

			stopwatch.Stop();
			Console.WriteLine("Throughput: {0:0.0} MB/s", total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
		}
	}
}
