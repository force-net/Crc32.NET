namespace Force.Crc32.Tests.Crc32Implementations
{
    class Klinkby_Checkum_Crc32 : CrcCalculator
    {
        public Klinkby_Checkum_Crc32() : base("Klinkby.Checkum.Crc32")
        {
        }

        public override uint Calculate(byte[] data)
        {
            return (uint)Klinkby.Checkum.Crc32.ComputeChecksum(data);
        }
    }
}
