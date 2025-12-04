using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace RedditDominatorCore.RDLibrary
{
    public interface IRdUpdateAccountProcess
    {
        Task UpdateAllUserDetails(DominatorAccountModel objDominatorAccountModel, CancellationToken token);
    }

    public class RdUpdateAccountProcess : IRdUpdateAccountProcess
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        public RdUpdateAccountProcess(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }


        public async Task UpdateAllUserDetails(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                await UpdateAccountInfo(objDominatorAccountModel, token);
                await UpdateCommunities(objDominatorAccountModel, token);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task UpdateAccountInfo(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                var objRedditFunction =
                    _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IRedditFunction>();
                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");

                var objResponseHandler = await objRedditFunction.GetUserDetailsAsync(objDominatorAccountModel, token);
                objDominatorAccountModel.DisplayColumnValue1 =
                    Convert.ToInt32(objResponseHandler.PostKarma) + Convert.ToInt32(objResponseHandler.CommentKarma) + Convert.ToInt32(objResponseHandler.AwardedKarma);
                objDominatorAccountModel.DisplayColumnValue3 = Convert.ToInt32(objResponseHandler.PostKarma);
                objDominatorAccountModel.DisplayColumnValue4 = Convert.ToInt32(objResponseHandler.CommentKarma);
                objDominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateDisplayColumn1(objDominatorAccountModel.DisplayColumnValue1)
                    .AddOrUpdateDisplayColumn3(objDominatorAccountModel.DisplayColumnValue3)
                    .AddOrUpdateDisplayColumn4(objDominatorAccountModel.DisplayColumnValue4)
                    .AddOrUpdateCookies(objDominatorAccountModel.Cookies)
                    .SaveToBinFile();
                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task UpdateCommunities(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                var objRedditFunction =
                    _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IRedditFunction>();
                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Communities");
                var communitiesScraperResponseHandler =
                    await objRedditFunction.ScrapeCommunitiesAsync(objDominatorAccountModel, token);
                var databaseConnection = SocinatorInitialize
                    .GetSocialLibrary(objDominatorAccountModel.AccountBaseModel.AccountNetwork).GetNetworkCoreFactory()
                    .AccountDatabase;
                var dbContext = databaseConnection.GetSqlConnection(objDominatorAccountModel.AccountId);
                var dbAccountOperation = new DbOperations(dbContext);

                dbAccountOperation.RemoveAll<OwnCommunities>();

                foreach (var community in communitiesScraperResponseHandler.LstCommunities)
                    dbAccountOperation.Add(new OwnCommunities
                    {
                        WhitelistStatus = community.WhitelistStatus,
                        IsNsfw = community.IsNsfw,
                        Subscribers = community.Subscribers,
                        PrimaryColor = community.PrimaryColor,
                        CommunityId = community.CommunityId,
                        IsQuarantined = community.IsQuarantined,
                        Name = community.Name,
                        Title = community.Title,
                        Url = community.Url,
                        DisplayText = community.DisplayText,
                        Type = community.Type,
                        CommunityIcon = community.CommunityIcon,
                        IsOwn = community.IsOwn
                    });
                objDominatorAccountModel.DisplayColumnValue2 = communitiesScraperResponseHandler.LstCommunities.Count;

                SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateLoginStatus(objDominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateDisplayColumn2(objDominatorAccountModel.DisplayColumnValue2)
                    .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                    .SaveToBinFile();

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Communities");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}