using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ICancelSentRequestViewModel
    {
    }

    public class CancelSentRequestViewModel : StartupBaseViewModel, ICancelSentRequestViewModel
    {
        public CancelSentRequestViewModel(IRegionManager region) : base(region)
        {
            UnfriendOptionModel = new UnfriendOption
            {
                SourceDisplayName = Application.Current.FindResource("LangKeyCancelSentRequestSource")?.ToString(),
                BySoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedBySoftware")?.ToString(),
                OutsideSoftwareDisplayName =
                    Application.Current.FindResource("LangKeyPeopleAddedOutsideSoftware")?.ToString()
            };

            IsNonQuery = true;

            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.WithdrawSentRequest});

            NextCommand = new DelegateCommand(ValidateCancelSentRequest);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCancelNumberOfRequestPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCancelNumberOfRequestPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCancelNumberOfRequestPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCancelNumberOfMaximumRequestPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyCancelNumberOfRequestPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public UnfriendOption UnfriendOptionModel { get; set; } = new UnfriendOption();


        public void ValidateCancelSentRequest()
        {
            if (!UnfriendOptionModel.IsAddedThroughSoftware && !UnfriendOptionModel.IsAddedOutsideSoftware)
            {
                Dialog.ShowDialog("Error", "Please select atleast one source.");
                return;
            }

            if (UnfriendOptionModel.IsFilterApplied && UnfriendOptionModel.DaysBefore == 0 &&
                UnfriendOptionModel.HoursBefore == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid source filter.");
                return;
            }

            NavigateNext();
        }
    }
}