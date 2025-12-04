using CefSharp;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuoraDominatorCore.QdLibrary
{
    internal class AnswerOnQuestionProcess : QdJobProcessInteracted<InteractedAnswers>
    {
        private readonly AnswerQuestionModel _answerQuestionModel;
        private IQuoraBrowserManager _browser;
        private readonly SocialNetworks _networks;

        private readonly IQuoraFunctions quoraFunct;
        private readonly IQdHttpHelper httpHelper;
        private IQDBrowserManagerFactory managerFactory;
        public AnswerOnQuestionProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            quoraFunct = qdFunc;
            _answerQuestionModel = processScopeModel.GetActivitySettingsAs<AnswerQuestionModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
            httpHelper = qdHttpHelper;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            _browser = managerFactory.QdBrowserManager();
            var jobProcessResult = new JobProcessResult();
            if (jobProcessResult.IsProcessCompleted)
                return jobProcessResult;
            if (_answerQuestionModel.LstManageCommentModel.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.AnswerOnQuestions, "Answer is empty");
                return jobProcessResult;
            }

            try
            {
                #region
                var quoraUser = (QuoraUser)scrapeResult.ResultUser;
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.AnswerOnQuestions, "Trying To Answer On Question " + quoraUser.Url);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (ImageExtracter.IsValidUrl(quoraUser.Url) && !quoraUser.Url.Contains("www.quora.com"))
                {
                    var objQuestionDetailsResponseHandler =
                        quoraFunct.QuestionDetails(DominatorAccountModel, quoraUser.Url);
                    quoraUser.Url = objQuestionDetailsResponseHandler.QuestionUrl;
                }
                var lstComment = new List<string>();
                var newComment = "";
                try

                {
                    _answerQuestionModel.LstManageCommentModel.ForEach(x =>
                    {
                        x.SelectedQuery.ForEach(y =>
                        {
                            if (y.Content.QueryType == scrapeResult.QueryInfo.QueryType &&
                                y.Content.QueryValue == scrapeResult.QueryInfo.QueryValue)
                                if (lstComment.All(z => z != x.CommentText))
                                {
                                    newComment = x.CommentText;
                                    lstComment.Add(newComment);
                                }
                        });
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                if(lstComment.Count == 0)
                {
                    newComment = _answerQuestionModel.LstManageCommentModel.FirstOrDefault().CommentText;
                }
                string resp = string.Empty;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    resp = _browser.SearchByCustomUrl(DominatorAccountModel, quoraUser.Url).Response;
                if (DominatorAccountModel.IsRunProcessThroughBrowser && (resp.Contains("You've written an answer") ||
                    resp.Contains("You can edit or delete it at anytime.") || resp.Contains("Quora deleted this answer")))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Already Answered on this " + quoraUser.Url);
                    jobProcessResult.IsProcessSuceessfull = false;
                    throw new Exception();
                }

                var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var regex = new Regex(@"^(www.|[a-zA-Z].)[a-zA-Z0-9\-\.]+\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&amp;%\$#\=~_\-]+))*$");
                var matches = linkParser.Matches(newComment);
                var matches1 = Regex.Matches(newComment, "^http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?$");
                var IsSuccess = false;
                var ErrorMessage = string.Empty;
                if(_answerQuestionModel.IsSpintaxChecked)
                    newComment = " " + SpinTexHelper.GetSpinMessageCollection(newComment).GetRandomItem() + " ";
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var AnswerResponse = quoraFunct.AnswerOnQuestion(DominatorAccountModel, _answerQuestionModel, quoraUser.Url, newComment).Result;
                    IsSuccess = AnswerResponse.Success;
                    ErrorMessage = AnswerResponse.Message;
                }
                else
                    IsSuccess = _browser.AnswerOnQuestion(DominatorAccountModel, _answerQuestionModel, quoraUser.Url, newComment);
                if (IsSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                    AddAnswerUrlToDataBase(quoraUser, scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                    IncrementCounters();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,ErrorMessage);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();

                #endregion
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }


        private void AddAnswerUrlToDataBase(QuoraUser quorauser, ScrapeResultNew scrapeResult)
        {
            try
            {
                #region Add to Account DB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(
                    new InteractedAnswers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AnswersUrl = quorauser.Url,
                        Accountusername = DominatorAccountModel.UserName
                    });

                #endregion

                #region Add to campaign Db

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);

                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedAnswers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AnswersUrl = quorauser.Url,
                        Accountusername = DominatorAccountModel.UserName
                    });
                }

                #endregion

                #region Add to PrivateBlacklist DB
                //if (_answerQuestionModel.IsChkAnswerScraperPrivateList)
                //    dbAccountService.Add(
                //        new PrivateBlacklist
                //        {
                //            UserName = quorauser.Username,
                //            UserId = quorauser.UserId,
                //            InteractionTimeStamp = GetEpochTime()
                //        });

                //#endregion

                //#region Add to GroupBlacklist DB

                //if (_answerQuestionModel.IsChkAnswerScraperGroupList)
                //{
                //    IDbGlobalService dbGlobalService = new DbGlobalService();
                //    dbGlobalService.Add(new BlackListUser
                //    {
                //        UserName = quorauser.Username,
                //        UserId = quorauser.UserId,
                //        AddedDateTime = DateTime.Now
                //    });
                //    GlobusLogHelper.log.Info("{0}\t {1}\t Add to Blacklisted " + quorauser.Username,
                //        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                //        DominatorAccountModel.AccountBaseModel.UserName);
                //}

                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("{0}\t {1}\t could not add to Blacklist " + quorauser.Username,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName);
                ex.DebugLog();
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
        }
    }
}