using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TWP.Api.Infrastructure.Helpers
{
    public static class CsvMappers
    {
        public static List<T> MapCsvTo<T>(this string csvPath, bool hasHeaderRecord)
            where T : class
        {
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
            }))
            {
                return csv.GetRecords<T>().ToList();
            }
        }
    }
}
