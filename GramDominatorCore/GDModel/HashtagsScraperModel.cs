using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using ProtoBuf;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class HashtagsScraperModel : ModuleSetting,  IGeneralSettings
    {
        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();





        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(9, 12)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(3333, 5000),
            ActivitiesPerHour = new RangeUtilities(333, 500),
            ActivitiesPerWeek = new RangeUtilities(20000, 30000),
            ActivitiesPerJob = new RangeUtilities(416, 625),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(6, 9)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(6666, 10000),
            ActivitiesPerHour = new RangeUtilities(666, 1000),
            ActivitiesPerWeek = new RangeUtilities(40000, 60000),
            ActivitiesPerJob = new RangeUtilities(833, 1250),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(3, 6)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13333, 20000),
            ActivitiesPerHour = new RangeUtilities(1200, 3500),
            ActivitiesPerWeek = new RangeUtilities(80000, 120000),
            ActivitiesPerJob = new RangeUtilities(600, 1200),
            DelayBetweenJobs = new RangeUtilities(10, 20),
            DelayBetweenActivity = new RangeUtilities(0, 3)
        };
        
        #endregion


        private string _keyword;
        [ProtoMember(4)]
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                if ( _keyword == value)
                    return;
                SetProperty(ref _keyword, value);
            }
        }
        
        private List<string> _lstKeyword = new List<string>();
        [ProtoMember(5)]
        public List<string> LstKeyword
        {
            get
            {
                return _lstKeyword;
            }
            set
            {
                if (value == _lstKeyword)
                    return;
                SetProperty(ref _lstKeyword, value);
            }
        }      
    }
}
