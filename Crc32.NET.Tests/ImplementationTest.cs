using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

#if !NETCORE
using E = Crc32.Crc32Algorithm;
#endif


namespace Force.Crc32.Tests
{
	[TestFixture]
	public class ImplementationTest
	{

#if !NETCORE
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
#endif

		[Test]
		public void ResultConsistency2()
		{
			Assert.That(Crc32Algorithm.Compute(new byte[] { 1 }), Is.EqualTo(2768625435));
			Assert.That(Crc32Algorithm.Compute(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }), Is.EqualTo(622876539));
		}

#if !NETCORE
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
#endif

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

		[Test]
		public void Result_Is_BigEndian()
		{
			var bytes = new byte[30000];
			new Random().NextBytes(bytes);
			var crc1 = Crc32Algorithm.Append(0, bytes, 0, bytes.Length);
			var crc2Bytes = new Crc32Algorithm().ComputeHash(bytes);
			if (BitConverter.IsLittleEndian) crc2Bytes = crc2Bytes.Reverse().ToArray();
			var crc2 = BitConverter.ToUInt32(crc2Bytes, 0);
			Assert.That(crc2, Is.EqualTo(crc1));
		}

		[Test]
		public void Result_Is_LittleEndian_IF_Specified()
		{
			var bytes = new byte[30000];
			new Random().NextBytes(bytes);
			var crc1 = Crc32Algorithm.Append(0, bytes, 0, bytes.Length);
			var crc2Bytes = new Crc32Algorithm(false).ComputeHash(bytes);
			if (!BitConverter.IsLittleEndian) crc2Bytes = crc2Bytes.Reverse().ToArray();
			var crc2 = BitConverter.ToUInt32(crc2Bytes, 0);
			Assert.That(crc2, Is.EqualTo(crc1));
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[TestCase(11)]
		[TestCase(30)]
		[TestCase(200)]
		[TestCase(10000)]
		public void Computation_With_Crc_End_Should_Be_Validated(int length)
		{
			var buf = new byte[length + 4];
			var r = new Random();
			r.NextBytes(buf);
			Crc32Algorithm.ComputeAndWriteToEnd(buf);
			Assert.That(Crc32Algorithm.IsValidWithCrcAtEnd(buf), Is.True);
			buf[r.Next(buf.Length)] ^= 0x1;
			Assert.That(Crc32Algorithm.IsValidWithCrcAtEnd(buf), Is.False);

			// partial test
			if (length > 2)
			{
				Crc32Algorithm.ComputeAndWriteToEnd(buf, 1, length - 2);
				Assert.That(Crc32Algorithm.IsValidWithCrcAtEnd(buf, 1, length - 2 + 4), Is.True);
				buf[1 + r.Next(buf.Length - 2)] ^= 0x1;
				Assert.That(Crc32Algorithm.IsValidWithCrcAtEnd(buf, 1, length - 2 + 4), Is.False);
			}
		}

		[Test]
		public void Compute_With_Stream_Should_Be_Consistent()
		{
			var buf = new byte[5000];
			var r = new Random();
			r.NextBytes(buf);

			using (var ms = new MemoryStream(buf))
			{
				Assert.AreEqual(Crc32Algorithm.Compute(buf), Crc32Algorithm.Compute(ms));
			}
		}

		[Test]
		public void Compute_And_Write_To_End_With_Stream_Should_Be_Consistent()
		{
			var buf = new byte[5000];
			var r = new Random();
			r.NextBytes(buf);

			var copyA = new byte[buf.Length + 4];
			Array.Copy(buf, copyA, buf.Length);

			Crc32Algorithm.ComputeAndWriteToEnd(copyA);

			using (var ms = new MemoryStream())
			{
				ms.Write(buf, 0, buf.Length);
				ms.Seek(0, SeekOrigin.Begin);

				Crc32Algorithm.ComputeAndWriteToEnd(ms, buf.Length);

				ms.Seek(-4, SeekOrigin.End);

				for (int i = 0; i < 4; ++i)
				{
					var index = i + buf.Length;
					Assert.AreEqual(copyA[index], ms.ReadByte());
				}
			}
		}
	}
}
