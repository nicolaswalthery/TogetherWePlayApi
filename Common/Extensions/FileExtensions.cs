namespace Common.Extensions
{
    public static class FileExtensions
    {
        public static string GetJsonFile(this string folderName, string fileName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $".../{folderName}/{fileName}.json");

                string jsonContent = File.ReadAllText(filePath);

                return jsonContent is not null ? jsonContent : String.Empty;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "";
            }
        }
    }
}
