namespace Force.Crc32.Tests
{
	public static class Program
	{
		public static void Main()
		{
			var pt = new PerformanceTest();
			pt.ThroughputCrc32_By_dariogriffo();
			pt.ThroughputCrc32_By_Me();
		}
	}
}
