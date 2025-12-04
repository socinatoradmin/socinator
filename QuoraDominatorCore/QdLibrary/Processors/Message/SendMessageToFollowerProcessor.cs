using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Message
{
    public interface ISendMessageToFollowerProcessor : IQueryProcessor
    {

    }

    public class SendMessageToFollowerProcessor : BaseQuoraProcessor, ISendMessageToFollowerProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public SendMessageToFollowerProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var url = quoraFunct.GetProfileUrl(JobProcess.DominatorAccountModel);
                FollowerResponseHandler scrapedFollower = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    var scrollResponse = quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(url), UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel, -1, "MostRecent").Result;
                    scrapedFollower = new FollowerResponseHandler(scrollResponse,IsBrowser);
                }
                else
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var resp = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.OwnFollowers);
                    scrapedFollower = new FollowerResponseHandler(resp,IsBrowser);
                }
                var temp = DbAccountService.GetInteractedMessage().Select(x => x.Username).ToList();
                var SkippedUserCount=scrapedFollower.FollowerList.RemoveAll(x => temp.Any(y => y == x||x.Contains(y)));
                if (!jobProcessResult.IsProcessCompleted && scrapedFollower.FollowerList.Count != 0)
                    jobProcessResult = FilterAndStartFinalProcess(null, jobProcessResult, scrapedFollower.FollowerList);
                if (jobProcessResult.IsProcessCompleted || scrapedFollower.FollowerList.Count == 0)
                    return;
                StartPagination(ref jobProcessResult, scrapedFollower);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (_browser.BrowserWindow != null)
                    _browser.CloseBrowser();
            }
        }

        private void StartPagination(ref JobProcessResult jobProcessResult, FollowerResponseHandler scrapedFollower)
        {
            var responseFollowerFirstPage = scrapedFollower.Response.Response;
            var post = new BasePostData(responseFollowerFirstPage);
            do
            {
                try
                {
                    var urlServerCallPost = "https://www.quora.com/webnode2/server_call_POST?_v=" +
                                            Uri.EscapeDataString(post.VconJson) + "&_m=increase_count";

                    var postDataCallPost = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22cid%22%3A%22" +
                                           post.PagedList +
                                           "%22%2C%22num%22%3A18%2C%22current%22%3A72%7D%7D&revision=" +
                                           post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey +
                                           "&window_id=" + post.WindowId +
                                           "&referring_controller=user&referring_action=followers&__vcon_json=%5B%22" +
                                           "" + Uri.EscapeDataString(post.VconJson) +
                                           "%22%5D&__vcon_method=increase_count&__e2e_action_id=exz34w8dfi&js_init=%7B%22object_id%22%3A44794736%2C%22initial_count%22%3A18%2C%22buffer_count%22%3A18%2C%22crawler%22%3Afalse%2C%22has_more%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22fixed_size_paged_list%22%3Afalse%2C%22auto_paged%22%3Atrue%7D&__metadata=%7B%7D";

                    _httpHelper.PostRequest(urlServerCallPost, postDataCallPost);

                    var urlPagination = "https://tch678384.tch.quora.com/up/" + post.Chan + "/updates?&callback=" +
                                        post.JsonP + "&" +
                                        "min_seq=" + post.MinSeq + "&channel=" + post.Channel + "&hash=" + post.Hash +
                                        "&_=" + GenerateTimeStamp();

                    scrapedFollower = quoraFunct.Follower(JobProcess.DominatorAccountModel, urlPagination);
                    if (scrapedFollower.FollowerList.Count == 0 || jobProcessResult.IsProcessCompleted)
                        break;
                    if (scrapedFollower.Success)
                        if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                            jobProcessResult =
                                FilterAndStartFinalProcess(null, jobProcessResult, scrapedFollower.FollowerList);
                    var followerpaginationData = scrapedFollower.Response.Response;

                    if (followerpaginationData.Contains("jsonp"))
                        post.JsonP = "jsonp" + Utilities.GetBetween(followerpaginationData, "jsonp", "({");

                    if (followerpaginationData.Contains("min_seq\":"))
                        post.MinSeq = Utilities.GetBetween(followerpaginationData, "min_seq\":", "})");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    break;
                }
            } while (!string.IsNullOrEmpty(post.MinSeq));
        }
    }
}