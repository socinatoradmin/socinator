#region

using System;
using System.ComponentModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    /// <summary>
    ///     QueryInfo is used to save the user query value
    /// </summary>
    [ProtoContract]
    public class QueryInfo : BindableBase, ICloneable
    {
        private string _id = Utilities.GetGuid();

        private string _queryValue;
        private bool _isCustomFilterSelected;
        private string _customFilters;
        private int _addedDateTime = DateTimeUtilities.GetEpochTime();
        private int _queryPriority;
        private string _queryTypeDisplayName;

        public static readonly QueryInfo NoQuery = new QueryInfo();
        private int _index;

        [ProtoMember(10)]
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        /// <summary>
        ///     Id is the unique id for the query, which contains guid without dashes
        /// </summary>
        [ProtoMember(1)]
        public string Id
        {
            get => _id;
            set
            {
                if (_id != null && _id == value)
                    return;
                SetProperty(ref _id, value);
            }
        }

        private string _queryType;

        /// <summary>
        ///     QueryType holds the index value of QueryParameters
        /// </summary>
        [ProtoMember(2)]
        public string QueryType
        {
            get => _queryType;
            set
            {
                if (_queryType == value)
                    return;
                SetProperty(ref _queryType, value);
            }
        }

        /// <summary>
        ///     QueryValue holds the input value for selected query type
        /// </summary>
        [ProtoMember(3)]
        public string QueryValue
        {
            get => _queryValue;
            set
            {
                if (_queryValue != null && _queryValue == value)
                    return;
                SetProperty(ref _queryValue, value);
            }
        }


        /// <summary>
        ///     IsCustomFilterSelected holds whether the Query contains custom filters
        /// </summary>
        [ProtoMember(4)]
        public bool IsCustomFilterSelected
        {
            get => _isCustomFilterSelected;
            set
            {
                if (_isCustomFilterSelected == value)
                    return;
                SetProperty(ref _isCustomFilterSelected, value);
            }
        }


        [ProtoMember(5)]
        public string CustomFilters
        {
            get => _customFilters;
            set
            {
                if (_customFilters != null && _customFilters == value)
                    return;
                SetProperty(ref _customFilters, value);
            }
        }


        /// <summary>
        ///     AddedDateTime holds when the query added datetime
        /// </summary>
        [ProtoMember(6)]
        public int AddedDateTime
        {
            get => _addedDateTime;
            set
            {
                if (_addedDateTime == value)
                    return;
                SetProperty(ref _addedDateTime, value);
            }
        }


        /// <summary>
        ///     QueryPriority defines the order which query are going to execute in business logic
        /// </summary>
        [ProtoMember(7)]
        public int QueryPriority
        {
            get => _queryPriority;
            set
            {
                if (_queryPriority == value)
                    return;
                SetProperty(ref _queryPriority, value);
            }
        }

        private bool _isQuerySlected;

        public bool IsQuerySelected
        {
            get => _isQuerySlected;
            set
            {
                if (_isQuerySlected == value)
                    return;
                SetProperty(ref _isQuerySlected, value);
            }
        }


        [ProtoMember(8)]
        public string QueryTypeDisplayName
        {
            get => _queryTypeDisplayName;
            set
            {
                if (_queryTypeDisplayName != null && _queryTypeDisplayName == value)
                    return;

                var displayName = value; // ConvertToDisplayName(value)
                SetProperty(ref _queryTypeDisplayName, displayName);
            }
        }

        private string _queryTypeEnum;

        /// <summary>
        ///     It holds the QueryType Enum Name
        /// </summary>
        [ProtoMember(9)]
        public string QueryTypeEnum
        {
            get => _queryTypeEnum;
            set
            {
                if (_queryTypeEnum == value)
                    return;
                SetProperty(ref _queryTypeEnum, value);
            }
        }

        public object Clone()
        {
            return (QueryInfo) MemberwiseClone();
        }
    }


    public enum UserQueryParameters
    {
        [Description("LangKeyHashtagPostS")] HashtagPost = 1,
        [Description("LangKeyHashtagUsers")] HashtagUsers = 2,
        [Description("LangKeyKeywords")] Keywords = 3,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers = 4,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings = 5,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 6,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowers = 7,
        [Description("LangKeyLocationUsers")] LocationUsers = 8,
        [Description("LangKeyLocationPosts")] LocationPosts = 9,

        [Description("LangKeyCustomUsersList")]
        CustomUsers = 10,
        [Description("LangKeySuggestedUsers")] SuggestedUsers = 11,
        [Description("LangKeyCustomPhotos")] CustomPhotos = 12,

        [Description("LangKeyUsersWhoLikedPosts")]
        UsersWhoLikedPost = 13,

        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost = 14,

        [Description("LangKeyFromSomeonesCircle")]
        FromSomeonesCircle = 15,

        [Description("LangKeyFromCircleOfFollowers")]
        FromCircleOfFollowers = 16,

        [Description("LangKeyFromCircleOfFollowings")]
        FromCircleOfFollowings = 17,
        [Description("LangKeyBoardFollowers")] BoardFollowers = 18,
        [Description("LangKeyCustomBoard")] CustomBoard = 19,
        [Description("LangKeyCustomPin")] CustomPin = 20,
        [Description("LangKeyNewsfeed")] NewsFeedPins = 21,
        [Description("LangKeyCustomurl")] CustomUrl = 22
    }
}