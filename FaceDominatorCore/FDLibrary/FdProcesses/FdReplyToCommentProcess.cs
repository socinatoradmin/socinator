using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThreadUtils;
using AccountFriends = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.Friends;
using AccountInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments;
using CampaignInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdReplyToCommentProcess : FdJobProcessInteracted<AccountInteractedComments>
    {
        public ReplyToCommentModel ReplyToCommentsModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public Dictionary<string, string> AccountCommentPair { get; set; } = new Dictionary<string, string>();

        private readonly IFdRequestLibrary _fdRequestLibrary;
        private readonly IDelayService _delayService;

        public FdReplyToCommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            ReplyToCommentsModel = processScopeModel.GetActivitySettingsAs<ReplyToCommentModel>();
            _fdRequestLibrary = fdRequestLibrary;
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            bool processSuccess = false;

            QueryInfo objQueryInfo = scrapeResult.QueryInfo;

            string usedMention = string.Empty;

            FdPostCommentDetails objFacebookPostDetails = (FdPostCommentDetails)scrapeResult.ResultComment;

            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

            IResponseHandler commentOnPostResponseHandler = null;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();



                var replyForComment = GetCommentDetailsNew(objQueryInfo);

                var mentionDictionary = new Dictionary<string, string>();

                if (ReplyToCommentsModel.IsMentionUsersChecked)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        mentionDictionary = CommentWithMentionsBrowser(ref usedMention);
                    }
                    else
                        replyForComment = CommentWithMentions(replyForComment, ref usedMention);
                }


                var isChangedActor = objFanpageDetails != null
                        && !AccountModel.IsRunProcessThroughBrowser
                        && _fdRequestLibrary.ChangeActor(AccountModel, objFacebookPostDetails.PostId, objFanpageDetails.FanPageID);

                commentOnPostResponseHandler = !AccountModel.IsRunProcessThroughBrowser && (objFanpageDetails == null || isChangedActor)
                       ? _fdRequestLibrary.ReplyOnPost(AccountModel, objFacebookPostDetails.PostId, replyForComment, objFacebookPostDetails.CommentId)
                       : FdLogInProcess._browserManager.ReplyToComments(AccountModel, objFacebookPostDetails, replyForComment, objFanpageDetails?.FanPageID, objFanpageDetails, mentionDictionary, ReplyToCommentsModel.IsMentionCommentedUserChecked);


                if (!mentionDictionary.ContainsKey(objFacebookPostDetails.CommenterID) && ReplyToCommentsModel.IsMentionCommentedUserChecked)
                {
                    if (string.IsNullOrEmpty(usedMention)) usedMention = objFacebookPostDetails.CommenterName;
                    else usedMention = usedMention + "," + objFacebookPostDetails.CommenterName;

                }

                if (commentOnPostResponseHandler != null && commentOnPostResponseHandler.Status)
                {
                    processSuccess = true;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.CommentId);
                }
                else if (commentOnPostResponseHandler != null && commentOnPostResponseHandler.ObjFdScraperResponseParameters.IsBlocked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Activity blocked by facebook. Stopping campaign!");
                    Stop();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.CommenterID, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (processSuccess)
                {
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    AddCommentScraperDataToDatabase(scrapeResult, usedMention, replyForComment);
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private Dictionary<string, string> CommentWithMentionsBrowser(ref string usedMentions)
        {
            string mentionsUsed = string.Empty;
            Dictionary<string, string> mentionUserList = new Dictionary<string, string>();
            try
            {
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);

                var randomFriends = objFdFunctions.MentionFriends(AccountModel, ReplyToCommentsModel.SelectOptionModel);

                randomFriends.RemoveAll(x => x == "");

                int noOfMentions = ReplyToCommentsModel.NoOfUserMention.GetRandom();

                if (noOfMentions == 0)
                {
                    noOfMentions = ReplyToCommentsModel.NoOfUserMention.GetRandom();
                }

                if (randomFriends.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Getting friend details for mention!");
                }

                randomFriends.ForEach(x =>
                {
                    var friendId = _fdRequestLibrary.GetFriendUserId(AccountModel, x);
                    ReplyToCommentsModel.SelectOptionModel.ListCustomDetailsUrl.Remove(x);
                    ReplyToCommentsModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(AccountModel.AccountId, $"{FdConstants.FbHomeUrl}{friendId}"));
                    Thread.Sleep(1000);
                });

                randomFriends = ReplyToCommentsModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Where(x => x.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                Random random = new Random();

                randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfMentions).ToList();

                randomFriends.ForEach(x =>
                {
                    var friendId = Utilities.GetBetween(x, FdConstants.FbHomeUrl, "\"");
                    friendId = string.IsNullOrEmpty(friendId) ? x.Split('/').LastOrDefault() : friendId;
                    var friendDetails = ObjDbAccountService.GetSingle<AccountFriends>(y => y.FriendId == friendId);
                    if (!mentionUserList.ContainsKey(friendId))
                        mentionUserList.Add(friendId, friendDetails?.FullName);
                    mentionsUsed = friendDetails?.FullName + ",";
                });
                usedMentions = mentionsUsed.Remove(mentionsUsed.Length - 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return mentionUserList;
        }

        private string GetCommentDetailsNew(QueryInfo objQueryInfo)
        {
            string commentDetails = string.Empty;

            QueryInfo queryInfo = new QueryInfo
            {
                QueryType = objQueryInfo.QueryType,
                QueryValue = objQueryInfo.QueryValue
            };



            List<string> lstComment = new List<string>();

            bool isCommentAdded = false;

            try
            {
                ReplyToCommentsModel.LstManageCommentModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryType == queryInfo.QueryType && y.Content.QueryValue == queryInfo.QueryValue)
                        {
                            if (lstComment.All(z => z != x.CommentText))
                            {
                                lstComment.Add(x.CommentText);
                            }
                        }
                    });
                });

                foreach (var currentMessage in lstComment)
                {
                    commentDetails = currentMessage;

                    var messageCollection = new List<string>();

                    var messages = new List<string>();

                    var messageText = string.Empty;

                    if (ReplyToCommentsModel.IsSpintaxChecked)
                    {
                        messages = SpinTexHelper.GetSpinMessageCollection(commentDetails);

                        foreach (var message in messages)
                        {
                            messageText = message.Trim();
                            messageCollection.Add(messageText);
                        }
                        if (AccountCommentPair.Count >= messageCollection.Count)
                            AccountCommentPair.Clear();
                    }
                    else
                    {
                        messageCollection.Add(currentMessage);
                        if (AccountCommentPair.Count >= lstComment.Count)
                            AccountCommentPair.Clear();
                    }

                    messageCollection.Shuffle();
                    messageCollection.Shuffle();

                    foreach (var comment in messageCollection)
                    {
                        try
                        {
                            if (AccountCommentPair.Any(z =>
                                z.Key == comment))
                            {
                            }
                            else
                            {
                                commentDetails = comment;

                                AccountCommentPair.Add($"{comment}", AccountModel.AccountId);

                                isCommentAdded = true;

                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    if (isCommentAdded)
                    {
                        break;
                    }
                }

                return commentDetails;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commentDetails;
        }




        /*
                private void GetReactionDetails(ref ReactionType objReactionType)
                {
                    if (!Enum.IsDefined(typeof(ReactionType), objReactionType))
                        throw new InvalidEnumArgumentException(nameof(objReactionType), (int) objReactionType,
                            typeof(ReactionType));

                    var listReactionType = ReplyToCommentsModel.LikerCommentorConfigModel.ListReactionType;

                    if (listReactionType.Count > 0)
                    {
                        var random = new Random();

                        int index = random.Next(ReplyToCommentsModel.LikerCommentorConfigModel.ListReactionType.Count);

                        objReactionType = ReplyToCommentsModel.LikerCommentorConfigModel.ListReactionType[index];
                    }

                    else
                    {
                        objReactionType = ReplyToCommentsModel.LikerCommentorConfigModel.ListReactionType[0];
                    }
                }
        */

        private string CommentWithMentions(string comment, ref string mentionUsed)
        {
            var mention = string.Empty;

            FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);

            try
            {

                string usedMentions = string.Empty;

                var randomFriends = objFdFunctions.MentionFriends(AccountModel, ReplyToCommentsModel.SelectOptionModel);

                randomFriends.RemoveAll(x => x == "");

                int noOfMentions = ReplyToCommentsModel.NoOfUserMention.GetRandom();

                if (randomFriends.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Getting friend details for mention!");
                }

                randomFriends.ForEach(x =>
                {
                    var friendId = _fdRequestLibrary.GetFriendUserId(AccountModel, x);
                    ReplyToCommentsModel.SelectOptionModel.ListCustomDetailsUrl.Remove(x);
                    ReplyToCommentsModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(AccountModel.AccountId, $"{FdConstants.FbHomeUrl}{friendId}"));
                    _delayService.ThreadSleep(1000);
                });

                randomFriends = ReplyToCommentsModel.SelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Where(x => x.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                Random random = new Random();

                randomFriends = randomFriends.OrderBy(x => random.Next()).Take(noOfMentions).ToList();

                randomFriends.ForEach(x =>
                {
                    var friendId = FdFunctions.FdFunctions.GetIntegerOnlyString(x);
                    var friendDetails = ObjDbAccountService.GetSingle<AccountFriends>(y => y.FriendId == friendId);
                    mention += $"@[{friendId}:{friendDetails.FullName}] ";

                    usedMentions = mention + ",";

                });

                if (!string.IsNullOrEmpty(usedMentions))
                    usedMentions = usedMentions.Remove(usedMentions.Length - 1);

                if (!string.IsNullOrEmpty(usedMentions))
                {
                    comment = usedMentions + comment;
                    mentionUsed = usedMentions;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return comment;
        }


        private void AddCommentScraperDataToDatabase(ScrapeResultNew scrapeResult, string usedMention, string reply)
        {
            try
            {

                FdPostCommentDetails commenDetail = (FdPostCommentDetails)scrapeResult.ResultComment;


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedComments
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        CommentUrl = commenDetail.CommentUrl,
                        CommentId = commenDetail.CommentId,
                        CommentText = commenDetail.CommentText,
                        CommenterId = commenDetail.CommenterID,
                        CommentPostId = commenDetail.PostId,
                        CommentTimeWithDate = commenDetail.CommentTimeWithDate,
                        CommetLikeCount = commenDetail.ReactionCountOnComment,
                        HasLikedByUser = commenDetail.HasLikedByUser,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        Mentions = usedMention,
                        ReplyForComment = reply

                    });
                }

                DbAccountService.Add(new AccountInteractedComments
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentUrl = commenDetail.CommentUrl,
                    CommentId = commenDetail.CommentId,
                    CommentText = commenDetail.CommentText,
                    CommenterId = commenDetail.CommenterID,
                    CommentPostId = commenDetail.PostId,
                    CommentTimeWithDate = commenDetail.CommentTimeWithDate,
                    CommetLikeCount = commenDetail.ReactionCountOnComment,
                    HasLikedByUser = commenDetail.HasLikedByUser,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    Mentions = usedMention,
                    ReplyForComment = reply
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
