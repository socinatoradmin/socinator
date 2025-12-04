using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IEventInviterViewModel
    {
    }

    public class EventInviterViewModel : StartupBaseViewModel, IEventInviterViewModel
    {
        private InviterDetails _inviterDetailsModel = new InviterDetails();

        private InviterOptions _inviterOptionsModel = new InviterOptions();

        public EventInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.EventInviter});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(EventInviterValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyInviteNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyInviteNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyInviteNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyInviteToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyInviteMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public InviterDetails InviterDetailsModel
        {
            get => _inviterDetailsModel;
            set
            {
                if ((_inviterDetailsModel == value) & (_inviterDetailsModel == null))
                    return;
                SetProperty(ref _inviterDetailsModel, value);
            }
        }

        public InviterOptions InviterOptionsModel
        {
            get => _inviterOptionsModel;
            set
            {
                if ((_inviterOptionsModel == value) & (_inviterOptionsModel == null))
                    return;
                SetProperty(ref _inviterOptionsModel, value);
            }
        }

        private void EventInviterValidate()
        {
            if (InviterDetailsModel.ListEventUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please enter event url.");
                return;
            }

            NavigateNext();
        }
    }
}