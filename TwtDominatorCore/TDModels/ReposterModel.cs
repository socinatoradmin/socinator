using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class ReposterModel : ModuleSetting, IGeneralSettings
    {
        private string _downloadedFolderPath = ConstantVariable.GetDownloadedMediaFolderPath + @"\Socinator\Twitter";
        private bool _isBookmarkTweet = false;

        [ProtoMember(4)]
        public string DownloadFolderPath
        {
            get => _downloadedFolderPath;
            set
            {
                if (_downloadedFolderPath != null && _downloadedFolderPath == value)
                    return;
                SetProperty(ref _downloadedFolderPath, value);
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();


        [ProtoMember(2)] public override TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();


        [ProtoMember(3)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(5)]
        public bool IsBookmarkTweet
        {
            get => _isBookmarkTweet;
            set
            {
                if (_isBookmarkTweet == value)
                    return;
                SetProperty(ref _isBookmarkTweet, value);
            }
        }

        private bool _isChkQuoteTweet;

        [ProtoMember(6)]
        public bool IsChkQuoteTweet
        {
            get => _isChkQuoteTweet;
            set
            {
                if (value == _isChkQuoteTweet)
                    return;
                SetProperty(ref _isChkQuoteTweet, value);
            }
        }
        private string _UploadQuotesTweets;

        [ProtoMember(7)]
        public string UploadQuotesTweets
        {
            get => _UploadQuotesTweets;
            set
            {
                if (value == _UploadQuotesTweets)
                    return;
                SetProperty(ref _UploadQuotesTweets, value);
            }
        }
        private bool _IsSpintax;
        [ProtoMember(8)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }
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