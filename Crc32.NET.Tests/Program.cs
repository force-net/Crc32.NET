namespace Force.Crc32.Tests
{
	public static class Program
	{
		public static void Main()
		{
			var pt = new PerformanceTest();
#if NETFRAMEWORK
			pt.ThroughputCHCrc32_By_tanglebones();
            pt.ThroughputKlinkby_Checksum();
			pt.ThroughputCrc32_By_Data_HashFunction_Crc();
			pt.ThroughputCrc32_By_Me();
			pt.ThroughputCrc32_By_Dexiom();
#endif
			pt.ThroughputCrc32_By_dariogriffo();
			pt.ThroughputCrc32C_By_K4os_Hash_Crc();
			pt.ThroughputCrc32C_Standard();
			pt.ThroughputCrc32C_By_Me();
			pt.ThroughputCrc32_By_Me();
#if NETCOREAPP3_0_OR_GREATER
			pt.ThroughputCrc32C_By_Me_Intrinsics();
#endif
#if NET5_0_OR_GREATER
			pt.ThroughputCrc32_By_Me_Intrinsics();
#endif
        }
    }
}
