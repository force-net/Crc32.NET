using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using E = Crc32.Crc32Algorithm;

namespace Force.Crc32.Tests
{
	[TestFixture]
	public class ImplementationTest
	{
		[TestCase("Hello", 3)]
		[TestCase("Nazdar", 0)]
		[TestCase("Ahoj", 1)]
		[TestCase("Very long text.Very long text.Very long text.Very long text.Very long text.Very long text.Very long text", 0)]
		[TestCase("Very long text.Very long text.Very long text.Very long text.Very long text.Very long text.Very long text", 3)]
		public void ResultConsistency(string text, int offset)
		{
			var bytes = Encoding.ASCII.GetBytes(text);

			var crc1 = E.Compute(bytes.Skip(offset).ToArray());
			var crc2 = Crc32Algorithm.Append(0, bytes, offset, bytes.Length - offset);
			Assert.That(crc2, Is.EqualTo(crc1));
		}

		[Test]
		public void ResultConsistency2()
		{
			Assert.That(Crc32Algorithm.Compute(new byte[] { 1 }), Is.EqualTo(2768625435));
			Assert.That(Crc32Algorithm.Compute(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }), Is.EqualTo(622876539));
		}

		[Test]
		public void ResultConsistencyAsHashAlgorithm()
		{
			var bytes = new byte[30000];
			new Random().NextBytes(bytes);
			var e = new E();
			var c = new Crc32Algorithm();
			var crc1 = BitConverter.ToInt32(e.ComputeHash(bytes), 0);
			var crc2 = BitConverter.ToInt32(c.ComputeHash(bytes), 0);
			Console.WriteLine(crc1.ToString("X8"));
			Console.WriteLine(crc2.ToString("X8"));
			Assert.That(crc1, Is.EqualTo(crc2));
		}

		[Test]
		public void PartIsWhole()
		{
			var bytes = new byte[30000];
			new Random().NextBytes(bytes);
			var r1 = Crc32Algorithm.Append(0, bytes, 0, 15000);
			var r2 = Crc32Algorithm.Append(r1, bytes, 15000, 15000);
			var r3 = Crc32Algorithm.Append(0, bytes, 0, 30000);
			Assert.That(r2, Is.EqualTo(r3));
		}
	}
}
