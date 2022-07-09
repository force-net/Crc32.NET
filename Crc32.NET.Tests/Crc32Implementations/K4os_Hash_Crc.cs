namespace Force.Crc32.Tests.Crc32Implementations
{
	public class K4os_Hash_Crc : CrcCalculator
	{
		public K4os_Hash_Crc() : base("K4os.Hash.Crc")
		{
		}

		public override uint Calculate(byte[] data)
		{
			return K4os.Hash.Crc.Crc32.DigestOf(data, 0, data.Length);
		}
	}
}
