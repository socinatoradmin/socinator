using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class OwnFollowingsProcessor : BaseInstagramUserProcessor
    {
        public List<Friendships> AllFollowings = new List<Friendships>();
        public OwnFollowingsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService,gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;

                AllFollowings = DbAccountService.GetFollowings().Where(x => x.Followings == 1).ToList();                
                List<InstagramUser> allFollowing = GetAllUser(AllFollowings);
                if (allFollowing.Count!= 0)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, allFollowing);                
                else
                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
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
