using DominatorHouseCore.Interfaces;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public class TumblrUser : IUser
    {
        public string Pk { get; set; }

        public string PageUrl { get; set; }

        public string TumblrsFormKey { get; set; }
        public string Message { get; set; }

        public bool IsFollowed { get; set; }
        public bool IsYou { get; set; }
        public bool CanFollow { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int PostsCount { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string ProfilePicUrl { get; set; }

        public string UserId { get; set; }
        public string Uuid { get; set; }
        public string UserUuid { get; set; }
        public bool CanMessage { get; set; }
        public string PlacementId { get; set; }
        public bool ShareLikes { get; set; }
        public bool ShareFollowing { get; set; }
        public List<TumblrPost> PostList { get; set; }
    }
}