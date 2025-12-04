#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum GroupJoinerParameter
    {
        [Description("LangKeyKeywords")] Keywords = 1,

        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 2,

        [Description("LangKeyCustomGroupUrl")] CustomGroupUrl = 3
    }

    public enum GroupScraperParameter
    {
        [Description("LangKeyKeywords")] Keywords = 1,

        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 2

        /*
                [Description("LangKeyCustomGroupUrl")]
                CustomGroupUrl = 3
        */
    }

    public enum FanpageLikerQueryParameters
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 2,
        [Description("LangKeyCustomPageUrlS")] CustomPageList = 3,

        [Description("LangKeyPagesLikedByFriends")]
        PagesLikedByFriends = 4
    }

    public enum FdProfileQueryParameters
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyLocation")] Location = 2,
        [Description("LangKeyGroupMembers")] GroupMembers = 3,
        [Description("LangKeyFanpageLikers")] FanpageLikers = 4,
        [Description("LangKeyFriendOfFriend")] FriendofFriend = 5,
        [Description("LangKeyPostLikers")] PostLikers = 6,
        [Description("LangKeyPostSharer")] PostSharer = 7,
        [Description("LangKeyPostCommentors")] PostCommentor = 8,
        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 9,
        [Description("LangKeyPagePostLikers")] PagePostLikers = 10,

        [Description("LangKeyGroupPostLikers")]
        GroupPostLikers = 11,

        [Description("LangKeyCustomProfileUrl")]
        CustomProfileUrl = 12,

        [Description("LangKeySuggestedFriends")]
        SuggestedFriends = 13,
        /*[Description("LangKeyEventUrl")] EventUrl = 14,*/
        [Description("LangKeyUserFollower")] UserFollowers = 15,
        [Description("LangKeyOwnFriends")] OwnFriends = 16,

        [Description("LangKeyFriendsBasicDetails")]
        FriendsBasicDetails = 17,

        [Description("LangKeyGroupMemberBasicDetails")]
        GroupMemberBasicDetails = 18,

        [Description("LangKeyPagePostCommenters")]
        PagePostPostCommenters = 19,

        [Description("LangKeyGroupPostCommenters")]
        GroupPostCommenters = 20,

        [Description("LangKeyPeopleConnectedInMessenger")]
        ConnectedPeopleInMessenger = 21
    }

    public enum WebCommentLikerParameter
    {
        [Description("LangKeywebpagecomment")] Webpagecomment = 1,

        [Description("LangKeywebpagereplycomments")]
        Webpagereplycomments = 2
    }

    public enum CommentScraperParameter
    {
        [Description("LangKeyPostUrl")] PostUrl = 1,

        [Description("LangKeyPagePostComments")]
        PagePostComments = 2,

        [Description("LangKeyGroupPostComments")]
        GroupPostComments = 3,
        [Description("LangKeyNewsFeedPosts")] NewsFeedPosts = 4
    }

    public enum CommentLikerParameter
    {
        [Description("LangKeyPostUrl")] PostUrl = 1,

        [Description("LangKeyPagePostComments")]
        PagePostComments = 2,

        [Description("LangKeyGroupPostComments")]
        GroupPostComments = 3,
        [Description("LangKeyNewsFeedPosts")] NewsFeedPosts = 4
    }

    public enum MarketPlaceQueryParameter
    {
        [Description("LangKeyKeywords")] Keywords = 1
    }

    public enum FdUserQueryParameters
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyLocation")] Location = 2,
        [Description("LangKeyGroupMembers")] GroupMembers = 3,
        [Description("LangKeyFanpageLikers")] FanpageLikers = 4,
        [Description("LangKeyFriendOfFriend")] FriendofFriend = 5,
        [Description("LangKeyPostLikers")] PostLikers = 6,
        [Description("LangKeyPostSharer")] PostSharer = 7,
        [Description("LangKeyPostCommentors")] PostCommentor = 8,
        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 9,
        [Description("LangKeyPagePostLikers")] PagePostLikers = 10,

        [Description("LangKeyGroupPostLikers")]
        GroupPostLikers = 11,

        [Description("LangKeyPagePostCommenters")]
        PagePostPostCommenters = 12,

        [Description("LangKeyGroupPostCommenters")]
        GroupPostCommenters = 13,

        [Description("LangKeyCustomProfileUrl")]
        CustomProfileUrl = 14,

        [Description("LangKeySuggestedFriends")]
        SuggestedFriends = 15,
        //[Description("LangKeyEventUrl")] EventUrl = 16,
        [Description("LangKeyUserFollower")] UserFollowers = 17
    }
}