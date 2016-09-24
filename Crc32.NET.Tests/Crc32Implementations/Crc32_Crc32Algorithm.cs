using Crc = Crc32.Crc32Algorithm;

namespace Force.Crc32.Tests.Crc32Implementations
{
    class Crc32_Crc32Algorithm : CrcCalculator
    {
        public Crc32_Crc32Algorithm() : base("Crc32.Crc32Algorithm")
        {
        }

        public override uint Calculate(byte[] data)
        {
            return Crc.Compute(data);
        }
    }
}
