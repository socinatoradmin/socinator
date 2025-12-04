using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.Response;

namespace GramDominatorCore.GDModel
{
    public class InstagramPost : IPost, IEquatable<InstagramPost> 
    {
        private List<CarouselMedia> album = new List<CarouselMedia>();
        private List<InstaGramImage> images = new List<InstaGramImage>();
        public List<string> RepostMedia { get; set; } = new List<string>();
        private InstaGramImage video;
        private double videoDuration;
        private int viewCount;      
        public List<CarouselMedia> Album
        {
            get
            {
                if (MediaType != MediaType.Album)
                    throw new InstagramException("Can't be used if media type isn't album");
                return album;
            }
            set
            {
                if (MediaType != MediaType.Album)
                    throw new InstagramException("Can't be used if media type isn't album");
                album = value;
            }
        }

        public string Caption { get; set; }
        string IPost.Code
        {
            get { return Code; }
            set { Code = value; }
        }

        

        public string Code { get; set; }

        public int CommentCount { get; set; }

//public int SelfCommentCount { get; set; }

        public bool CommentLikesEnabled { get; set; }

        public bool CommentsDisabled { get; set; }

        public string DeviceTimestamp { get; set; }
        public string CommentText { get; set; }

        public bool HasDetailedLocation { get; set; }

        public bool HasLiked { get; set; }
        public bool IsLocationPost { get; set; } = false;
        public bool HasLocation { get; set; }

        public string Id { get; set; }

        public string Url { get; set; }
        public bool IsCheckedCropMedia {  get; set; }
        public string CropRatio {  get; set; }
        public List<InstaGramImage> Images
        {
            get
            {
                if (MediaType == MediaType.Album)
                    throw new InstagramException("Can't be used if media type is album");
                return images;
            }
            set
            {
                if (MediaType == MediaType.Album)
                    throw new InstagramException("Can't be used if media type is album");
                images = value;
            }
        }

        public bool IsAd { get; set; }

        public int LikeCount { get; set; }

        public Location Location { get; set; }

        public MediaType MediaType { get; set; }

        public bool PhotoOfYou { get; set; }

        public string Pk { get; set; }

        public int TakenAt { get; set; }

        public string ProductType { get; set; }

        public InstagramUser User { get; set; }

        public List<InstagramUser> UserTags = new List<InstagramUser>();

        public InstaGramImage Video
        {
            get
            {
                //if (MediaType != MediaType.Video)
                //    throw new InstagramException("Can be only used if mediaType is a video");
                return video;
            }
            set
            {
                //if (MediaType != MediaType.Video)
                //    throw new InstagramException("Can be only used if mediaType is a video");
                video = value;
            }
        }

        public double VideoDuration
        {
            // ReSharper disable once UnusedMember.Local
            private get
            {
                if (MediaType != MediaType.Video)
                    throw new InstagramException("Can be only used if mediaType is a video");
                return videoDuration;
            }
            set
            {
                if (MediaType != MediaType.Video)
                    throw new InstagramException("Can be only used if mediaType is a video");
                videoDuration = value;
            }
        }

        public int ViewCount
        {
            // ReSharper disable once UnusedMember.Local
            private get
            {
                if (MediaType != MediaType.Video)
                    throw new InstagramException("Can be only used if mediaType is a video");
                return viewCount;
            }
            set
            {
                if (MediaType != MediaType.Video)
                    throw new InstagramException("Can be only used if mediaType is a video");
                viewCount = value;
            }
        }

        public bool Equals(InstagramPost other)
        {
            if (other == null)
                return false;
            if (Equals(other))
                return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((InstagramPost)obj);
        }

        public override int GetHashCode()
        {
            if (Id == null)
                return 0;
            return Id.GetHashCode();
        }
    }
}
