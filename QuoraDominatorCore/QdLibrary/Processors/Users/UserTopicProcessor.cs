using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class UserTopicProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;

        public UserTopicProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                jobProcessResult = ScrapeTopicFollower(queryinfo, ref jobProcessResult);
        }

        private JobProcessResult ScrapeTopicFollower(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            string url;
            try
            {
                if (queryinfo.QueryValue.Contains("http"))
                    url = queryinfo.QueryValue + "/followers";
                else
                    url = $"{QdConstants.HomePageUrl}/topic/" + queryinfo.QueryValue + "/followers";

                var objTopicFollowersResponseHandler = quoraFunct.TopicFollowers(JobProcess.DominatorAccountModel, url);
                var responseFollowerFirstPage = objTopicFollowersResponseHandler.Response.Response;


                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult = FilterAndStartFinalProcess(queryinfo, jobProcessResult,
                    objTopicFollowersResponseHandler.TopicFollowers);
                if (jobProcessResult.IsProcessCompleted)
                    return jobProcessResult;

                jobProcessResult = StartPagination(queryinfo, responseFollowerFirstPage, jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartPagination(QueryInfo queryinfo, string responseFollowerFirstPage,
            JobProcessResult objJobProcessResult)
        {
            var post = new BasePostData(responseFollowerFirstPage);
            var followerPaginationData = string.Empty;
            do
            {
                try
                {
                    var urlServerCallPost = "https://www.quora.com/webnode2/server_call_POST?_v=" +
                                            Uri.EscapeDataString(post.VconJson) +
                                            "&_m=increase_count";
                    var postDataCallPost = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22cid%22%3A%22" +
                                           post.PagedList +
                                           "%22%2C%22num%22%3A18%2C%22current%22%3A72%7D%7D&revision=" +
                                           post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey +
                                           "&window_id=" + post.WindowId +
                                           "&referring_controller=user&referring_action=followers&__vcon_json=%5B%22" +
                                           "" +
                                           Uri.EscapeDataString(post.VconJson) +
                                           "%22%5D&__vcon_method=increase_count&__e2e_action_id=exz34w8dfi&js_init=%7B%22object_id%22%3A44794736%2C%22initial_count%22%3A18%2C%22buffer_count%22%3A18%2C%22crawler%22%3Afalse%2C%22has_more%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22fixed_size_paged_list%22%3Afalse%2C%22auto_paged%22%3Atrue%7D&__metadata=%7B%7D";

                    _httpHelper.PostRequest(urlServerCallPost, postDataCallPost);
                    var urlPagination = "https://tch678384.tch.quora.com/up/" + post.Chan + "/updates?&callback=" +
                                        post.JsonP + "&" +
                                        "min_seq=" + post.MinSeq + "&channel=" + post.Channel + "&hash=" + post.Hash +
                                        "&_=" + GenerateTimeStamp();

                    var objTopicFollowersResponseHandler =
                        quoraFunct.TopicFollowers(JobProcess.DominatorAccountModel, urlPagination);

                    if (objTopicFollowersResponseHandler.TopicFollowers.Count == 0)
                        break;
                    if (!objJobProcessResult.IsProcessCompleted)
                        objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                            objTopicFollowersResponseHandler.TopicFollowers);

                    followerPaginationData = objTopicFollowersResponseHandler.Response.Response;
                    if (followerPaginationData.Contains("jsonp"))
                        post.JsonP = "jsonp" + Utilities.GetBetween(followerPaginationData, "jsonp", "({");

                    if (followerPaginationData.Contains("min_seq\":"))
                        post.MinSeq = Utilities.GetBetween(followerPaginationData, "min_seq\":", "})");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.ToString().Contains("Value cannot be null."))
                        break;
                }
            } while (!string.IsNullOrEmpty(followerPaginationData));

            return objJobProcessResult;
        }
    }
}