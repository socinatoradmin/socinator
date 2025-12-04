using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendMessageToNewConnectionViewModel
    {
        bool IsCheckedConnectedBefore { get; set; }
        int Days { get; set; }
        int Hours { get; set; }
        bool IsChkSpintaxChecked { get; set; }
        bool IsChkTagChecked { get; set; }
        string Message { get; set; }
        List<string> LstMessage { get; set; }
    }

    public class SendMessageToNewConnectionViewModel : StartupBaseViewModel, ISendMessageToNewConnectionViewModel
    {
        public SendMessageToNewConnectionViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig
                {Model = this, ActivityType = ActivityType.SendMessageToNewConnection});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        public bool IsCheckedConnectedBefore
        {
            get => _IsCheckedConnectedBefore;
            set => SetProperty(ref _IsCheckedConnectedBefore, value);
        }


        public int Days
        {
            get => _Days;
            set => SetProperty(ref _Days, value);
        }

        public int Hours
        {
            get => _Hours;
            set => SetProperty(ref _Hours, value);
        }


        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;
            set => SetProperty(ref _IsChkSpintaxChecked, value);
        }

        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;
            set => SetProperty(ref _IsChkTagChecked, value);
        }

        public string Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }

        public List<string> LstMessage
        {
            get => _LstMessage;
            set => SetProperty(ref _LstMessage, value);
        }

        #region MyRegion

        private bool _IsCheckedConnectedBefore;
        private int _Days;
        private int _Hours;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;
        private string _Message;
        private List<string> _LstMessage;

        #endregion
    }
}