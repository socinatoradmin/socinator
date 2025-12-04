using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class UnLike : BindableBase
    {
        private string _CustomTweets = string.Empty;
        private bool _IsCustomTweets;
        private bool _IsLikedTweets;

        [ProtoMember(1)]
        public bool IsLikedTweets
        {
            get => _IsLikedTweets;
            set => SetProperty(ref _IsLikedTweets, value);
        }

        [ProtoMember(2)]
        public bool IsCustomTweets
        {
            get => _IsCustomTweets;
            set => SetProperty(ref _IsCustomTweets, value);
        }

        [ProtoMember(3)]
        public string CustomTweets
        {
            get => _CustomTweets;
            set => SetProperty(ref _CustomTweets, value);
        }
    }

    public class UnLikeModel : ModuleSetting, IGeneralSettings
    {
        private string _CustomTweets = string.Empty;
        private bool _IsCustomTweets;


        private bool _IsLikedTweets;

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();


        [ProtoMember(2)] public override TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();

        [ProtoMember(4)]
        public bool IsLikedTweets
        {
            get => _IsLikedTweets;
            set => SetProperty(ref _IsLikedTweets, value);
        }

        [ProtoMember(5)]
        public bool IsCustomTweets
        {
            get => _IsCustomTweets;
            set => SetProperty(ref _IsCustomTweets, value);
        }

        [ProtoMember(6)]
        public string CustomTweets
        {
            get => _CustomTweets;
            set => SetProperty(ref _CustomTweets, value);
        }


        [ProtoMember(3)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region Manage Speed 

        /// <summary>
        ///     Slow week 200
        ///     Medium week 400
        ///     Fast week 600
        ///     SuperFast week 800
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(20, 25),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(150, 200),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(40, 60),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(300, 400),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 90),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(95, 120),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(600, 800),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}