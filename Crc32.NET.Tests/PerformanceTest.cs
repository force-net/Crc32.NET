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
		public void ThroughputCrc32_By_Dexiom()
		{
			Calculate(new Dexiom_Quick_Crc32());
		}

		[Test]
		public void ThroughputCrc32_By_Me()
		{
			Calculate(new Force_Crc32_Crc32Algorithm());
		}

		[Test]
		public void ThroughputCrc32_By_Me_Unaligned()
		{
			Calculate(new Force_Crc32_Crc32Algorithm(), 60);
		}

		private void Calculate(CrcCalculator implementation, int size = 65536)
		{
			var data = new byte[size];
			var random = new Random();
			random.NextBytes(data);

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var maxRate = 0.0;
			for (var i = 0; i < 3; i++)
			{
				long total = 0;
				stopwatch.Restart();
				while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
				{
					implementation.Calculate(data);
					total += data.Length;
				}

				stopwatch.Stop();
				maxRate = Math.Max(total / stopwatch.Elapsed.TotalSeconds / 1024 / 1024, maxRate);
			}

			Console.WriteLine(
				"{0} Throughput: {1:0.0} MB/s",
				implementation.Name,
				maxRate);
		}
	}
}
