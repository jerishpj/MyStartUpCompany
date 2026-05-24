namespace MyStartUpCompany.Worker.Tests.Utilities
{
    /// <summary>
    /// Utility for managing temporary directories in tests.
    /// Automatically cleans up created folders when disposed.
    /// </summary>
    public class TempFolderManager : IDisposable
    {
        private readonly string _rootTempPath;

        public TempFolderManager()
        {
            _rootTempPath = Path.Combine(Path.GetTempPath(), $"worker-tests-{Guid.NewGuid()}");
            Directory.CreateDirectory(_rootTempPath);
        }

        public string RootPath => _rootTempPath;

        public string CreateSubFolder(string folderName)
        {
            var folderPath = Path.Combine(_rootTempPath, folderName);
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public void CreateFile(string folderName, string fileName, string content)
        {
            var folderPath = CreateSubFolder(folderName);
            var filePath = Path.Combine(folderPath, fileName);
            File.WriteAllText(filePath, content);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_rootTempPath))
                {
                    Directory.Delete(_rootTempPath, recursive: true);
                }
            }
            catch
            {
                // Suppress errors during cleanup
            }
        }
    }
}
