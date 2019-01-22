/* This is .NET safe implementation of Crc32 algorithm.
 * This implementation was investigated as fastest from different variants. It based on Robert Vazan native implementations of Crc32C
 * Also, it is good for x64 and for x86, so, it seems, there is no sense to do 2 different realizations.
 * 
 * Addition: some speed increase was found with splitting xor to 4 independent blocks. Also, some attempts to optimize unaligned tails was unsuccessfull (JIT limitations?).
 * 
 * 
 * Max Vysokikh, 2016-2017
 */

using System.IO;

namespace Force.Crc32
{
	internal class SafeProxy
	{
		private const uint Poly = 0xedb88320u;

		private readonly uint[] _table;

		internal SafeProxy()
		{
            _table = CreateTable(Poly);
		}

        internal SafeProxy(uint poly)
        {
            _table = CreateTable(poly);
        }

        protected uint[] CreateTable(uint poly)
        {
            var table = new uint[16 * 256];
            for (uint i = 0; i < 256; i++)
            {
                uint res = i;
                for (int t = 0; t < 16; t++)
                {
                    for (int k = 0; k < 8; k++) res = (res & 1) == 1 ? poly ^ (res >> 1) : (res >> 1);
                    table[(t * 256) + i] = res;
                }
            }

            return table;
        }

        public uint Append(uint crc, Stream input)
		{
			var crcLocal = uint.MaxValue ^ crc;
            var data = new byte[16];
            int readLength;

            input.Seek(0, SeekOrigin.Begin);

            while ((readLength = input.Read(data, 0, 16)) == 16)
            {
				var a = _table[(3 * 256) + data[12]]
					^ _table[(2 * 256) + data[13]]
					^ _table[(1 * 256) + data[14]]
					^ _table[(0 * 256) + data[15]];

				var b = _table[(7 * 256) + data[8]]
					^ _table[(6 * 256) + data[9]]
					^ _table[(5 * 256) + data[10]]
					^ _table[(4 * 256) + data[11]];

				var c = _table[(11 * 256) + data[4]] 
					^ _table[(10 * 256) + data[5]] 
					^ _table[(9 * 256) + data[6]] 
					^ _table[(8 * 256) + data[7]];

				var d = _table[(15 * 256) + ((byte)crcLocal ^ data[0])]
					^ _table[(14 * 256) + ((byte)(crcLocal >> 8) ^ data[1])]
					^ _table[(13 * 256) + ((byte)(crcLocal >> 16) ^ data[2])]
					^ _table[(12 * 256) + ((crcLocal >> 24) ^ data[3])];

				crcLocal = d ^ c ^ b ^ a;
			}
            
            for (var i = 0; i < readLength; i++)
                crcLocal = _table[(byte)(crcLocal ^ data[i])] ^ crcLocal >> 8;

			return crcLocal ^ uint.MaxValue;
		}
	}
}
