using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary
{
    public class ManageUniqueProcess
    {
        private readonly string CurrentActivityType = string.Empty;
        private readonly object InteractedUserObject = new object();

        private readonly object LockScrapedUser = new object();


        public ManageUniqueProcess(string campaignId, ActivityType activityType, DominatorAccountModel accountModel)
        {
            if (!Intialized)
            {
                #region  Initializing values

                try
                {
                    CurrentActivityType = activityType.ToString();
                    CampaignId = campaignId;
                    if (UniqueDict == null)
                        UniqueDict = new Dictionary<string, HashSet<string>>();

                    _dbAccountService =
                        InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(accountModel);
                    _campaignService = new DbCampaignService(campaignId);
                    Intialized = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
        }

        private static Dictionary<string, HashSet<string>> UniqueDict { get; set; }
        private string CampaignId { get; }
        private bool Intialized { get; }

        public bool IsUniqueUser(TwitterUser user)
        {
            try
            {
                lock (InteractedUserObject)
                {
                    if (UniqueDict == null) UniqueDict = new Dictionary<string, HashSet<string>>();
                    if (!UniqueDict.ContainsKey(CampaignId))
                    {
                        var hash = new HashSet<string>();
                        UniqueDict.Add(CampaignId, new HashSet<string>());
                        if (CurrentActivityType == ActivityType.Follow.ToString())
                        {
                            var IntereactedUser = _campaignService.GetAllUnfollowedUsers().ToList();
                            IntereactedUser.ForEach(x => hash.Add(x.UserId));
                        }
                        else
                        {
                            var IntereactedUser =
                                _campaignService.GetAllInteractedUsers()
                                    .ToList(); //<DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedUsers>();
                            IntereactedUser.ForEach(x => hash.Add(x.InteractedUserId));
                        }

                        UniqueDict[CampaignId] = hash;
                    }

                    return UniqueDict[CampaignId].Add(user.UserId);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }

        public bool IsActivityDoneWithThisUserId(string userId)
        {
            try
            {
                lock (LockScrapedUser)
                {
                    var count = _dbAccountService.Count<InteractedUsers>(x =>
                        x.InteractedUserId == userId && x.ActivityType == CurrentActivityType);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        #region database instance

        private readonly IDbAccountService _dbAccountService;
        private readonly IDbCampaignService _campaignService;

        #endregion
    }
}