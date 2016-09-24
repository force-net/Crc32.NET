namespace Force.Crc32.Tests.Crc32Implementations
{
	public abstract class CrcCalculator
	{
		protected CrcCalculator(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public abstract uint Calculate(byte[] data);
	}
}
