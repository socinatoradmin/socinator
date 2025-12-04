using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominatorCore.PDEnums;
using System.Collections.Generic;
using PinDominatorCore.PDLibrary.DAL;
using System;
using DominatorHouseCore.Utility;
using System.Text.RegularExpressions;
using DominatorHouseCore.LogHelper;
using PinDominatorCore.Utility;

namespace PinDominatorCore.PDUtility
{
    public static class PdStatic
    {
        public static System.Drawing.Rectangle ScreenResolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        private static JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        /// <summary>
        ///     returns Pinterest Module Type (Pin or User or Borad) - On which Pinterest thing the activitytype is going to
        ///     perform
        /// </summary>
        /// <param name="actType"></param>
        /// <returns>PinterestElements</returns>
        public static PdElements GetPdElementByActivityType(this ActivityType actType)
        {
            if (actType == ActivityType.Try || actType == ActivityType.Comment || actType == ActivityType.Repin ||
                actType == ActivityType.PinScraper || actType == ActivityType.DeletePin)
                return PdElements.Pin;
            if (actType == ActivityType.UserScraper || actType == ActivityType.Follow
                                                    || actType == ActivityType.BroadcastMessages ||
                                                    actType == ActivityType.SendMessageToFollower
                                                    || actType == ActivityType.AutoReplyToNewMessage)
                return PdElements.Users;
            if (actType == ActivityType.BoardScraper)
                return PdElements.Board;

            return PdElements.None;
        }

        /// <summary>
        ///     Get a deleting query from the list of QueryContent Just by comparing the another QueryContent with any of the list
        /// </summary>
        /// <param name="queryList">The list of QueryContent</param>
        /// <param name="queryToDelete">the another QueryContent to compare</param>
        /// <returns></returns>
        public static QueryContent GetDeletingQuery(this ObservableCollection<QueryContent> queryList,
            QueryContent queryToDelete)
        {
            return queryList.FirstOrDefault(x =>
                x.Content.QueryType == queryToDelete.Content.QueryType &&
                x.Content.QueryValue == queryToDelete.Content.QueryValue);
        }

        public static readonly object LockInitializingCommentLockDict = new object();
        public static Dictionary<string, object> LockUniqueCommentsDict = new Dictionary<string, object>();

        public static Dictionary<string, Dictionary<string, List<string>>> UniqueCommentsListWithPinId = new Dictionary<string, Dictionary<string, List<string>>>();
        public static Dictionary<string, Dictionary<string, bool>> FirstTimeUniqueCommentListFromDb = new Dictionary<string, Dictionary<string, bool>>();
        public static Dictionary<string, List<string>> UniqueCommentsListUniqueCmntUniqueAccount = new Dictionary<string, List<string>>();
        public static Dictionary<string, bool> FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount = new Dictionary<string, bool>();

        public static List<string> UnusedCommentsForUnique(string accountId, string campaignId, string pinUrl, List<string> msgList, IDbCampaignService campaignService, IDbAccountServiceScoped accountServiceScoped, bool campaignWise, bool accountwise)
        {
            var pinId = pinUrl;
            if (FirstTimeUniqueCommentListFromDb[campaignId][pinId])
            {
                FirstTimeUniqueCommentListFromDb[campaignId][pinId] = false;

                var usedComment = new List<string>();

                var activityType = ActivityType.Comment.ToString();
                if (campaignWise)
                {
                    var postsDone = campaignService.Get<DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts>(x => x.OperationType == activityType);
                    postsDone = postsDone.Where(x => x.PinId.Contains(pinId)).ToList();
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.Comment))
                            usedComment.Add(x.Comment);
                    });
                }
                else
                {
                    var postsDone = accountServiceScoped.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts>(x => x.OperationType == activityType);
                    postsDone = postsDone.Where(x => x.PinId.Contains(pinId)).ToList();
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.Comment))
                            usedComment.Add(x.Comment);
                    });
                }

                if (usedComment.Count > 0)
                {
                    foreach (var cmnt in msgList)
                    {
                        var usedCommentsChangedTemp = new List<string>();
                        usedCommentsChangedTemp.AddRange(usedComment);
                        var addit = true;
                        var index = 0;
                        if (usedCommentsChangedTemp.Count > 0)
                        {
                            foreach (var usedCmnt in usedCommentsChangedTemp)
                            {
                                ++index;
                                if (usedCmnt == cmnt)
                                {
                                    usedComment.RemoveAt(index - 1);
                                    addit = false;
                                    break;
                                }
                            }
                        }
                        if (addit)
                            UniqueCommentsListWithPinId[campaignId][pinId].Add(cmnt);
                    }
                }
                else
                    UniqueCommentsListWithPinId[campaignId][pinId].AddRange(msgList);
            }

            return UniqueCommentsListWithPinId[campaignId][pinId];
        }

        public static void RemoveUsedUniqueCommentFromUniqueUser(ref string usedUniqueComment, string campaignId)
        {
            if (UniqueCommentsListUniqueCmntUniqueAccount?[campaignId]?.Count > 0)
            {
                var tempListComments = new List<string>();

                usedUniqueComment = UniqueCommentsListUniqueCmntUniqueAccount[campaignId].ElementAt(new Random().Next(0, UniqueCommentsListUniqueCmntUniqueAccount[campaignId].Count));
                tempListComments.AddRange(UniqueCommentsListUniqueCmntUniqueAccount[campaignId]);

                var index = 0;
                foreach (var cmnt in tempListComments)
                {
                    ++index;
                    if (cmnt == usedUniqueComment)
                    {
                        UniqueCommentsListUniqueCmntUniqueAccount[campaignId].RemoveAt(index - 1);
                        break;
                    }
                }
            }
        }

        public static List<string> UnusedCommentsForUniqueCommentFromUniqueUser(string campaignId, List<string> msgList,
            IDbCampaignService campaignService, IDbAccountServiceScoped accountServiceScoped, bool campaignWise, bool isAllowMultiple)
        {
            var usedComment = new List<string>();

            var activityType = ActivityType.Comment.ToString();
            if (campaignWise)
            {
                var postsDone = campaignService.Get<DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts>(x => x.OperationType == activityType);
                postsDone.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Comment))
                        usedComment.Add(x.Comment);
                });
            }
            else
            {
                var postsDone = accountServiceScoped.Get<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.InteractedPosts>(x => x.OperationType == activityType);
                postsDone.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Comment))
                        usedComment.Add(x.Comment);
                });
            }

            if (usedComment.Count > 0)
            {
                foreach (var cmnt in msgList)
                {
                    var usedCommentsChangedTemp = new List<string>();
                    usedCommentsChangedTemp.AddRange(usedComment);
                    var addit = true;
                    var index = 0;
                    if (usedCommentsChangedTemp.Count > 0)
                    {
                        foreach (var usedCmnt in usedCommentsChangedTemp)
                        {
                            ++index;
                            if (usedCmnt == cmnt)
                            {
                                usedComment.RemoveAt(index - 1);
                                addit = false;
                                break;
                            }
                        }
                    }
                    if (addit)
                        UniqueCommentsListUniqueCmntUniqueAccount[campaignId].Add(cmnt);
                }
            }
            else
                UniqueCommentsListUniqueCmntUniqueAccount[campaignId].AddRange(msgList);
            return UniqueCommentsListUniqueCmntUniqueAccount[campaignId];

        }

        public static void RemoveUsedUniqueComment(ref string usedUniqueComment, string campaignId, string processingUrl)
        {
            if (UniqueCommentsListWithPinId?[campaignId]?[processingUrl]?.Count > 0)
            {
                var tempListComments = new List<string>();

                usedUniqueComment = UniqueCommentsListWithPinId[campaignId][processingUrl].ElementAt(new Random().Next(0, UniqueCommentsListWithPinId[campaignId][processingUrl].Count));
                tempListComments.AddRange(UniqueCommentsListWithPinId[campaignId][processingUrl]);

                var index = 0;
                foreach (var cmnt in tempListComments)
                {
                    ++index;
                    if (cmnt == usedUniqueComment)
                    {
                        UniqueCommentsListWithPinId[campaignId][processingUrl].RemoveAt(index - 1);
                        break;
                    }
                }
            }
        }
        public static void FailedLog(DominatorAccountModel dominatorAccountModel, string msg)
        => GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, msg);
        public static string GetFailedMessage(AccountStatus accountStatus)
        {
            return accountStatus == AccountStatus.SetNewPassword ?"LangKeyAccountBlockedResetPassword".FromResourceDictionary() :
                        accountStatus == AccountStatus.InvalidCredentials ?"LangKeyEmailOrPasswordIncorrect".FromResourceDictionary() :
                        accountStatus == AccountStatus.TemporarilyBlocked ?"LangKeyBlockActionsBecauseOfSuspiciousActivity".FromResourceDictionary() : "Something Went Wrong";
        }
    }
}