#if NETFRAMEWORK

namespace Force.Crc32.Tests.Crc32Implementations
{
	public class CH_Crc32_Crc : CrcCalculator
	{
		public CH_Crc32_Crc() : base("CH.Crc32.Crc")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return CH.Crc32.Crc.Crc32(data);
		}
	}
}
#endif
