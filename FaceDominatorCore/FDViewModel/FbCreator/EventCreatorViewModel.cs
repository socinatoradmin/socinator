using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.FbEvents;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.FbCreator
{
    public class EventCreatorViewModel : BindableBase
    {
        public ICommand TypeSelectionChangedCommand { get; set; }

        public ICommand AddEventCommand { get; set; }

        List<KeyValuePair<string, string>> ListKeyValuePair;

        public EventCreatorViewModel()
        {
            ListKeyValuePair = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("1116111648515721","Art"),
                new KeyValuePair<string, string>("1284277608291920", "Causes"),
                new KeyValuePair<string, string>("660032617536373", "Comedy"),
                new KeyValuePair<string, string>("258647957895086", "Crafts"),
                new KeyValuePair<string, string>("363764800677393", "Dance"),
                new KeyValuePair<string, string>("412284995786529", "Drinks"),
                new KeyValuePair<string, string>("392955781081975", "Film"),
                new KeyValuePair<string, string>("1138994019544264", "Fitness"),
                new KeyValuePair<string, string>("370585540007142", "Food"),
                new KeyValuePair<string, string>("1219165261515884", "Gardening"),
                new KeyValuePair<string, string>("1254988834549294", "Health"),
                new KeyValuePair<string, string>("220618358412161", "Home"),
                new KeyValuePair<string, string>("432347013823672", "Literature"),
                new KeyValuePair<string, string>("1821948261404481", "Music"),
                new KeyValuePair<string, string>("1915104302042536", "Networking"),
                new KeyValuePair<string, string>("183019258855149", "Party"),
                new KeyValuePair<string, string>("1763934757268181", "Religion"),
                new KeyValuePair<string, string>("1759906074034918", "Shopping"),
                new KeyValuePair<string, string>("607999416057365", "Sports"),
                new KeyValuePair<string, string>("664694117046626", "Theatre"),
                new KeyValuePair<string, string>("1712245629067288", "Wellness"),
                new KeyValuePair<string, string>("359809011100389", "Others")
            };

            EventCreatorModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyCreateNumberOfEventsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyCreateNumberOfEventsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyCreateNumberOfEventsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyCreateNumberOfMaximumEventsPerDay")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyCreateNumberOfEventsPerWeek")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            //Initialize the all model values
            EventCreatorModel.EventCreaterManagerModel.FbMultiMediaModel.IsMultiselect = false;
            EventCreatorModel.EventCreaterManagerModel.IsPrivatePostingVisibile = true;

            TypeSelectionChangedCommand = new BaseCommand<object>((sender) => true, TypeSelectionChangedCommandExecute);
            AddEventCommand = new BaseCommand<object>((sender) => true, AddEventExecute);
        }

        private void AddEventExecute(object sender)
        {
            Model.EventCreaterManagerModel.CategoryId = ListKeyValuePair.FirstOrDefault(x => x.Value == Model.EventCreaterManagerModel.Category).Key;

            if (string.IsNullOrEmpty(Model.EventCreaterManagerModel.EventName?.Trim()))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    $"{"LangKeyGiveAProperEventName".FromResourceDictionary()}");
                return;
            }

            if (Model.EventCreaterManagerModel.EventType == "CreatePublicEvent" && Model.EventCreaterManagerModel.Category == "Select Category")
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "LangKeySelectCategory".FromResourceDictionary());
                return;
            }

            if (Model.EventCreaterManagerModel.EventStartDate.AddDays(13) < Model.EventCreaterManagerModel.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyEventWillBeOfDay".FromResourceDictionary());
                return;
            }

            if (Model.EventCreaterManagerModel.EventStartDate < DateTime.Now
                || Model.EventCreaterManagerModel.EventEndDate < DateTime.Now)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyGiveStartAndEndDate".FromResourceDictionary());
                return;
            }

            if (Model.EventCreaterManagerModel.EventStartDate > Model.EventCreaterManagerModel.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyEndDateShouldNotBeLessThanStartDate".FromResourceDictionary());
                return;
            }

            if (Model.EventCreaterManagerModel.FbMultiMediaModel.MediaPaths.Count > 0)
            {
                Model.EventCreaterManagerModel.MediaPath = Model.EventCreaterManagerModel.FbMultiMediaModel.MediaPaths
               .FirstOrDefault().MediaPath;
            }
            Model.LstManageEventModel.Add(Model.EventCreaterManagerModel);

            Model.EventCreaterManagerModel = new EventCreaterManagerModel()
            {
                FbMultiMediaModel = new FbMultiMediaModel()
                {
                    IsMultiselect = false,
                    IsAddImageVisibile = true,
                    MediaPaths = new ObservableCollection<MultiMediaValueModel>()
                },
                EventType = EventType.CreatePrivateEvent.ToString()
            };
        }

        public void TypeSelectionChangedCommandExecute(object sender)
        {
            Model.EventCreaterManagerModel.IsPublicPostingVisibile = Model.EventCreaterManagerModel.EventType != EventType.CreatePrivateEvent.ToString();
            Model.EventCreaterManagerModel.IsPrivatePostingVisibile = Model.EventCreaterManagerModel.EventType == EventType.CreatePrivateEvent.ToString();
        }

        public EventCreatorModel Model => EventCreatorModel;
        private EventCreatorModel _eventCreatorModel = new EventCreatorModel();

        public EventCreatorModel EventCreatorModel
        {
            get { return _eventCreatorModel; }
            set
            {
                if (_eventCreatorModel == value)
                    return;
                SetProperty(ref _eventCreatorModel, value);

            }
        }

    }
}