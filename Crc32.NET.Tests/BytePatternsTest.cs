using System;
using System.Linq;
using Force.Crc32.Tests.Crc32Implementations;
using NUnit.Framework;

namespace Force.Crc32.Tests
{
	[TestFixture]
	public class BytePatternsTest
	{
		[Test]
		public void Crc32ForEmptySequenseIs0()
		{
			var actual = Crc32Algorithm.Compute(new byte[0]);
			Assert.That(actual, Is.EqualTo(0));
		}

		// Pattern: 
		// xx
		// xx xx
		// xx xx xx
		// ...
		[Test]
		public void RepeatedBytePatternTest()
		{
			foreach (var x in Enumerable.Range(0, 256))
			{
				foreach (int len in Enumerable.Range(1, 32))
				{
					var data = Enumerable.Repeat((byte)x, len).ToArray();
					TestByteSequence(data);
				}
			}
		}

		// Pattern:
		// xx
		// xx 00
		// 00 xx
		// xx 00 00
		// 00 xx 00
		// 00 00 xx
		// ...

		// xx
		// xx FF
		// FF xx
		// xx FF FF
		// FF xx FF
		// FF FF xx
		// ...
		[TestCase(0x00)]
		[TestCase(0xFF)]
		public void SlidingBytePatternTest(byte fillValue)
		{
			foreach (int len in Enumerable.Range(1, 32))
			{
				var data = Enumerable.Repeat(fillValue, len).ToArray();

				foreach (var i in Enumerable.Range(0, len))
				{
					foreach (var x in Enumerable.Range(0, 256))
					{
						data[i] = (byte)x;
						TestByteSequence(data);
					}

					data[i] = fillValue;
				}
			}
		}

		private void TestByteSequence(byte[] data)
		{
			var actual = Crc32Algorithm.Compute(data);
			var expected = _referenceImplementation.Calculate(data);

			if (expected != actual)
			{
				var message = string.Format(
					"Test failed for {0}\nExpected: {1:x8}\nBut was: {2:x8}",
					BitConverter.ToString(data),
					expected,
					actual);
				Assert.Fail(message);
			}
		}

		private readonly CrcCalculator _referenceImplementation = new System_Data_HashFunction_CRC();
	}
}
