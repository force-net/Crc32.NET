#if NETFRAMEWORK
namespace Force.Crc32.Tests.Crc32Implementations
{
	public class Dexiom_Quick_Crc32 : CrcCalculator
	{
		public Dexiom_Quick_Crc32()
			: base("Dexiom.QuickCrc32")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Dexiom.QuickCrc32.QuickCrc32.Compute(data);
		}
	}
}
#endif
