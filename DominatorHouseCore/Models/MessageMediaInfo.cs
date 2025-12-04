using System;
using System.IO;

namespace DominatorHouseCore.Models
{
    public class MessageMediaInfo
    {
        public string MediaPath {  get; set; }
        public string MediaName {  get; set; }
        public bool IsVideo {  get; set; }
        public string FileSize { get; set; }
        public bool IsImage {  get; set; }
        public long MediaLength {  get; set; }
        public string MediaType { get; set; }
        public string VideoDuration {  get; set; }
        public string FileExtension {  get; set; }
        public byte[] MediaBytes { get; set; }
        public MessageMediaInfo(string FilePath="") {
            if (string.IsNullOrEmpty(FilePath))
                return;
            MediaPath = FilePath;
            UpdateAllDetails(MediaPath);
        }

        private void UpdateAllDetails(string mediaPath)
        {
            try
            {
                var fileInfo = new FileInfo(MediaPath);
                MediaName = fileInfo.Name;
                FileExtension = fileInfo.Extension;
                MediaType = fileInfo.GetType().Name;
                MediaBytes = File.ReadAllBytes(fileInfo.FullName);
                MediaLength = fileInfo.Length;
                FileSize = (fileInfo.Length / 1048576d).ToString() + " MB";
                IsImage = !string.IsNullOrEmpty(FileExtension) && (FileExtension.ToLower() == "jpg" || FileExtension.ToLower() == "jpeg" || FileExtension.ToLower() == "png");
                IsVideo = !string.IsNullOrEmpty(FileExtension) && (FileExtension.ToLower() == "mp4" || FileExtension.ToLower() == "m4v"|| FileExtension.ToLower() == "3gp");
            }catch(Exception) { }
        }
    }
}
