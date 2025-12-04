using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.Instachat
{
    public class SendMessageToFollowerViewModel : BindableBase
    {
        private ObservableCollection<MessageMediaInfo> mediaInfos=new ObservableCollection<MessageMediaInfo>();
        public SendMessageToFollowerViewModel()
        {
            SendMessageToFollowerModel.ManageMessagesModel = new ManageMessagesModel()
            {
                LstQueries = new ObservableCollection<QueryContent>()
                {
                    new QueryContent()
                    {
                        Content = new QueryInfo
                        {
                            QueryType = "All",
                            QueryValue = "All"
                        }
                    }
                }
            };
            SendMessageToFollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCommand = new BaseCommand<object>(saveCommandCanExecute, SaveCommandExecute);
        }
        public ICommand SaveCommand { get; set; }
        private SendMessageToFollowerModel _sendMessageToFollowerModel = new SendMessageToFollowerModel();
        private Visibility _visible=Visibility.Visible;
        public Visibility ShowMediaControl
        {
            get => _visible;
            set=>SetProperty(ref _visible, value);
        }
        public SendMessageToFollowerModel SendMessageToFollowerModel
        {
            get
            {
                return _sendMessageToFollowerModel;
            }
            set
            {
                if (_sendMessageToFollowerModel == null & _sendMessageToFollowerModel == value)
                    return;
                SetProperty(ref _sendMessageToFollowerModel, value);
            }
        }

        public SendMessageToFollowerModel Model => SendMessageToFollowerModel;

        private static bool saveCommandCanExecute(object sender)
        {
            return true;
        }

        private void SaveCommandExecute(object sender)
        {
            if (string.IsNullOrWhiteSpace(SendMessageToFollowerModel.TextMessage))
            {
                SendMessageToFollowerModel.TextMessage = string.Empty;
                GlobusLogHelper.log.Info("Please add custom user list");
            }

        }
        public ObservableCollection<MessageMediaInfo> Medias
        {
            get => mediaInfos;
            set { SetProperty(ref mediaInfos,value); }
        }
    }
}
