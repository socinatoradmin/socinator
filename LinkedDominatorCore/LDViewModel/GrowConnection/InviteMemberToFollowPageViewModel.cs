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
    public class InviteMemberToFollowPageViewModel : BindableBase
    {
        private InviteMemberToFollowPageModel _inviteMemberToFollowPageModel = new InviteMemberToFollowPageModel();

        public InviteMemberToFollowPageViewModel()
        {
            InviteMemberToFollowPageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendinvitationPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfSendinvitationPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberSendinvitationPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfSendinvitationPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToSendinvitationPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomAdminPageUrl);
        }

        public InviteMemberToFollowPageModel InviteMemberToFollowPageModel
        {
            get => _inviteMemberToFollowPageModel;
            set
            {
                if ((_inviteMemberToFollowPageModel == null) & (_inviteMemberToFollowPageModel == value))
                    return;
                SetProperty(ref _inviteMemberToFollowPageModel, value);
            }
        }

        public InviteMemberToFollowPageModel Model => InviteMemberToFollowPageModel;

        public ICommand SaveCustomUserListCommand { get; set; }

        private void SaveCustomAdminPageUrl(object sender)
        {
            try
            {
                if (InviteMemberToFollowPageModel.UrlInput.Contains("\r\n"))
                {
                    InviteMemberToFollowPageModel.UrlList =
                        Regex.Split(InviteMemberToFollowPageModel.UrlInput, "\r\n")
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();

                    GlobusLogHelper.log.Info("" + InviteMemberToFollowPageModel.UrlList.Count +
                                             " Event urls saved sucessfully");
                }
                else
                {
                    InviteMemberToFollowPageModel.UrlList = new List<string>() { InviteMemberToFollowPageModel.UrlInput };
                    GlobusLogHelper.log.Info("One Event url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}