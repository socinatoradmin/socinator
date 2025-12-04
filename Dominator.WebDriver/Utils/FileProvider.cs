using System.IO;

namespace Dominator.WebDriver.Utils
{
    public class FileProvider : IFileProvider
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Create(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        public void Move(string sourceFileName, string path)
        {
            File.Move(sourceFileName, path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}