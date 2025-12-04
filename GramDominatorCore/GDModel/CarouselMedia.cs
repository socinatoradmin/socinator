using System.Collections.Generic;
using GramDominatorCore.Response;

namespace GramDominatorCore.GDModel
{
    public class CarouselMedia
    {
        private InstaGramImage video;
        private double videoDuration;

        public int Height { get; set; }

        public string Id { get; set; }

        public List<InstaGramImage> Images { get; } = new List<InstaGramImage>();

        public int MediaType { get; set; }

        public InstaGramImage Video
        {
            get
            {
                //if (MediaType != 2)
                //    throw new InstagramException("Can be only used if mediaType is a video");
                return video;
            }
            set
            {
                //if (MediaType != 2)
                //    throw new InstagramException("Can be only used if mediaType is a video");
                video = value;
            }
        }

        public double VideoDuration
        {
            get
            {
                if (MediaType != 2)
                    throw new InstagramException("Can be only used if mediaType is a video");
                return videoDuration;
            }
            set
            {
                if (MediaType != 2)
                    throw new InstagramException("Can be only used if mediaType is a video");
                videoDuration = value;
            }
        }

        public int Width { get; set; }
    }
}
