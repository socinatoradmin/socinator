namespace Dominator.WebDriver
{
    public interface IFileProvider
    {
        void Create(string path);

        bool Exists(string path);

        void Move(string sourceFileName, string path);
        
        string ReadAllText(string path);
    }
}