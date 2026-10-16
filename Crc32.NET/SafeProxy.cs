/* This is .NET safe implementation of Crc32 algorithm.
 * This implementation was investigated as fastest from different variants. It based on Robert Vazan native implementations of Crc32C
 * Also, it is good for x64 and for x86, so, it seems, there is no sense to do 2 different realizations.
 * 
 * Addition: some speed increase was found with splitting xor to 4 independent blocks. Also, some attempts to optimize unaligned tails was unsuccessful (JIT limitations?).
 * 
 * 
 * Max Vysokikh, 2016-2026
 */

#if NETCOREAPP
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace Force.Crc32
{
	internal class SafeProxy
	{
		private const uint Poly = 0xedb88320u;

		private readonly uint[] _table = new uint[16 * 256];

		internal SafeProxy()
		{
			Init(Poly);
		}

		protected void Init(uint poly)
		{
			var table = _table;
			for (uint i = 0; i < 256; i++)
			{
				uint res = i;
				for (int t = 0; t < 16; t++)
				{
					for (int k = 0; k < 8; k++) res = (res & 1) == 1 ? poly ^ (res >> 1) : (res >> 1);
					table[(t * 256) + i] = res;
				}
			}
		}

#if NETCOREAPP
		public uint Append(uint crc, ReadOnlySpan<byte> input)
		{
			int offset = 0;
			int length = input.Length;
			// Reads are guarded by complete 16-byte or 4-byte groups; the last 0-3 bytes are read individually.
			// All table indices select a byte within one of the 16 fixed 256-entry slices.
			uint crcLocal = uint.MaxValue ^ crc;

			ref uint table = ref MemoryMarshal.GetArrayDataReference(_table);
			ref byte data = ref MemoryMarshal.GetReference(input);
			while (length >= 16)
			{
				uint first = ReadUInt32(ref data, offset) ^ crcLocal;
				uint second = ReadUInt32(ref data, offset + 4);
				uint third = ReadUInt32(ref data, offset + 8);
				uint fourth = ReadUInt32(ref data, offset + 12);

				var a = Unsafe.Add(ref table, (3 * 256) + (byte)fourth)
					^ Unsafe.Add(ref table, (2 * 256) + (byte)(fourth >> 8))
					^ Unsafe.Add(ref table, (1 * 256) + (byte)(fourth >> 16))
					^ Unsafe.Add(ref table, (0 * 256) + (byte)(fourth >> 24));

				var b = Unsafe.Add(ref table, (7 * 256) + (byte)third)
					^ Unsafe.Add(ref table, (6 * 256) + (byte)(third >> 8))
					^ Unsafe.Add(ref table, (5 * 256) + (byte)(third >> 16))
					^ Unsafe.Add(ref table, (4 * 256) + (byte)(third >> 24));

				var c = Unsafe.Add(ref table, (11 * 256) + (byte)second)
					^ Unsafe.Add(ref table, (10 * 256) + (byte)(second >> 8))
					^ Unsafe.Add(ref table, (9 * 256) + (byte)(second >> 16))
					^ Unsafe.Add(ref table, (8 * 256) + (byte)(second >> 24));

				var d = Unsafe.Add(ref table, (15 * 256) + (byte)first)
					^ Unsafe.Add(ref table, (14 * 256) + (byte)(first >> 8))
					^ Unsafe.Add(ref table, (13 * 256) + (byte)(first >> 16))
					^ Unsafe.Add(ref table, (12 * 256) + (byte)(first >> 24));

				crcLocal = d ^ c ^ b ^ a;
				offset += 16;
				length -= 16;
			}

			while (length >= 4)
			{
				uint word = ReadUInt32(ref data, offset) ^ crcLocal;
				crcLocal = Unsafe.Add(ref table, (3 * 256) + (byte)word)
					^ Unsafe.Add(ref table, (2 * 256) + (byte)(word >> 8))
					^ Unsafe.Add(ref table, (1 * 256) + (byte)(word >> 16))
					^ Unsafe.Add(ref table, (0 * 256) + (byte)(word >> 24));
				offset += 4;
				length -= 4;
			}

			while (--length >= 0)
				crcLocal = Unsafe.Add(ref table, (byte)(crcLocal ^ Unsafe.Add(ref data, offset++))) ^ crcLocal >> 8;

			return crcLocal ^ uint.MaxValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint ReadUInt32(ref byte data, int offset)
		{
			// Slice offsets need not be aligned. Read exactly four bytes and normalize
			// their order so that the slicing tables retain their little endian layout.
			uint word = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref data, offset));
			return BitConverter.IsLittleEndian ? word : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(word);
		}
#else
		public uint Append(uint crc, byte[] input, int offset, int length)
		{
			uint crcLocal = uint.MaxValue ^ crc;

			uint[] table = _table;
			while (length >= 16)
			{
				var a = table[(3 * 256) + input[offset + 12]]
					^ table[(2 * 256) + input[offset + 13]]
					^ table[(1 * 256) + input[offset + 14]]
					^ table[(0 * 256) + input[offset + 15]];

				var b = table[(7 * 256) + input[offset + 8]]
					^ table[(6 * 256) + input[offset + 9]]
					^ table[(5 * 256) + input[offset + 10]]
					^ table[(4 * 256) + input[offset + 11]];

				var c = table[(11 * 256) + input[offset + 4]] 
					^ table[(10 * 256) + input[offset + 5]] 
					^ table[(9 * 256) + input[offset + 6]] 
					^ table[(8 * 256) + input[offset + 7]];

				var d = table[(15 * 256) + ((byte)crcLocal ^ input[offset])]
					^ table[(14 * 256) + ((byte)(crcLocal >> 8) ^ input[offset + 1])]
					^ table[(13 * 256) + ((byte)(crcLocal >> 16) ^ input[offset + 2])]
					^ table[(12 * 256) + ((crcLocal >> 24) ^ input[offset + 3])];

				crcLocal = d ^ c ^ b ^ a;
				offset += 16;
				length -= 16;
			}

			while (--length >= 0)
				crcLocal = table[(byte)(crcLocal ^ input[offset++])] ^ crcLocal >> 8;

			return crcLocal ^ uint.MaxValue;
		}
#endif
	}
}
