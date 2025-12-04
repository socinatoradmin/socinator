using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Linq;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class BlockFollowerProcessor : BaseInstagramUserProcessor
    {
        public BlockFollowerProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService, processScopeModel,delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            Token.ThrowIfCancellationRequested();
            var allFollowers = DbAccountService.GetFollowers().Where(x => x.Followers == 1);

            try
            {
                if(allFollowers.ToList().Count > 0)
                {
                    foreach (var follower in allFollowers)
                    {
                        InstagramUser instagramUser = new InstagramUser
                        {
                            Username = follower.Username,
                            UserId = follower.UserId,
                            FullName = follower.FullName,
                            IsPrivate = follower.IsPrivate,
                            IsVerified = follower.IsVerified,
                            IsBusiness = follower.IsBusiness,
                            ProfilePicUrl = follower.ProfilePicUrl,
                            HasAnonymousProfilePicture = follower.HasAnonymousProfilePicture
                        };

                        FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, instagramUser);

                        Token.ThrowIfCancellationRequested();
                    }
                }
                else
                {
                    var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, DominatorAccountModel?.AccountBaseModel?.ProfileId, DominatorAccountModel.Token);
                    var followers = InstaFunction.GetUserFollowers(DominatorAccountModel, userInfo?.Pk, DominatorAccountModel.Token).Result;
                    if(followers != null && followers.UsersList.Count > 0)
                    {
                        foreach(var instagramUser in followers.UsersList)
                        {
                            Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, instagramUser);

                            Token.ThrowIfCancellationRequested();
                        }
                    }
                    jobProcessResult.IsProcessCompleted = true;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception )
            {
               // ex.DebugLog();
            }
        }
    }
}
