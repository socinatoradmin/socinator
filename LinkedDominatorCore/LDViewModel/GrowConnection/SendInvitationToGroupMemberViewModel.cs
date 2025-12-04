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
using LinkedDominatorCore.LDModel.GrowConnection;

namespace LinkedDominatorCore.LDViewModel.GrowConnection
{
    
    public class SendInvitationToGroupMemberViewModel : BindableBase
    {
        private SendInvitationToGroupMemberModel _sendInvitationToGroupMemberModel = new SendInvitationToGroupMemberModel();

        public SendInvitationToGroupMemberViewModel()
        {
            SendInvitationToGroupMemberModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendinvitationPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfSendinvitationPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberSendinvitationPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfSendinvitationPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToSendinvitationPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomAdminGroupUrl);
        }

        public SendInvitationToGroupMemberModel SendInvitationToGroupMemberModel
        {
            get => _sendInvitationToGroupMemberModel;
            set
            {
                if ((_sendInvitationToGroupMemberModel == null) & (_sendInvitationToGroupMemberModel == value))
                    return;
                SetProperty(ref _sendInvitationToGroupMemberModel, value);
            }
        }

        public SendInvitationToGroupMemberModel Model => SendInvitationToGroupMemberModel;

        public ICommand SaveCustomUserListCommand { get; set; }

        private void SaveCustomAdminGroupUrl(object sender)
        {
            try
            {
                if (SendInvitationToGroupMemberModel.UrlInput.Contains("\r\n"))
                {
                    SendInvitationToGroupMemberModel.UrlList =
                        Regex.Split(SendInvitationToGroupMemberModel.UrlInput, "\r\n")
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();

                    GlobusLogHelper.log.Info("" + SendInvitationToGroupMemberModel.UrlList.Count +
                                             " AdminGroup urls saved sucessfully");
                }
                else
                {
                    SendInvitationToGroupMemberModel.UrlList = new List<string>() { SendInvitationToGroupMemberModel.UrlInput };
                    GlobusLogHelper.log.Info("One AdminGroup url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
