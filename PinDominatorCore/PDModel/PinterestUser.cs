using System.Collections.Generic;
using PinDominatorCore.PDEnums;

namespace PinDominatorCore.PDModel
{
    public class PinterestUser : BaseUser
    {
        public PinterestUser()
        {
        }

        public PinterestUser(string pk, string username)
            : base(pk, username)
        {
        }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string ImageSmallUrl { get; set; } = string.Empty;

        public string ImageMediumUrl { get; set; } = string.Empty;

        public string ImageLargeUrl { get; set; } = string.Empty;

        public string ImageXlargeUrl { get; set; } = string.Empty;

        public int FollowersCount { get; set; }

        public int FollowingsCount { get; set; }

        public int BoardsCount { get; set; }

        public int PinsCount { get; set; }

        public int TriesCount { get; set; }

        public List<PinterestPin> PinDetails { get; set; } = new List<PinterestPin>();

        public string UserBio { get; set; }

        public string WebsiteUrl { get; set; }

        public PinterestGender Gender { get; set; }

        public int FollowedBack { get; set; }

        public bool HasProfilePic { get; set; }

        public bool IsVerified { get; set; }

        public bool IsFollowedByMe { get; set; }

        public PinterestUser PinterestUserSender { get; set; }

        public PinterestUser PinterestUserRecipient { get; set; }

        public bool? Read { get; set; }

        public string Type { get; set; }

        public string ContactRequestId { get; set; }

        public string CreatedAt { get; set; }

        public string Conversation { get; set; }
    }
}