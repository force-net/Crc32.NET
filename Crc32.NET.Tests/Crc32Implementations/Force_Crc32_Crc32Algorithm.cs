namespace Force.Crc32.Tests.Crc32Implementations
{
    class Force_Crc32_Crc32Algorithm : CrcCalculator
    {
        public Force_Crc32_Crc32Algorithm() : base("Force.Crc32.Crc32Algorithm")
        {
        }

        public override uint Calculate(byte[] data)
        {
            return Force.Crc32.Crc32Algorithm.Compute(data);
        }
    }
}
