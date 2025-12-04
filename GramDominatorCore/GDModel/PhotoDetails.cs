using GramDominatorCore.GDUtility;

namespace GramDominatorCore.GDModel
{
    public class PhotoDetails : MediaDetails
    {
        public PhotoDetails(int height, int width, string fileName, long fileSize, ImageHelper.ImageFormats type)
      : base(height, width, fileName, fileSize)
        {
            Type = type;
            ImageResizer.IsValidImage(this);
        }

        public ImageHelper.ImageFormats Type { get; }
    }
}