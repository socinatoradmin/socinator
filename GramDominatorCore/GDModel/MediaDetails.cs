namespace GramDominatorCore.GDModel
{
    public abstract class MediaDetails
    {
        protected MediaDetails(int height, int width, string fileName, long fileSize)
        {
            Height = height;
            Width = width;
            FileName = fileName;
            FileSize = fileSize;
        }

        public string FileName { get; }

        public long FileSize { get; }

        public int Height { get; }

        public int Width { get; }
    }
}