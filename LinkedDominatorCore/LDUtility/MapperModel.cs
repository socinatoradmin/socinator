using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorCore.LDModel;

namespace LinkedDominatorCore.LDUtility
{
    public class MapperModel
    {
        #region Initializations

        public bool IsCheckedBySoftware { get; set; }
        public bool IsCheckedOutSideSoftware { get; set; }
        public bool IsCheckedLangKeyCustomUserList { get; set; }
        public bool IsCheckedFilterProfileImageCheckbox { get; set; }
        public bool IsCheckedConnectedBefore { get; set; }
        public bool IsCheckedRequestedBefore { get; set; }
        public bool IsChkSkipBlackListedUser { get; set; }
        public bool IsChkPrivateBlackList { get; set; }
        public bool IsChkGroupBlackList { get; set; }
        public bool IsChkSkipWhiteListedUser { get; set; }
        public bool IsChkUsePrivateWhiteList { get; set; }
        public bool IsChkUseGroupWhiteList { get; set; }
        public bool IsCheckedRecentConnections { get; set; }
        public bool IsDonwloadAttachment { get; set; }
        public bool IsSkipUserAlreadyRecievedMessageFromSoftware { get; set; }
        public bool IsSkipUserAlreadyRecievedMessageFromOutSideSoftware { get; set; }
        public string UrlInput { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }

        #region group properties

        public bool IsGroup { get; set; }
        public bool IsFollower {  get; set; }
        public string CurrentGroup { get; set; }
        public string GroupUrlInput { get; set; }
        public List<string> GroupUrlList { get; set; } = new List<string>();
        public int CurrentCount { get; set; }

        #endregion

        public List<LinkedinUser> ListUsersFromSelectedSource { get; set; } = new List<LinkedinUser>();
        public List<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } = new List<ManageMessagesModel>();
        public List<string> ListCustomUser { get; set; } = new List<string>();

        public List<string> UrlList { get; set; } = new List<string>();
        public bool IsStopScheduling { get; set; }

        public void SetCustomList()
        {
            try
            {
                if (!string.IsNullOrEmpty(UrlInput))
                    UrlList = Regex.Split(UrlInput, "\r\n")
                        .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                if (!string.IsNullOrEmpty(GroupUrlInput))
                    GroupUrlList = Regex.Split(GroupUrlInput, "\r\n")
                        .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        #endregion
    }
}