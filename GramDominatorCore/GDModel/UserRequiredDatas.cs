using System;

namespace GramDominatorCore.GDModel
{
    public class UserRequiredDatas
    {
        public string ProfilePictureUrl = string.Empty;
        public string UserName = string.Empty;
        public string UserId = string.Empty;
        public string UserFullName = string.Empty;
        public bool IsFollowedAlready = false;
        public int PostCount = 0;
        public int FollowerCount = 0;
        public int FollowingCount = 0;
        public string Email_Id = string.Empty;
        public string Phone_Number = string.Empty;
        public string EngagementRate = string.Empty;
        public string CommentCount = string.Empty;
        public string LikeCount = string.Empty;
        public string Biography = string.Empty;
        public string BusinessCategory = string.Empty;
        public bool IsBusiness =false;
    }

    public class PostRequiredDatas
    {
        public string totalLikes { get; set; }
        public string totalComments { get; set; }
        public string dataAndTime { get; set; }
        public string location { get; set; }

        public string PostUrl { get; set; }
       // public string engagementRate { get; set; }
    }
}

