/* This is .NET safe implementation of Crc32C algorithm.
 * See detailed comments in Crc32 implementation
 * 
 * Max Vysokikh, 2016-2026
 */

#if NETCOREAPP
using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using ArmCrc32 = System.Runtime.Intrinsics.Arm.Crc32;

#endif

namespace Force.Crc32
{
	internal class SafeProxyC : SafeProxy
	{
		private const uint Poly = 0x82F63B78u;

#if NETCOREAPP
		public new uint Append(uint initial, ReadOnlySpan<byte> input)
		{
			if (Sse42.X64.IsSupported)
				return AppendX64(initial, input);
			if (Sse42.IsSupported)
				return AppendX86(initial, input);
			if (ArmCrc32.IsSupported)
				return AppendArm(initial, input);

			return base.Append(initial, input);
		}

		private static uint AppendX64(uint initial, ReadOnlySpan<byte> input)
		{
			ulong crc = ~initial;
			int offset = 0;
			if (input.Length >= 1024)
			{
				// Four independent CRC chains hide the latency of the CRC32 instruction.
				// Use smaller blocks on short inputs to avoid a long sequential tail.
				int block = input.Length >= 4096 ? 1024 : 256;
				uint[] shift = block == 1024 ? Shift1024.Table : Shift256.Table;
				while (input.Length - offset >= block * 4)
				{
					ulong crc1 = 0, crc2 = 0, crc3 = 0;
					int end = offset + block;
					while (offset < end)
					{
						crc = Sse42.X64.Crc32(crc, ReadUInt64(input, offset));
						crc1 = Sse42.X64.Crc32(crc1, ReadUInt64(input, offset + block));
						crc2 = Sse42.X64.Crc32(crc2, ReadUInt64(input, offset + block * 2));
						crc3 = Sse42.X64.Crc32(crc3, ReadUInt64(input, offset + block * 3));
						offset += 8;
					}

					// These are raw (not complemented) CRCs. Advance each earlier
					// chain by one block of zero bytes before XORing the next chain.
					crc = Shift((uint)crc, shift) ^ (uint)crc1;
					crc = Shift((uint)crc, shift) ^ (uint)crc2;
					crc = Shift((uint)crc, shift) ^ (uint)crc3;
					offset += block * 3;
				}
			}

			while (input.Length - offset >= 8)
			{
				crc = Sse42.X64.Crc32(crc, ReadUInt64(input, offset));
				offset += 8;
			}

			uint tail = (uint)crc;
			while (offset < input.Length)
				tail = Sse42.Crc32(tail, input[offset++]);
			return ~tail;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ReadUInt64(ReadOnlySpan<byte> input, int offset)
		{
			// Only used by x64. The loop checks that a complete 8-byte word remains.
			return Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref MemoryMarshal.GetReference(input), offset));
		}

		private static uint AppendX86(uint initial, ReadOnlySpan<byte> input)
		{
			uint crc = ~initial;
			int offset = 0;
			while (input.Length - offset >= 4)
			{
				crc = Sse42.Crc32(crc, BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(offset)));
				offset += 4;
			}

			while (offset < input.Length)
				crc = Sse42.Crc32(crc, input[offset++]);
			return ~crc;
		}

		private static uint AppendArm(uint initial, ReadOnlySpan<byte> input)
		{
			uint crc = ~initial;
			int offset = 0;
			if (ArmCrc32.Arm64.IsSupported)
			{
				while (input.Length - offset >= 8)
				{
					crc = ArmCrc32.Arm64.ComputeCrc32C(crc,
						BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(offset)));
					offset += 8;
				}
			}

			if (input.Length - offset >= 4)
			{
				do
				{
					crc = ArmCrc32.ComputeCrc32C(crc, BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(offset)));
					offset += 4;
				} while (input.Length - offset >= 4);
			}

			while (offset < input.Length)
				crc = ArmCrc32.ComputeCrc32C(crc, input[offset++]);
			return ~crc;
		}

		private static class Shift256
		{
			internal static readonly uint[] Table = CreateShiftTable(256);
		}

		private static class Shift1024
		{
			internal static readonly uint[] Table = CreateShiftTable(1024);
		}

		private static uint[] CreateShiftTable(int length)
		{
			// The GF(2) operator for appending 'length' zero bytes, expanded
			// into four byte lookup tables. Initialized only for the parallel path.
			var basis = new uint[32];
			for (int bit = 0; bit < 32; bit++)
			{
				uint crc = 1u << bit;
				for (int i = 0; i < length * 8; i++)
					crc = (crc >> 1) ^ ((crc & 1) != 0 ? Poly : 0);
				basis[bit] = crc;
			}

			var table = new uint[1024];
			for (int b = 0; b < 4; b++)
			for (int value = 0; value < 256; value++)
			for (int bit = 0; bit < 8; bit++)
				if ((value & (1 << bit)) != 0)
					table[b * 256 + value] ^= basis[b * 8 + bit];
			return table;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Shift(uint crc, uint[] table)
		{
			return table[(byte)crc] ^ table[256 + (byte)(crc >> 8)]
									^ table[512 + (byte)(crc >> 16)] ^ table[768 + (byte)(crc >> 24)];
		}
#endif

		internal SafeProxyC()
		{
			Init(Poly);
		}
	}
}