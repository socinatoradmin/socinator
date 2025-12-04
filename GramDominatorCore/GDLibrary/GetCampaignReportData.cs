using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using System.Text;
using System.Threading.Tasks;
using GramDominatorCore.Report;
using DominatorHouseCore;
using System.Threading;

namespace GramDominatorCore.GDLibrary
{
    public class GetCampaignReportData
    {
        public void GetCamaignFollowerCount(string Account,string query, CampaignDetails campaign, List<FollowReportDetails> lstReports,out string FollowedCounts,out string FollowBackCounts)
        {          
            var dataBase = new DbCampaignService(campaign.CampaignId);          
            int FollowedCount = dataBase.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>().Where(q => q.Username == Account && q.Query == query).Count();
            FollowedCounts = FollowedCount.ToString();
            FollowBackCounts = GettingFollowBack(Account, query, lstReports);         
        }
        public string GettingFollowBack(string AccountName,string query, List<FollowReportDetails> lstReports)
        {
            int followBackCount = 0;
            try
            {
                List<InstagramUser> lstInstagramUser = new List<InstagramUser>();
                string maxid = null;
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var _accountModel = accountsFileManager.GetAccount(AccountName, SocialNetworks.Instagram);
                var scope = InstanceProvider.GetInstance<IAccountScopeFactory>();
                var Instafunc = scope[_accountModel.AccountId].Resolve<IInstaFunction>();
                var httphelper = scope[_accountModel.AccountId].Resolve<IGdHttpHelper>();
                IgRequestParameters requestParameter = new IgRequestParameters(_accountModel.UserAgentMobile);
                requestParameter.Cookies = _accountModel.Cookies;
                httphelper.SetRequestParameter(requestParameter);
                do
                {
                    var userInfo = Instafunc.SearchUserInfoById(_accountModel, null, _accountModel.AccountBaseModel.UserId, CancellationToken.None).Result;
                    FollowerAndFollowingIgResponseHandler userFollowings = Instafunc.GetUserFollowers(_accountModel, _accountModel.AccountBaseModel.UserId, CancellationToken.None, maxid, _accountModel.AccountBaseModel.ProfileId,IsWeb:true).Result;
                   if(lstInstagramUser.Count>= userInfo.FollowerCount)
                        break;
                    
                    lstInstagramUser.AddRange(userFollowings.UsersList);
                    maxid = userFollowings.MaxId;
                } while (!string.IsNullOrEmpty(maxid));

                List<FollowReportDetails> eachQueryList = lstReports.Where(x => x.Query == query).ToList();
                followBackCount = lstInstagramUser.Where(x => eachQueryList.Any(y => y.InteractedUserId == x.Pk)).Count();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return followBackCount.ToString();
        }
    }
}
