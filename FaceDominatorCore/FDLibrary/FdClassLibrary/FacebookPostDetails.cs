using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDEnums;
using System;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FacebookPostDetails : IPost, IEntity
    {
        public MediaType MediaType { get; set; }

        public DateTime PostedDateTime { get; set; } = new DateTime();

        public string Title { get; set; } = string.Empty;

        public string MediaUrl { get; set; } = string.Empty;

        public string OtherMediaUrl { get; set; } = string.Empty;

        public string LikersCount { get; set; } = string.Empty;

        public string SharerCount { get; set; } = string.Empty;

        public string CommentorCount { get; set; } = string.Empty;

        public string PostedBy { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string PostUrl { get; set; } = string.Empty;

        public string SubDescription { get; set; } = string.Empty;

        public string Caption { get; set; } = string.Empty;

        public string NavigationUrl { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string QueryType { get; set; } = string.Empty;

        public string QueryValue { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string OwnerLogoUrl { get; set; } = string.Empty;

        public string LikePostAsPageId { get; set; } = string.Empty;

        public string AlbumName { get; set; } = string.Empty;

        public string DownLoadPath { get; set; } = string.Empty;

        //        public string AdId { get;  set; } = string.Empty;

        public FbEntityTypes EntityType { get; set; } = FbEntityTypes.Friend;

        public string EntityId { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public AdMediaType AdMediaType { get; set; }

        public string FullPostUrl { get; set; } = string.Empty;

        public string ScapedUrl { get; set; } = string.Empty;

        public int MaxCommentsOnEachPost { get; set; }

        public int actorChangeNumber { get; set; }

        public bool IsPendingPost { get; set; }

        public bool IsLastPost { get; set; }

        public bool IsActorChangeable { get; set; }
        public bool CanComment { get; set; }
    }

}
