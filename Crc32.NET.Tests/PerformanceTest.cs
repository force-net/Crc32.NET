using System;
using System.Diagnostics;
using Force.Crc32.Tests.Crc32Implementations;
using NUnit.Framework;

namespace Force.Crc32.Tests
{
	[TestFixture]
	public class PerformanceTest
	{
		[Test]
		public void ThroughputCHCrc32_By_tanglebones()
		{
			Calculate(new CH_Crc32_Crc());
		}

		[Test]
		public void ThroughputKlinkby_Checksum()
		{
			Calculate(new Klinkby_Checkum_Crc32());
		}

		[Test]
		public void ThroughputCrc32_By_dariogriffo()
		{
			Calculate(new Crc32_Crc32Algorithm());
		}

		[Test]
		public void ThroughputCrc32_By_Data_HashFunction_Crc()
		{
			Calculate(new System_Data_HashFunction_CRC());
		}

		[Test]
		public void ThroughputCrc32_By_Me()
		{
			Calculate(new Force_Crc32_Crc32Algorithm());
		}

		private void Calculate(CrcCalculator implementation)
		{
			var data = new byte[65536];
			var random = new Random();
			random.NextBytes(data);
			long total = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				implementation.Calculate(data);
				total += data.Length;
			}

			stopwatch.Stop();

			Console.WriteLine(
				"{0} Throughput: {1:0.0} MB/s",
				implementation.Name,
				total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
		}
	}
}
