using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    //public class WelcomeTweetModel
    // {
    // }

    [ProtoContract]
    public class WelcomeTweetModel : ModuleSetting, IGeneralSettings
    {
        private string _WelcomeMessageText = string.Empty;
        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(5)]
        public string WelcomeMessageText
        {
            get => _WelcomeMessageText;
            set => SetProperty(ref _WelcomeMessageText, value);
        }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region Manage Speed 

        /// <summary>
        ///     Slow week 150
        ///     Medium week 300
        ///     Fast week 450
        ///     SuperFast week 600
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(15, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(120, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 45),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(250, 300),
            ActivitiesPerJob = new RangeUtilities(3, 4),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 450),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(80, 90),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}