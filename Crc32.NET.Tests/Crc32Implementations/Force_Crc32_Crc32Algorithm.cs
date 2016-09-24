namespace Force.Crc32.Tests.Crc32Implementations
{
	public class Force_Crc32_Crc32Algorithm : CrcCalculator
	{
		public Force_Crc32_Crc32Algorithm() : base("Force.Crc32.Crc32Algorithm")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return Crc32Algorithm.Compute(data);
		}
	}
}
