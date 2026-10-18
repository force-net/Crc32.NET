#if NETCOREAPP
using System;
using System.Buffers.Binary;
using ArmCrc32 = System.Runtime.Intrinsics.Arm.Crc32;

namespace Force.Crc32
{
	internal static class ArmCrc32Proxy
	{
		internal static bool IsSupported => ArmCrc32.IsSupported;

		public static uint Append(uint initial, ReadOnlySpan<byte> input)
		{
			System.Diagnostics.Debug.Assert(IsSupported);
			uint crc = ~initial;
			int offset = 0;
			if (ArmCrc32.Arm64.IsSupported)
			{
				while (input.Length - offset >= 8)
				{
					crc = ArmCrc32.Arm64.ComputeCrc32(crc, BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(offset)));
					offset += 8;
				}
			}

			// ARM32 processes full words here; ARM64 handles its remaining 0-7 bytes.
			while (input.Length - offset >= 4)
			{
				crc = ArmCrc32.ComputeCrc32(crc, BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(offset)));
				offset += 4;
			}

			if (input.Length - offset >= 2)
			{
				crc = ArmCrc32.ComputeCrc32(crc, BinaryPrimitives.ReadUInt16LittleEndian(input.Slice(offset)));
				offset += 2;
			}

			if (offset < input.Length)
				crc = ArmCrc32.ComputeCrc32(crc, input[offset]);
			return ~crc;
		}
	}
}
#endif
