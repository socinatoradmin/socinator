#region

using System.IO;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IFileSystemProvider
    {
        byte[] ReadAllBytes(string path);
        bool Exists(string path);
        Stream Create(string file);
        Stream Open(string file);
        Stream OpenReadonly(string file);
    }

    public class FileSystemProvider : IFileSystemProvider
    {
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public Stream Create(string file)
        {
            return File.Create(file);
        }

        public Stream Open(string file)
        {
            return File.Open(file, FileMode.Open);
        }

        public Stream OpenReadonly(string file)
        {
            return File.Open(file, FileMode.Open, FileAccess.Read);
        }
    }
}