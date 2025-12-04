using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.PinMessenger
{
    public class SendMessageToNewFollowersViewModel : BindableBase
    {
        private SendMessageToNewFollowersModel _sendMessageToNewFollowersModel = new SendMessageToNewFollowersModel();

        public SendMessageToNewFollowersViewModel()
        {
            SendMessageToNewFollowersModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendMessageToNewFollowersPerJob".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfSendMessageToNewFollowersPerDay".FromResourceDictionary(),
                ActivitiesPerHourDisplayName =
                    "LangKeyNumberOfSendMessageToNewFollowersPerHour".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName =
                    "LangKeyNumberOfSendMessageToNewFollowersPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAutoReplyToNewMessegePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            UploadNotesCommand = new BaseCommand<object>(sender => true, CustomUser);
        }

        public SendMessageToNewFollowersModel Model => SendMessageToNewFollowersModel;

        public ICommand UploadNotesCommand { get; set; }

        public SendMessageToNewFollowersModel SendMessageToNewFollowersModel
        {
            get => _sendMessageToNewFollowersModel;
            set
            {
                if ((_sendMessageToNewFollowersModel == null) & (_sendMessageToNewFollowersModel == value))
                    return;
                SetProperty(ref _sendMessageToNewFollowersModel, value);
            }
        }

        private void CustomUser(object sender)
        {
            try
            {
                var messageData = sender as InputBoxControl;
                if (messageData != null)
                {
                    if (string.IsNullOrEmpty(messageData.InputText))
                        return;
                    if (messageData != null)
                        SendMessageToNewFollowersModel.LstMessages = new List<string> { messageData.InputText };
                }

                GlobusLogHelper.log.Info("LangKeyMessagesSavedSuccessfully".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}