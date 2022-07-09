using System;
using System.Security.Cryptography;
#if NETCOREAPP3_0_OR_GREATER
using X86 = System.Runtime.Intrinsics.X86.Sse42;
using X64 = System.Runtime.Intrinsics.X86.Sse42.X64;
#endif
#if NET5_0_OR_GREATER
using Arm32 = System.Runtime.Intrinsics.Arm.Crc32;
using Arm64 = System.Runtime.Intrinsics.Arm.Crc32.Arm64;
using System.Runtime.InteropServices;
#endif

namespace Force.Crc32.Intrinsics
{
    /// <summary>
    /// The hardware implementation of the CRC32C polynomial supported by:
    /// - Intel CPUs supporting SSE4.2
    /// - ARM CPUs since ARMv8.1 
    /// </summary>
    public class Crc32CAlgorithm : HashAlgorithm
    {
        /// <summary>
        /// the current CRC value, bit-flipped
        /// </summary>
        private uint _crc;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Crc32CAlgorithm()
        {
#if NETSTANDARD2_0_OR_GREATER
            this.HashSizeValue = 32;
#endif
            this.Reset();
        }

        /// <summary>
        /// Check if the algorithm is supported on the current CPU
        /// </summary>
        public static bool IsSupported => Platform != Platform.Unsupported;

        /// <summary>
        /// Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class
        /// </summary>
        public override void Initialize()
        {
            this.Reset();
        }

        /// <summary>
        /// Computes CRC32C from input buffer.
        /// </summary>
        /// <param name="input">Input buffer with data to be checksummed.</param>
        /// <returns>CRC32C of the data in the buffer.</returns>
        public static uint Compute(byte[] input)
        {
            return Compute(input, 0, input.Length);
        }

        /// <summary>
        /// Computes CRC32C from input buffer.
        /// </summary>
        /// <param name="input">Input buffer with data to be checksummed.</param>
        /// <param name="offset">Offset of the input data within the buffer.</param>
        /// <param name="length">Length of the input data in the buffer.</param>
        /// <returns>CRC32C of the data in the buffer.</returns>
        public static uint Compute(byte[] input, int offset, int length)
        {
            var algo = new Crc32CAlgorithm();
            algo.HashCore(input, offset, length);
            return ~algo._crc;
        }

        /// <summary>
        /// When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash
        /// </summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            switch (Platform)
            {
                case Platform.X64:
                case Platform.X86:
                    HashCoreX86(array, ibStart, cbSize);
                    break;
                case Platform.Arm64:
                case Platform.Arm32:
                    HashCoreArm(array, ibStart, cbSize);
                    break;
                default:
                    throw new NotSupportedException("CRC32 intrinsics are not suppored on this platform");
            }
        }

        /// <summary>
        /// When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(~_crc);
        }

        private void HashCoreX86(byte[] array, int ibStart, int cbSize)
        {
#if NETCOREAPP3_0_OR_GREATER
            var span = new ReadOnlySpan<byte>(array, ibStart, cbSize);

            if (Platform == Platform.X64 && span.Length > 0)
            {
                unsafe
                {
                    fixed(byte* start = &span[0])
                    {
                        ulong* current = (ulong*)start;
                        ulong* end = current + span.Length / sizeof(ulong);
                        ulong crc = _crc;
                        while (current < end)
                        {
                            crc = X64.Crc32(crc, *current++);
                        }
                        _crc = (uint)crc;

                        span = new ReadOnlySpan<byte>((byte*)current, span.Length % sizeof(ulong));
                    }
                }
            }

            if(span.Length > 0)
            {
                unsafe
                {
                    fixed(byte* start = &span[0])
                    {
                        byte* current = start;
                        byte* end = current + span.Length;
                        while (current < end)
                        {
                            _crc = X86.Crc32(_crc, *current++);
                        }
                    }
                }
            }
#endif
        }

        private void HashCoreArm(byte[] array, int ibStart, int cbSize)
        {
#if NET5_0_OR_GREATER
            var span = new ReadOnlySpan<byte>(array, ibStart, cbSize);

            if (Platform == Platform.Arm64 && span.Length > 0)
            {
                unsafe
                {
                    fixed(byte* start = &span[0])
                    {
                        ulong* current = (ulong*)start;
                        ulong* end = current + span.Length / sizeof(ulong);
                        while (current < end)
                        {
                            _crc = Arm64.ComputeCrc32C(_crc, *current++);
                        }

                        span = new ReadOnlySpan<byte>((byte*)current, span.Length % sizeof(ulong));
                    }
                }
            }

            if(span.Length > 0)
            {
                unsafe
                {
                    fixed(byte* start = &span[0])
                    {
                        byte* current = start;
                        byte* end = current + span.Length;
                        while (current < end)
                        {
                            _crc = Arm32.ComputeCrc32C(_crc, *current++);
                        }
                    }
                }
            }
#endif
        }

        private static Platform Platform =>
#if NETCOREAPP3_0_OR_GREATER
            X64.IsSupported ? Platform.X64 :
            X86.IsSupported ? Platform.X86 :
#endif
#if NET5_0_OR_GREATER
            Arm64.IsSupported ? Platform.Arm64 :
            Arm32.IsSupported ? Platform.Arm32 :
#endif
            Platform.Unsupported;

        private void Reset()
        {
            _crc = uint.MaxValue;
        }
    }
}
