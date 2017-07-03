namespace Force.Crc32.Tests.Crc32Implementations
{
	public class Force_Crc32_Crc32CAlgorithm : CrcCalculator
	{
		public Force_Crc32_Crc32CAlgorithm() : base("Force.Crc32.Crc32CAlgorithm")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Crc32CAlgorithm.Compute(data);
		}
	}
}
