using System;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Messages
{
    public class SendMessageToFollowerViewModel : BindableBase
    {
        private SendMessageToFollowerModel _sendMessageToFollowerModel = new SendMessageToFollowerModel();

        public SendMessageToFollowerViewModel()
        {
            SendMessageToFollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendMessegesToNewFollowerPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName =
                    "LangKeyNumberOfSendMessegesToNewfollowerPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfSendMessegesToNewFollowerPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName =
                    "LangKeyNumberOfSendMessegesToNewFollowerPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxSendmessegestonewfollowerPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveInputCommand = new BaseCommand<object>(sender => true, SaveInput);
        }


        #region Commands

        public ICommand SaveInputCommand { get; set; }

        #endregion

        public SendMessageToFollowerModel SendMessageToFollowerModel
        {
            get => _sendMessageToFollowerModel;
            set
            {
                if ((_sendMessageToFollowerModel == null) & (_sendMessageToFollowerModel == value))
                    return;
                SetProperty(ref _sendMessageToFollowerModel, value);
            }
        }

        public SendMessageToFollowerModel Model => SendMessageToFollowerModel;

        private void SaveInput(object sender)
        {
            SendMessageToFollowerModel.Message = sender as string;
        }
    }
}