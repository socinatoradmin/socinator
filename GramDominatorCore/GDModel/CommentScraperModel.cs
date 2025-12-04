using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class CommentScraperModel: ModuleSetting
    {
        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(9,12),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(3333, 5000),
            ActivitiesPerHour = new RangeUtilities(333, 500),
            ActivitiesPerWeek = new RangeUtilities(20000, 30000),
            ActivitiesPerJob = new RangeUtilities(416, 625),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(6, 9),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(6666, 10000),
            ActivitiesPerHour = new RangeUtilities(666, 1000),
            ActivitiesPerWeek = new RangeUtilities(40000, 60000),
            ActivitiesPerJob = new RangeUtilities(833, 1250),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(3, 6),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13333, 20000),
            ActivitiesPerHour = new RangeUtilities(1200, 3500),
            ActivitiesPerWeek = new RangeUtilities(80000, 120000),
            ActivitiesPerJob = new RangeUtilities(600, 1200),
            DelayBetweenJobs = new RangeUtilities(10, 20),
            DelayBetweenActivity = new RangeUtilities(0, 3),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(3)]
        private string _downloadedFolderPath = ConstantVariable.GetDownloadedMediaFolderPath;
        public string DownloadedFolderPath
        {
            get
            {
                return _downloadedFolderPath;
            }
            set
            {
                if (value == _downloadedFolderPath)
                    return;
                SetProperty(ref _downloadedFolderPath, value);
            }
        }
        #endregion
    }
}
