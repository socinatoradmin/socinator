using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.Inviter
{

    public class EventInviterViewModel : BindableBase
    {

        public EventInviterViewModel()
        {
            EventInviterModel.InviterDetailsModel = new DominatorHouseCore.Models.FacebookModels.InviterDetails();

            EventInviterModel.InviterOptionsModel.IsInviteWithNoteOptionVisible = false;
            EventInviterModel.InviterOptionsModel.IsInviteWithNoteOptionVisibleEvent = true;

            EventInviterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyInviteNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyInviteToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyInviteMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SaveCommand = new BaseCommand<object>((sender) => true, SaveEventUrl);
        }

        private void SaveEventUrl(object sender)
        {
            if (!string.IsNullOrEmpty(EventInviterModel.InviterDetailsModel.EventUrls))
            {
                EventInviterModel.InviterDetailsModel.ListEventUrl =
               Regex.Split(EventInviterModel.InviterDetailsModel.EventUrls, "\r\n").ToList();
            }
            else
            {
                Dialog.ShowDialog(this, "Error", "There is no data to save.");
            }


        }

        public ICommand SaveCommand { get; set; }

        public EventInviterModel Model => EventInviterModel;

        private EventInviterModel _eventInviterModel = new EventInviterModel();

        public EventInviterModel EventInviterModel
        {
            get
            {
                return _eventInviterModel;
            }
            set
            {
                if (_eventInviterModel == null & _eventInviterModel == value)
                    return;
                SetProperty(ref _eventInviterModel, value);
            }
        }
    }
}
