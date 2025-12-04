using DominatorHouseCore;
using DominatorHouseCore.Utility;
using System;
using System.IO;
using System.Linq;

namespace YoutubeDominatorCore.YDUtility
{
    public class UploadingFileInfos
    {
        public UploadingFileInfos(string filePath)
        {
            try
            {
                FilePath = filePath;
                FileNameWithExtension = FilePath.Split('\\').Last();
                FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameWithExtension);
                DirectoryName = Utilities.GetBetween(FilePath, "", "\\" + FileNameWithExtension);
                FileExtension = Path.GetExtension(FileNameWithExtension);
                FileInfo = new FileInfo(DirectoryName.Trim('\\') + "\\" + FileNameWithExtension);
                FileLength = FileInfo.Length;
                FileBytes = File.ReadAllBytes(FilePath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string FileNameWithExtension { get; }
        public string FileNameWithoutExtension { get; }
        public string DirectoryName { get; }
        public string FileExtension { get; }
        public FileInfo FileInfo { get; }
        public long FileLength { get; }
        public string FilePath { get; }
        public byte[] FileBytes { get; set; }
    }
}