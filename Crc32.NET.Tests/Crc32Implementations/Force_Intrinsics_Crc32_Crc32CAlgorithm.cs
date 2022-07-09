namespace Force.Crc32.Tests.Crc32Implementations
{
	using Algorithm = Force.Crc32.Intrinsics.Crc32CAlgorithm;
	
	public class Force_Intrinsics_Crc32_Crc32CAlgorithm : CrcCalculator
	{
		public Force_Intrinsics_Crc32_Crc32CAlgorithm()
			: base("Force.Crc32.Intrinsics.Crc32CAlgorithm", Algorithm.IsSupported)
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Algorithm.Compute(data);
		}
	}
}
