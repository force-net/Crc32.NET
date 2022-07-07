using System;
using System.Security.Cryptography;

#if NET5_0_OR_GREATER
using Arm32 = System.Runtime.Intrinsics.Arm.Crc32;
using Arm64 = System.Runtime.Intrinsics.Arm.Crc32.Arm64;
#endif

namespace Force.Crc32.Intrinsics
{
    /// <summary>
    /// The hardware implementation of the CRC32-C polynomial 
    /// implemented on Intel CPUs supporting SSE4.2.
    /// </summary>
    public class Crc32Algorithm : HashAlgorithm
    {
        /// <summary>
        /// the current CRC value, bit-flipped
        /// </summary>
        private uint _crc;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Crc32Algorithm()
        {
            // The size, in bits, of the computed hash code.
            this.HashSizeValue = 32;
            this.Reset();
        }

        /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm"></see> class.</summary>
        public override void Initialize()
        {
            this.Reset();
        }

		/// <summary>
		/// Computes CRC-32C from input buffer.
		/// </summary>
		/// <param name="input">Input buffer containing data to be checksummed.</param>
		/// <returns>CRC-32C of the buffer.</returns>
        public static uint Compute(byte[] input)
		{
			var algo = new Crc32Algorithm();
            algo.HashCore(input);
            return ~algo._crc;
		}


        /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            switch(Platform)
            {
                case Platform.Arm64:
                case Platform.Arm32:
                    HashCoreArm(array, ibStart, cbSize);
                    break;
                default:
                    throw new NotSupportedException("CRC32 intrinsics are not suppored on this platform");
            }
        }

        /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal()
        {
            uint outputCrcValue = ~_crc;

            return BitConverter.GetBytes(outputCrcValue);
        }


        private void HashCoreArm(byte[] array, int ibStart, int cbSize)
        {
#if NET5_0_OR_GREATER
            if (Platform == Platform.Arm64)
            {
                while (cbSize >= 8)
                {
                    _crc = Arm64.ComputeCrc32(_crc, BitConverter.ToUInt64(array, ibStart));
                    ibStart += 8;
                    cbSize -= 8;
                }
            }

            while (cbSize > 0)
            {
                _crc = Arm32.ComputeCrc32(_crc, array[ibStart]);
                ibStart++;
                cbSize--;
            }
#endif
        }

        private static Platform Platform =>
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
