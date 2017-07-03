#if !NETCORE
namespace Force.Crc32.Tests.Crc32Implementations
{
	public class Crc32C_Crc32CAlgorithm : CrcCalculator
	{
		public Crc32C_Crc32CAlgorithm() : base("Crc32C.Crc32CAlgorithm")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Crc32C.Crc32CAlgorithm.Compute(data);
		}
	}
}
#endif