using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public interface ICommentScraperModel
    {
        RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; }
    }

    [ProtoContract]
    public class CommentScraperModel : ModuleSetting, ICommentScraperModel, IGeneralSettings
    {
        private bool _isAddedToCampaign;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(500, 750),
            ActivitiesPerHour = new RangeUtilities(50, 75),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(60, 90),
            DelayBetweenJobs = new RangeUtilities(52, 70),
            DelayBetweenActivity = new RangeUtilities(25, 35)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(250, 400),
            ActivitiesPerHour = new RangeUtilities(25, 45),
            ActivitiesPerWeek = new RangeUtilities(1000, 1200),
            ActivitiesPerJob = new RangeUtilities(40, 60),
            DelayBetweenJobs = new RangeUtilities(48, 66),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(150, 200),
            ActivitiesPerHour = new RangeUtilities(16, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 800),
            ActivitiesPerJob = new RangeUtilities(20, 31),
            DelayBetweenJobs = new RangeUtilities(60, 72),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 90),
            ActivitiesPerWeek = new RangeUtilities(3000, 5000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(45, 70),
            DelayBetweenActivity = new RangeUtilities(35, 60)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(11)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }
        }

        [ProtoMember(12)] public RangeUtilities CommentToBeLikeAfterEachLikedPost { get; set; } = new RangeUtilities();
        public SearchFilterModel SearchFilter { get; set; } = new SearchFilterModel();

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        public class QueryTypeWithTitle
        {
            public QueryTypeWithTitle(string queryType)
            {
                QueryType = queryType;
            }

            public string QueryType { get; set; }

            public string QueryDisplayName()
            {
                var value = (UserQueryParameters)Enum.Parse(typeof(UserQueryParameters), QueryType);

                var description = value.GetDescriptionAttr().FromResourceDictionary();
                return description;
            }

            public override string ToString()
            {
                return QueryDisplayName();
            }
        }
    }
}