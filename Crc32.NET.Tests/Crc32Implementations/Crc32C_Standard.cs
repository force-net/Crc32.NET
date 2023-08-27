namespace Force.Crc32.Tests.Crc32Implementations
{
	public class Crc32C_Standard : CrcCalculator
	{
		public Crc32C_Standard() : base("Crc32C.Standard")
		{
		}

		public override uint Calculate(byte[] data)
		{
			var crc32C = new Ralph.Crc32C.Crc32C();
			crc32C.Update(data, 0, data.Length);
			return crc32C.GetIntValue();
		}
	}
}
