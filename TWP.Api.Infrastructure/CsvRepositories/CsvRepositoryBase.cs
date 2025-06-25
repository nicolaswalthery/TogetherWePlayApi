using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TWP.Api.Infrastructure.CsvRepositories
{
    public class CsvRepositoryBase
    {
        private string _baseRelativePath = @"TWP.Api.Infrastructure\CsvFiles";
        private string _parentFolderName;
        private string _childFolderName;
        private readonly string _fileName;

        public CsvRepositoryBase()
        {
        }

        public CsvRepositoryBase(string parentFolderName, string childFolderName, string fileName)
        {
            _parentFolderName = parentFolderName;
            _childFolderName = childFolderName;
            _fileName = fileName;
        }

        public CsvRepositoryBase(string parentFolderName, string childFolderName)
        {
            _parentFolderName = parentFolderName;
            _childFolderName = childFolderName;
        }

        public CsvRepositoryBase(string childFolderName, string fileName)
        {
            _childFolderName = childFolderName;
            _fileName = fileName;
        }

        public CsvRepositoryBase(string childFolderName)
        {
            _childFolderName = childFolderName;
        }

        /// <summary>
        /// Get the TWP.Api.Infrastructure\CsvFiles base folder path
        /// </summary>
        protected string BaseFolderRelativePath => _baseRelativePath;
        protected string FullRelativePath => string.IsNullOrEmpty(_parentFolderName)
            ? $@"{_baseRelativePath}\{_childFolderName}"
            : $@"{_baseRelativePath}\{_parentFolderName}\{_childFolderName}";

        /// <summary>
        /// Retrieves the contents of the CSV file as a string.
        /// </summary>
        /// <returns>CSV file contents as a string.</returns>
        protected string GetCsvByFileName()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", FullRelativePath, _fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"CSV file not found: {path}");
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Retrieves all CSV file names from a specified folder under the base CSV path.
        /// </summary>
        /// <returns>A list of CSV file names without their full path.</returns>
        protected List<string> GetAllCsvFileNamesInBaseFolder()
        {
            var results = new List<string>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", FullRelativePath);
            if (!Directory.Exists(path))
                return results;

            results = Directory.GetFiles(path, "*.csv")
                            .Select(Path.GetFileName)
                            .ToList();

            return results;
        }
    }
} 