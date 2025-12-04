using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    internal interface IUnLikeModel
    {
        bool IsChkPostLikedBySoftwareChecked { get; set; }
        bool IsChkCustomPostsListChecked { get; set; }
        bool IsChkPostLikedOutsideSoftware { get; set; }
    }

    [ProtoContract]
    public class UnLikeModel : ModuleSetting, IUnLikeModel
    {
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(75, 100),
            DelayBetweenActivity = new RangeUtilities(35, 50)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        //public bool IsChkStartFollowToolStopUnFollow
        //{
        //    get
        //    {
        //        return _isChkStartFollowToolStopUnFollow;
        //    }

        //    set
        //    {

        //        if (value == _isChkStartFollowToolStopUnFollow)
        //            return;
        //        SetProperty(ref _isChkStartFollowToolStopUnFollow, value);
        //    }
        //}

        //public bool IsChkStopUnFollowTool
        //{
        //    get
        //    {
        //        return _isChkStopUnFollowTool;
        //    }

        //    set
        //    {

        //        if (value == _isChkStopUnFollowTool)
        //            return;
        //        SetProperty(ref _isChkStopUnFollowTool, value);
        //    }
        //}
        //private bool _IsChkStopFollowToolWhenReachChecked;
        //public bool IsChkStopFollowToolWhenReachChecked
        //{
        //    get
        //    {
        //        return _IsChkStopFollowToolWhenReachChecked;
        //    }

        //    set
        //    {

        //        if (value == _IsChkStopFollowToolWhenReachChecked)
        //            return;
        //        SetProperty(ref _IsChkStopFollowToolWhenReachChecked, value);
        //    }
        //}

        //public RangeUtilities StopFollowToolWhenReachValue
        //{
        //    get
        //    {
        //        //throw new NotImplementedException();
        //    }

        //    set
        //    {
        //       // throw new NotImplementedException();
        //    }
        //}

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(16, 25),
            ActivitiesPerHour = new RangeUtilities(2, 3),
            ActivitiesPerWeek = new RangeUtilities(100, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(45, 60),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(60, 95),
            DelayBetweenActivity = new RangeUtilities(40, 50)
        };

        [ProtoMember(1)] public new JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();


        [ProtoMember(3)] public override PostFilterModel BlogFilterModel { get; set; } = new PostFilterModel();
        [ProtoMember(27)] public SearchFilterModel SearchFilter { get; set; } = new SearchFilterModel();

        [ProtoMember(23)] public BlacklistSettings Whitelist { get; set; } = new BlacklistSettings();

        [ProtoMember(24)] public int MaxDaysSinceLastPost { get; set; }

        [ProtoMember(25)] public bool IgnoreFollowers { get; set; }

        public bool IsWhoNotFollowBackChecked { get; set; }

        [ProtoMember(26)] public bool IsChkPostLikedOutsideSoftware { get; set; }


        #region IUnfollowerModel

        private bool _isChkPostLikedBySoftwareCheecked;

        [ProtoMember(2)]
        public bool IsChkPostLikedBySoftwareChecked
        {
            get => _isChkPostLikedBySoftwareCheecked;
            set
            {
                if (value == _isChkPostLikedBySoftwareCheecked) return;
                SetProperty(ref _isChkPostLikedBySoftwareCheecked, value);
            }
        }

        private bool _isChkCustomPostsListChecked;

        [ProtoMember(3)]
        public bool IsChkCustomPostsListChecked
        {
            get => _isChkCustomPostsListChecked;
            set
            {
                if (_isChkCustomPostsListChecked == value) return;

                SetProperty(ref _isChkCustomPostsListChecked, value);
            }
        }

        private string _customPostsList;

        [ProtoMember(4)]
        public string CustomPostsList
        {
            get => _customPostsList;
            set
            {
                if (value == _customPostsList)
                    return;
                SetProperty(ref _customPostsList, value);
            }
        }

        //[ProtoMember(27)]
        //public bool FilterInactiveUser;

        //public bool FilterInactiveUsers
        //{
        //    get
        //    {
        //        return FilterInactiveUser;
        //    }
        //    set
        //    {
        //        if (value == FilterInactiveUser)
        //            return;
        //        SetProperty(ref FilterInactiveUser, value);
        //    }
        //}


        private List<string> _lstcustomPosts = new List<string>();

        [ProtoMember(5)]
        public List<string> LstCustomPosts
        {
            get => _lstcustomPosts;
            set
            {
                if (value == _lstcustomPosts)
                    return;
                SetProperty(ref _lstcustomPosts, value);
            }
        }
        //[ProtoMember(29)]
        //public bool IsChkAddToGroupBlackList
        //{
        //    get
        //    {
        //        return _isChkAddToGroupBlackList;
        //    }

        //    set
        //    {

        //        if (value == _isChkAddToGroupBlackList)
        //            return;
        //        SetProperty(ref _isChkAddToGroupBlackList, value);
        //    }
        //}


        //private bool _isChkSkipWhiteListedUser;
        //private bool _isChkPrivateWhiteListed;
        //private bool _isChkToGroupWhitelist;
        //private bool _isChkAddToPrivateBlackList;//IsChkStartFollowToolStopUnFollow
        //private bool _isChkAddToGroupBlackList;

        //private bool _isChkStartFollowToolStopUnFollow;
        //private bool _isChkStopUnFollowTool;


        //public bool IsChkSkipWhiteListedUser
        //{
        //    get
        //    {
        //        return _isChkSkipWhiteListedUser;
        //    }

        //    set
        //    {
        //        if (value == _isChkSkipWhiteListedUser)
        //            return;
        //        SetProperty(ref _isChkSkipWhiteListedUser, value);
        //    }
        //}

        //public bool IsChkPrivateWhiteListed
        //{
        //    get
        //    {
        //        return _isChkPrivateWhiteListed;
        //    }

        //    set
        //    {
        //        if (value == _isChkPrivateWhiteListed)
        //            return;
        //        SetProperty(ref _isChkPrivateWhiteListed, value);
        //    }
        //}

        //public bool IsChkToGroupWhitelist
        //{
        //    get
        //    {
        //        return _isChkToGroupWhitelist;
        //    }

        //    set
        //    {
        //        if (value == _isChkToGroupWhitelist)
        //            return;
        //        SetProperty(ref _isChkToGroupWhitelist, value);
        //    }
        //}

        //public bool IsChkAddToPrivateBlackList
        //{
        //    get
        //    {
        //        return _isChkAddToPrivateBlackList;
        //    }

        //    set
        //    {
        //        if (value == _isChkAddToPrivateBlackList)
        //            return;
        //        SetProperty(ref _isChkAddToPrivateBlackList, value);
        //    }
        //}

        #endregion
    }
}