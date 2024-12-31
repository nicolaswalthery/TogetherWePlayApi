using static System.Net.Mime.MediaTypeNames;

namespace Common.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        /// Get a json file.
        /// </summary>
        /// <param name="fileName">Json File Name</param>
        /// <param name="relativePath">Relative path to the folder where the json file is located./></param>
        /// <returns></returns>
        public static string GetJsonFile(this string fileName, string relativePath)
        {
            try
            {
                // Combine the project root and the relative path to form the absolute file path
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\", relativePath, $"{fileName}.json");

                // Resolve the full path
                filePath = Path.GetFullPath(filePath);

                // Ensure the file exists before trying to read it
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{fileName}.json' was not found in the path '{filePath}'.");
                }

                // Read and return the content of the file
                string jsonContent = File.ReadAllText(filePath);
                return jsonContent;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
