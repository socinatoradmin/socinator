using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IPageInviterViewModel
    {
        bool IsSelctDetails { get; set; }
        InviterDetails InviterDetailsModel { get; set; }
        InviterOptions InviterOptionsModel { get; set; }
        SelectAccountDetailsModel SelectAccountDetailsModel { get; set; }
    }

    public class PageInviterViewModel : StartupBaseViewModel, IPageInviterViewModel
    {
        private InviterDetails _inviterDetails = new InviterDetails();

        private InviterOptions _inviterOptionsModel = new InviterOptions();

        private bool _isSelctDetails;

        private SelectAccountDetailsModel _selectAccountDetailsModel = new SelectAccountDetailsModel();

        public PageInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.PageInviter});

            IsNonQuery = true;
            NextCommand = new DelegateCommand(PageInviterValidate);
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

        public InviterDetails InviterDetailsModel
        {
            get => _inviterDetails;
            set
            {
                if ((_inviterDetails == value) & (_inviterDetails == null))
                    return;
                SetProperty(ref _inviterDetails, value);
            }
        }

        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get => _selectAccountDetailsModel;
            set
            {
                if ((_selectAccountDetailsModel == value) & (_selectAccountDetailsModel == null))
                    return;
                SetProperty(ref _selectAccountDetailsModel, value);
            }
        }

        public bool IsSelctDetails
        {
            get => _isSelctDetails;
            set
            {
                if (_isSelctDetails == value)
                    return;
                SetProperty(ref _isSelctDetails, value);
            }
        }

        private void PageInviterValidate()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (InviterDetailsModel.IsProfileUrl && selectAccountDetailsControl.GetPageInviterDetails
                    (SelectAccountDetailsModel).PageInviterDetails.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one inviter details.");
                return;
            }

            if (!InviterDetailsModel.IsPostUrl && !InviterDetailsModel.IsRandomPosts
                                               && !InviterDetailsModel.IsSpecificPosts)
            {
                Dialog.ShowDialog("Error", "Please select atleast one post option.");
                return;
            }

            if (InviterDetailsModel.IsSpecificPosts && InviterDetailsModel.ListPostUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one Specific post.");
                return;
            }

            NavigateNext();
        }
    }
}