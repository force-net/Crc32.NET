using System;
using System.Data.HashFunction;

namespace Force.Crc32.Tests.Crc32Implementations
{
    class System_Data_HashFunction_CRC : CrcCalculator
    {
        public System_Data_HashFunction_CRC() : base("System.Data.HashFunction.CRC")
        {
            _crc = new CRC();
        }

        public override uint Calculate(byte[] data)
        {
            return BitConverter.ToUInt32(_crc.ComputeHash(data), 0);
        }

        private CRC _crc;
    }
}
