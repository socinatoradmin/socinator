namespace Dominator.WebDriver
{
    public interface IDirectoryProvider
    {
        void CreateDirectory(string folder);

        void DeleteDirectory(string folder);

        void Move(string sourceDirName, string destDirName);
        
        string[] GetDirectories(string path);

        string[] GetFiles(string path, string searchPattern);
    }
}