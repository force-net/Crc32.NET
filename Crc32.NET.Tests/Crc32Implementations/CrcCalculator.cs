namespace Force.Crc32.Tests.Crc32Implementations
{
	public abstract class CrcCalculator
	{
		protected CrcCalculator(string name, bool isSupported = true)
		{
			Name = name;
			IsSupported = isSupported;
		}

		public string Name { get; private set; }

		public bool IsSupported { get; private set; }

		public abstract uint Calculate(byte[] data);
	}
}
