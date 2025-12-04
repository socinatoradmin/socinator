using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.GroupsModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.Groups
{
    public class MakeAdminViewModel : BindableBase
    {

        public MakeAdminViewModel()
        {
            MakeAdminModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyMakeAdminToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyMakeAdminToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyMakeAdminToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyMakeAdminToMaxProfilesPerDay")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMakeAdminToNumberOfProfilesPerWeek")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public MakeAdminModel Model => MakeAdminModel;

        private MakeAdminModel _makeAdminModel = new MakeAdminModel();

        public MakeAdminModel MakeAdminModel
        {
            get
            {
                return _makeAdminModel;
            }
            set
            {
                if (_makeAdminModel == null & _makeAdminModel == value)
                    return;
                SetProperty(ref _makeAdminModel, value);
            }
        }
    }
}

