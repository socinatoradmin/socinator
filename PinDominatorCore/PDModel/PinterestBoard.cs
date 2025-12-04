using DominatorHouseCore.Interfaces;
using System.Collections.Generic;

namespace PinDominatorCore.PDModel
{
    public class PinterestBoard : IPost
    {
        public string BoardName { get; set; }
        public string BoardUrl { get; set; }
        public string BoardDescription { get; set; }
        public int FollowersCount { get; set; }
        public int PinsCount { get; set; }
        public string ContactRequestId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsFollowed { get; set; }
        public string BoardCreatedAt { get; set; }
        public string CreatedAt { get; set; }
        public string BoardOrderModifiedAt { get; set; }
        public string CollaboratedByMe { get; set; }
        public bool? IsCollaborative { get; set; }
        public string FollowedByMe { get; set; }
        public string ImageThumbnailUrl { get; set; }
        public string EmailToCollaborate { get; set; }
        public string FullName { get; set; }
        public string ProfilePicUrl { get; set; }
        public bool HasProfilePicture { get; set; }
        public List<PinterestBoardSections> BoardSections { get; set; } = new List<PinterestBoardSections>();
        public PinterestUser PinterestUserSender { get; set; }

        public PinterestUser PinterestUserRecipient { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
    }
}