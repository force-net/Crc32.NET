namespace Force.Crc32.Tests
{
	public static class Program
	{
		public static void Main()
		{
			var pt = new PerformanceTest();
			pt.ThroughputCrc32_By_dariogriffo();
			pt.ThroughputCHCrc32_By_tanglebones();
            pt.ThroughputKlinkby_Checksum();
			pt.ThroughputCrc32_By_Data_HashFunction_Crc();
			pt.ThroughputCrc32_By_Me();
        }
    }
}
