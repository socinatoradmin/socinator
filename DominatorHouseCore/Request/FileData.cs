#region

using System.Collections.Specialized;

#endregion

namespace DominatorHouseCore.Request
{
    public class FileData
    {
        public FileData(NameValueCollection headers, string fileName, byte[] contents)
        {
            Headers = headers;
            FileName = fileName;
            Contents = contents;
        }

        public byte[] Contents { get; }

        public string FileName { get; }

        public NameValueCollection Headers { get; }
    }
}