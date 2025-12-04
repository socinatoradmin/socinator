namespace TumblrDominatorCore.Models
{
    public interface IUploadMediaModel
    {
        string url { get; set; }
        int height { get; set; }
        int width { get; set; }
        string csrf { get; set; }
        string type { get; set; }

    }
    public class UploadedMediaModel : IUploadMediaModel
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string csrf { get; set; }
        public string type { get; set; }

    }
}
