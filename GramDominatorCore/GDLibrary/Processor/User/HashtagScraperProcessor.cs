using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class HashtagScraperProcessor : BaseInstagramUserProcessor
    {
        HashtagsScraperModel HashtagScraperModel { get; }
        public HashtagScraperProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
            HashtagScraperModel = JsonConvert.DeserializeObject<HashtagsScraperModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                if (HashtagScraperModel.LstKeyword.Count == 0)
                {
                    HashtagScraperModel.LstKeyword = Regex.Split(HashtagScraperModel.Keyword, "\r\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                }

                foreach (string hashTag in HashtagScraperModel.LstKeyword)
                {
                    // Get already scraped hashtags for current campaign with recent keyword.
                    // This line is used here to decrease overload on perforformance by selecting
                    // keyword wise data from database instead of selecting all at a time.
                    var alreadyScrapedHashtags = DbAccountService.GetHashtagScrape(DominatorAccountModel.UserName).Where(x => x.Keyword == hashTag);
                    //SearchTagIgResponseHandler searchTagIgResponseHandler = null;
                    //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    //{

                    //    searchTagIgResponseHandler = InstaFunction.SearchForTag(DominatorAccountModel, AccountModel, hashTag).Result;
                        
                    //}
                    //else
                    //{
                    //    searchTagIgResponseHandler = InstaFunction.GdBrowserManager.SearchForTag(DominatorAccountModel, hashTag, Token);
                    //}
                    var searchTagIgResponseHandler = InstaFunction.SearchForTag(DominatorAccountModel, AccountModel, hashTag).Result;
                    if (!CheckingLoginRequiredResponse(searchTagIgResponseHandler.ToString(), "", queryInfo))
                        continue;
                    var scrapedHashtags = alreadyScrapedHashtags as HashtagScrape[] ?? alreadyScrapedHashtags.ToArray();
                    if (searchTagIgResponseHandler.Success)
                    {
                        if (searchTagIgResponseHandler.Items.Count == 0)
                            continue;

                        foreach (var tagDetails in searchTagIgResponseHandler.Items)
                        {
                            if (scrapedHashtags.Any(tag => tag.HashtagName == tagDetails.Name))
                                continue;
                            ScrapeResultNewTag scrapresultWithTag = new ScrapeResultNewTag
                            {
                                TagDetails = tagDetails,
                                QueryInfo = new QueryInfo() { QueryValue = hashTag }
                            };
                            jobProcessResult = JobProcess.FinalProcess(scrapresultWithTag);
                            Token.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
