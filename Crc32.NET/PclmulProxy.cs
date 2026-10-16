#if NETCOREAPP
// Polynomial folding/reduction adapted from dotnet/runtime Crc32.Vectorized.cs.
// Copyright (c) .NET Foundation and Contributors. Licensed under the MIT license.
// Source: https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.IO.Hashing/src/System/IO/Hashing/Crc32.Vectorized.cs
// The retained implementation uses three independent 128-bit folding chains.
// Fold constants: reflect32(x^(128*N+32) mod 0x104C11DB7) << 1,
//		 reflect32(x^(128*N-32) mod 0x104C11DB7) << 1.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Force.Crc32
{
	internal static class PclmulProxy
	{
		internal static bool IsSupported => Pclmulqdq.IsSupported && Sse2.IsSupported;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<ulong> Load(ReadOnlySpan<byte> input, int offset)
		{
			return Unsafe.ReadUnaligned<Vector128<ulong>>(ref Unsafe.Add(ref MemoryMarshal.GetReference(input), offset));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<ulong> Fold(Vector128<ulong> target, Vector128<ulong> source,
			Vector128<ulong> constants)
		{
			return Sse2.Xor(target, Sse2.Xor(Pclmulqdq.CarrylessMultiply(source, constants, 0x00), Pclmulqdq.CarrylessMultiply(source, constants, 0x11)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Reduce(Vector128<ulong> crc)
		{
			var mask = Vector128.Create(0xFFFFFFFFUL, 0xFFFFFFFFUL);
			crc = Sse2.Xor(Sse2.ShiftRightLogical128BitLane(crc, 8), Pclmulqdq.CarrylessMultiply(crc, Vector128.CreateScalar(0x00CCAA009EUL), 0x00));
			crc = Sse2.Xor(Pclmulqdq.CarrylessMultiply(Sse2.And(crc, mask), Vector128.CreateScalar(0x0163CD6124UL), 0x00), Sse2.ShiftRightLogical128BitLane(crc, 4));
			var constants = Vector128.Create(0x01DB710641UL, 0x01F7011641UL);
			var quotient = Sse2.And(Pclmulqdq.CarrylessMultiply(Sse2.And(crc, mask), constants, 0x10), mask);
			crc = Sse2.Xor(crc, Pclmulqdq.CarrylessMultiply(quotient, constants, 0x00));
			return crc.AsUInt32().GetElement(1);
		}

		public static uint Append(uint initial, ReadOnlySpan<byte> input, SafeProxy table)
		{
			System.Diagnostics.Debug.Assert(IsSupported && input.Length >= 16);
			// Callers guarantee at least 16 bytes. Every later vector load is guarded
			// by a complete group or the remaining 16-byte count.
			int offset = 16;
			var crc = Sse2.Xor(Load(input, 0), Vector128.CreateScalar((ulong)~initial));
			var single = Vector128.Create(0x01751997D0UL, 0x00CCAA009EUL);
			if (input.Length >= 96)
			{
				var crc1 = Load(input, 16);
				var crc2 = Load(input, 32);
				offset = 48;
				var constants = Vector128.Create(0x3DB1ECDCUL, 0x174359406UL);
				do
				{
					crc = Fold(Load(input, offset), crc, constants);
					crc1 = Fold(Load(input, offset + 16), crc1, constants);
					crc2 = Fold(Load(input, offset + 32), crc2, constants);
					offset += 48;
				} while (input.Length - offset >= 48);

				crc = Fold(crc1, crc, single);
				crc = Fold(crc2, crc, single);
			}

			while (input.Length - offset >= 16)
			{
				crc = Fold(Load(input, offset), crc, single);
				offset += 16;
			}

			uint result = ~Reduce(crc);
			return offset == input.Length ? result : table.Append(result, input.Slice(offset));
		}
	}
}
#endif
