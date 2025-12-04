using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{

    public interface IDeletePostModel
    {
      //  bool IsCheckedDeleteAllPosts { get; set; }
    }


    [ProtoContract]
    public class DeletePostModel : ModuleSetting, IDeletePostModel, IGeneralSettings
    {
        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(84, 126),
            DelayBetweenActivity = new RangeUtilities(25, 50),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion


        #region IDeletePostModel

        private bool _isCheckedDeleteAllPosts = true;
        [ProtoMember(2)]
        public bool IsCheckedDeleteAllPosts
        {
            get
            {
                return _isCheckedDeleteAllPosts;
            }
            set
            {
                if (_isCheckedDeleteAllPosts == value)
                    return;
                SetProperty(ref _isCheckedDeleteAllPosts, value);
            }
        }
        #endregion
    }
}
