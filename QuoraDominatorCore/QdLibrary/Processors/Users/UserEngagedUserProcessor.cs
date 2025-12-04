using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class UserEngagedUserProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;

        public UserEngagedUserProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
            var typeAndUrl = queryinfo.QueryValue.Split(':');
            var type = typeAndUrl[0].ToLower();
            var questionOrAnswer = typeAndUrl[1].ToLower();
            var url = typeAndUrl[2] + ":" + typeAndUrl[3];
            var objQuoraUser = new QuoraUser { Url = url };
            if (type == "topic")
                jobProcessResult = ScrapeTopicFollower(queryinfo, objQuoraUser);
            else if (type.Equals("answer")) //who has answered
                jobProcessResult = ScraperUserNameWhoAnswered(queryinfo, objQuoraUser);

            else if (type.Equals("comment") && questionOrAnswer.Equals("question"))
                jobProcessResult = ScrapeQuestionComments(queryinfo, objQuoraUser);

            else if (type.Equals("comment") && questionOrAnswer.Equals("answer"))
                jobProcessResult = ScrapeWhoCommentOnAnswer(queryinfo, objQuoraUser);
        }

        private JobProcessResult ScrapeTopicFollower(QueryInfo queryinfo, QuoraUser objQuoraUser)
        {
            var middle = queryinfo.QueryType == "Engaged Users" ? objQuoraUser.Url : queryinfo.QueryValue;
            var objJobProcessResult = new JobProcessResult();
            string url;
            if (middle.Contains("http"))
                url = $"{QdConstants.HomePageUrl}/topic/" + middle.Replace($"{QdConstants.HomePageUrl}/topic/", "") +
                      "/followers";
            else
                url = $"{QdConstants.HomePageUrl}/topic/" + middle + "/followers";


            var objTopicFollowersResponseHandler = quoraFunct.TopicFollowers(JobProcess.DominatorAccountModel, url);
            var responseFollowerFirstPage = objTopicFollowersResponseHandler.Response.Response;
            objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                objTopicFollowersResponseHandler.TopicFollowers);

            if (objJobProcessResult.IsProcessCompleted)
                return objJobProcessResult;
            var post = new BasePostData(responseFollowerFirstPage);
            var followerPaginationData = string.Empty;
            do
            {
                try
                {
                    var urlServerCallPost =
                        $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_v=" + Uri.EscapeDataString(post.VconJson) +
                        "&_m=increase_count";
                    var postDataCallPost = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22cid%22%3A%22" +
                                           post.PagedList +
                                           "%22%2C%22num%22%3A18%2C%22current%22%3A72%7D%7D&revision=" + post.Revision +
                                           "&formkey=" + post.FormKey + "&postkey=" + post.PostKey + "&window_id=" +
                                           post.WindowId +
                                           "&referring_controller=user&referring_action=followers&__vcon_json=%5B%22" +
                                           "" + Uri.EscapeDataString(post.VconJson) +
                                           "%22%5D&__vcon_method=increase_count&__e2e_action_id=exz34w8dfi&js_init=%7B%22object_id%22%3A44794736%2C%22initial_count%22%3A18%2C%22buffer_count%22%3A18%2C%22crawler%22%3Afalse%2C%22has_more%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22fixed_size_paged_list%22%3Afalse%2C%22auto_paged%22%3Atrue%7D&__metadata=%7B%7D";
                    _httpHelper.PostRequest(urlServerCallPost, postDataCallPost);
                    var urlPagination =
                        "https://tch678384.tch.quora.com/up/" + post.Chan + "/updates?&callback=" + post.JsonP + "&" +
                        "min_seq=" + post.MinSeq + "&channel=" + post.Channel + "&hash=" + post.Hash + "&_=" +
                        GenerateTimeStamp();


                    objTopicFollowersResponseHandler =
                        quoraFunct.TopicFollowers(JobProcess.DominatorAccountModel, urlPagination);
                    if (objTopicFollowersResponseHandler.TopicFollowers.Count == 0)
                        break;
                    if (!objJobProcessResult.IsProcessCompleted)
                        objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                            objTopicFollowersResponseHandler.TopicFollowers);
                    if (objJobProcessResult.IsProcessCompleted)
                        break;
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

        private JobProcessResult ScraperUserNameWhoAnswered(QueryInfo queryinfo, QuoraUser objQuoraUser)
        {
            var objJobProcessResult = new JobProcessResult();
            try
            {
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                UserFromAnswerOnQuestionResponseHandler userAnswerResponseHandler = null;
                var url = objQuoraUser.Url;
                var PaginationId = quoraFunct.GetAnswerOfQuestionPaginationId(JobProcess.DominatorAccountModel, url, string.Empty).Result;
                if (IsBrowser)
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var response = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    userAnswerResponseHandler = new UserFromAnswerOnQuestionResponseHandler(response, IsBrowser);
                }
                else
                    userAnswerResponseHandler = quoraFunct.UserNameAnsweredOnQuestion(JobProcess.DominatorAccountModel, PaginationId, userAnswerResponseHandler == null ? -1 : userAnswerResponseHandler.PaginationCount);
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                    userAnswerResponseHandler.UserNames);
                if (objJobProcessResult.IsProcessCompleted)
                    return objJobProcessResult;
                StartPaginationForUserWhoAnsweredOnQuestion(queryinfo, userAnswerResponseHandler,PaginationId, ref objJobProcessResult);
            }
            catch (Exception)
            {
            }
            finally { objJobProcessResult.IsProcessCompleted = true; }
            return objJobProcessResult;
        }

        private void StartPaginationForUserWhoAnsweredOnQuestion(QueryInfo queryinfo, UserFromAnswerOnQuestionResponseHandler userAnswerResponse,string PaginationId, ref JobProcessResult objJobProcessResult)
        {
            while(userAnswerResponse!=null && userAnswerResponse.HasMoreResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                userAnswerResponse = quoraFunct.UserNameAnsweredOnQuestion(JobProcess.DominatorAccountModel, PaginationId,userAnswerResponse.PaginationCount);
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                    userAnswerResponse.UserNames);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private JobProcessResult ScrapeQuestionComments(QueryInfo queryinfo, QuoraUser objQuoraUser)
        {
            var objJobProcessResult = new JobProcessResult();
            try
            {
                var url = objQuoraUser.Url;
                CommentOnQuestionResponseHandler commentOnQuestionResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                var PaginationId = quoraFunct.GetAnswerOfQuestionPaginationId(JobProcess.DominatorAccountModel, url, string.Empty).Result;
                if (IsBrowser)
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var response = _browser.ClickOnViewMoreComments(JobProcess.DominatorAccountModel, CommentFor.Question);
                    commentOnQuestionResponseHandler = new CommentOnQuestionResponseHandler(response, IsBrowser);
                }
                else
                    commentOnQuestionResponseHandler = quoraFunct.UserNameCommentOnQuestion(JobProcess.DominatorAccountModel, PaginationId, commentOnQuestionResponseHandler == null ? -1 : commentOnQuestionResponseHandler.PaginationCount);
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                    commentOnQuestionResponseHandler.CommentedUser);
            }
            catch (Exception)
            {
            }
            finally { objJobProcessResult.IsProcessCompleted = true; }
            return objJobProcessResult;
        }

        private JobProcessResult ScrapeWhoCommentOnAnswer(QueryInfo queryinfo, QuoraUser objQuoraUser)
        {
            var objJobProcessResult = new JobProcessResult();
            try
            {
                CommentOnAnswerResponseHandler commentOnAnswerResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                var url = objQuoraUser.Url;
                var AnswerNodeId = string.Empty;
                if (IsBrowser)
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    var response = _browser.ClickOnViewMoreComments(JobProcess.DominatorAccountModel, CommentFor.Answer);
                    commentOnAnswerResponseHandler = new CommentOnAnswerResponseHandler(response, IsBrowser);
                }
                else
                {
                    quoraFunct.GetAnswerId(url, out AnswerNodeId);
                    commentOnAnswerResponseHandler = quoraFunct.UserNameCommentOnAnswer(JobProcess.DominatorAccountModel, AnswerNodeId, commentOnAnswerResponseHandler == null ? -1 : commentOnAnswerResponseHandler.PaginationCount);
                }
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                    commentOnAnswerResponseHandler.CommentedUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { objJobProcessResult.IsProcessCompleted = true; }
            return objJobProcessResult;
        }
    }
}