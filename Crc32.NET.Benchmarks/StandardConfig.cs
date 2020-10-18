using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;

namespace Crc32.NET.Benchmarks
{
    public class StandardConfig : ManualConfig
    {
        public StandardConfig()
        {
            AddColumnProvider(DefaultColumnProviders.Instance);
            AddColumn(RankColumn.Arabic);

            AddExporter(DefaultExporters.CsvMeasurements);
            AddExporter(DefaultExporters.Csv);
            AddExporter(DefaultExporters.Markdown);
            AddExporter(DefaultExporters.Html);

            AddLogger(ConsoleLogger.Default);
        }
    }
}
