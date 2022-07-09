namespace Force.Crc32.Tests.Crc32Implementations
{
	using Algorithm = Force.Crc32.Intrinsics.Crc32Algorithm;

	public class Force_Intrinsics_Crc32_Crc32Algorithm : CrcCalculator
	{
		public Force_Intrinsics_Crc32_Crc32Algorithm()
			: base("Force.Crc32.Intrinsics.Crc32Algorithm", Algorithm.IsSupported)
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Algorithm.Compute(data);
		}
	}
}
