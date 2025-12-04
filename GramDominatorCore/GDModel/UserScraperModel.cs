using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class UserScraperModel : ModuleSetting
    {
        private ObservableCollection<UserRequiredData> _listUserRequiredData = new ObservableCollection<UserRequiredData>();
       

        public class UserRequiredData :BindableBase
        {
            public string ItemName { get; set; }

            [ProtoMember(1)]
            private bool _IsSelected;
            public bool IsSelected
            {
                get
                {return _IsSelected;}
                set
                {SetProperty(ref _IsSelected, value);}
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]

        public ObservableCollection<UserRequiredData> ListUserRequiredData
        {
            get { return _listUserRequiredData; }
            set { SetProperty(ref _listUserRequiredData, value); }
        }

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(9, 12),
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
        
        #endregion

    }
}
