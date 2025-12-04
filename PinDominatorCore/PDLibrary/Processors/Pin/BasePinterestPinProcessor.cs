using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public abstract class BasePinterestPinProcessor : BasePinterestProcessor
    {
        private int repinByQueryCount = 1;

        protected BasePinterestPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        public void StartProcessForListOfPins(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<string> newPinterestPinsList)
        {
            foreach (var pin in newPinterestPinsList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    PinInfoPtResponseHandler pinInfoPtResponseHandler = null;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (ActivityType != ActivityType.PinScraper)
                            BrowserManager.AddNew(JobProcess.DominatorAccountModel.CancellationSource, $"https://{BrowserManager.Domain}");
                        pinInfoPtResponseHandler = BrowserManager.SearchByCustomPin(JobProcess.DominatorAccountModel, pin,
                         JobProcess.DominatorAccountModel.CancellationSource);
                    }
                    else
                        pinInfoPtResponseHandler = PinFunction.GetPinDetails(pin, JobProcess.DominatorAccountModel);

                    if (pinInfoPtResponseHandler.Success)
                        StartCustomPinProcess(queryInfo, ref jobProcessResult, pinInfoPtResponseHandler);
                    else
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }

                    if (jobProcessResult.IsProcessCompleted)
                        return;
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
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.BrowserWindows.Count > 1)
                        BrowserManager.CloseLast();
                }
        }

        public void StartProcessForListOfPins(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, List<PinterestPin> newPinterestPinsList)
        {
            foreach (var pin in newPinterestPinsList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && ActivityType != ActivityType.Comment
                        && !queryInfo.QueryType.Equals("Custom Pin"))
                        BrowserManager.AddNew(JobProcess.DominatorAccountModel.CancellationSource, $"https://{BrowserManager.Domain}");

                    StartCustomPinProcess(queryInfo, ref jobProcessResult, pin);

                    if (jobProcessResult.IsProcessCompleted)
                        return;
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
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.BrowserWindows.Count >= 1)
                        BrowserManager.CloseLast();
                }
        }

        public void StartCustomPinProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, PinterestPin newPinterestPin)
        {
            try
            {
                var boardUrl = string.Empty;
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateModel = templatesFileManager.GetTemplateById(JobProcess.TemplateId);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (FilterUserActionList.Count > 0)
                {
                    var user = DbAccountService.GetInteractedUsersWithSameQuery(ActivityType + "_UserScrap", queryInfo)
                        .FirstOrDefault(x => x.InteractedUsername == newPinterestPin.User.Username);
                    PinterestUser newPinterestUser = null;
                    if (user == null || user.FullDetailsScraped == null || !user.FullDetailsScraped.Value)
                    {
                        if (user == null)
                            user = new InteractedUsers();
                        newPinterestUser = PinFunction.GetUserDetails(newPinterestPin.User.Username,
                                JobProcess.DominatorAccountModel).Result;

                        user.Bio = newPinterestUser.UserBio;
                        user.FollowingsCount = newPinterestUser.FollowingsCount;
                        user.FollowersCount = newPinterestUser.FollowersCount;
                        user.FullName = newPinterestUser.FullName;
                        user.PinsCount = newPinterestUser.PinsCount;
                        user.IsVerified = newPinterestUser.IsVerified;
                        user.ProfilePicUrl = newPinterestUser.ProfilePicUrl;
                        user.FullDetailsScraped = true;
                        user.HasAnonymousProfilePicture = newPinterestUser.HasProfilePic;
                        user.IsFollowedByMe = newPinterestUser.IsFollowedByMe;
                        user.IsVerified = newPinterestUser.IsVerified;
                        user.InteractedUsername = newPinterestUser.Username;
                        user.InteractedUserId = newPinterestUser.UserId;
                        user.Username = JobProcess.DominatorAccountModel.AccountBaseModel.UserName;
                        user.Website = newPinterestUser.WebsiteUrl;

                        if (user.ActivityType == null)
                        {
                            user.Query = queryInfo.QueryValue;
                            user.QueryType = queryInfo.QueryType;
                            user.ActivityType = ActivityType + "_UserScrap";
                            DbAccountService.Add(user);
                        }
                        else
                        {
                            DbAccountService.Update(user);
                        }
                    }
                    else
                    {
                        newPinterestUser = new PinterestUser();
                        newPinterestUser.UserBio = user.Bio;
                        newPinterestUser.FollowersCount = user.FollowersCount;
                        newPinterestUser.FollowingsCount = user.FollowingsCount;
                        newPinterestUser.FullName = user.FullName;
                        if (user.HasAnonymousProfilePicture != null)
                            newPinterestUser.HasProfilePic = user.HasAnonymousProfilePicture.Value;
                        newPinterestUser.PinsCount = user.PinsCount;
                        newPinterestUser.ProfilePicUrl = user.ProfilePicUrl;
                        newPinterestUser.Username = user.InteractedUsername;
                        newPinterestUser.UserId = user.InteractedUserId;
                        newPinterestUser.WebsiteUrl = user.Website;
                        newPinterestUser.FollowedBack = user.FollowedBack;
                        newPinterestUser.IsFollowedByMe = user.IsFollowedByMe;
                        newPinterestUser.IsVerified = user.IsVerified;
                        newPinterestUser.TriesCount = user.TriesCount;
                    }

                    if (FilterUserApply(newPinterestUser, 1))
                    {
                        user.Filtered = true;
                        if (queryInfo != null && queryInfo?.QueryType?.ToLower() != "keywords")
                        {
                            jobProcessResult.IsProcessCompleted = true;
                            jobProcessResult.HasNoResult = true;
                        }
                        DbAccountService.Update(user);
                        return;
                    }
                }

                if (FilterPinActionList.Count > 0)
                {
                    var pin = DbAccountService.GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo)
                                    .FirstOrDefault(x => x.PinId == newPinterestPin.PinId);

                    if (pin == null)
                        pin = new ScrapPins();
                    if (pin.FullDetailsScraped == null || !pin.FullDetailsScraped.Value)
                    {
                        //if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        //{
                        //    newPinterestPin = BrowserManager.SearchByCustomPin(JobProcess.DominatorAccountModel, newPinterestPin.PinId,
                        //        JobProcess.DominatorAccountModel.CancellationSource);
                        //}
                        //else
                            newPinterestPin = PinFunction.GetPinDetails(newPinterestPin.PinId, JobProcess.DominatorAccountModel,true);

                        pin.CommentCount = newPinterestPin.CommentCount;
                        pin.PinDescription = newPinterestPin.Description;
                        pin.PinWebUrl = newPinterestPin.PinWebUrl;
                        pin.SourceBoardUrl = newPinterestPin.BoardUrl;
                        pin.SourceBoardName = newPinterestPin.BoardName;
                        pin.Username = newPinterestPin.User.Username;
                        pin.UserId = newPinterestPin.User.UserId;
                        pin.MediaType = newPinterestPin.MediaType;
                        pin.PublishedDate = newPinterestPin.PublishDate;
                        pin.MediaString = newPinterestPin.MediaString;
                        pin.PinId = newPinterestPin.PinId;
                        pin.FullDetailsScraped = true;

                        if (pin.ActivityType == null)
                        {
                            pin.QueryValue = queryInfo.QueryValue;
                            pin.QueryType = queryInfo.QueryType;
                            pin.ActivityType = ActivityType + "_Scrap";
                            DbAccountService.Add(pin);
                        }
                        else
                        {
                            DbAccountService.Update(pin);
                        }
                    }
                    else
                    {
                        newPinterestPin.CommentCount = pin.CommentCount;
                        newPinterestPin.Description = pin.PinDescription;
                        newPinterestPin.PinWebUrl = pin.PinWebUrl;
                        newPinterestPin.BoardUrl = pin.SourceBoardUrl;
                        newPinterestPin.BoardName = pin.SourceBoardName;
                        newPinterestPin.User.Username = pin.Username;
                        newPinterestPin.User.UserId = pin.UserId;
                        newPinterestPin.MediaType = pin.MediaType;
                        newPinterestPin.PublishDate = pin.PublishedDate;
                        newPinterestPin.MediaString = pin.MediaString;
                    }

                    if (FilterPinApply(newPinterestPin, 1))
                    {
                        pin.Filtered = true;
                        DbAccountService.Update(pin);
                        return;
                    }
                }

                if (ActivityType == ActivityType.Repin)
                {
                    var rePinModel = JsonConvert.DeserializeObject<RePinModel>(templateModel.ActivitySettings);
                    var activity = ActivityType.ToString();
                    var pinWithDiffBoards = DbAccountService.Get<InteractedPosts>(x => x.PinId == newPinterestPin.PinId
                                                                                       && x.OperationType == activity).Select(x => x.DestinationBoard).Distinct().ToList();

                    var boardsToBeReppinedWithCampaign = new List<string>();
                    if (!rePinModel.IsSelectPinsFromBoard)
                    {
                        boardsToBeReppinedWithCampaign = CampaignService.Get<InteractedPosts>()
                              .Select(x => x.DestinationBoard).ToList();
                        boardsToBeReppinedWithCampaign.RemoveAll(x => x == null);
                        boardsToBeReppinedWithCampaign = boardsToBeReppinedWithCampaign.Distinct().ToList();
                    }

                    var query = queryInfo.QueryType + " [" + queryInfo.QueryValue + "]";
                    var boardUrls = rePinModel.AccountPagesBoardsPair.Where(x =>
                        x.LstofPinsToRepin.Value.Any(y => y.Content == query && y.IsContentSelected) &&
                        x.AccountId == JobProcess.AccountId).Select(x => x.LstofPinsToRepin.Key).ToList();

                    if (boardUrls == null || boardUrls.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyselectAtleastOneBoardToRepinForQuery".FromResourceDictionary(), query));
                        jobProcessResult.IsProcessCompleted = true;
                        jobProcessResult.HasNoResult = true;
                        return;
                    }

                    if (!rePinModel.IsChkRepinOnTheSameBoard)
                        boardsToBeReppinedWithCampaign.ForEach(x => { boardUrls.Remove(x); });

                    if (boardUrls.Count == 0)
                        boardUrls = rePinModel.AccountPagesBoardsPair.Where(x =>
                            x.LstofPinsToRepin.Value.Any(y => y.Content == query && y.IsContentSelected) &&
                            x.AccountId == JobProcess.AccountId).Select(x => x.LstofPinsToRepin.Key).ToList();

                    boardUrls.Shuffle();
                    bool isBoardLimitReached = false;
                    foreach (var board in boardUrls)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        newPinterestPin.LstSection = rePinModel.AccountPagesBoardsPair.Where(x =>x.LstSection.Key==board)?.FirstOrDefault(y=>y.AccountId==JobProcess.AccountId && y.IsSelected && y.LstSection.Key==board)?.LstSection.Value;
                        int repinCountOnSameBoard = 1;
                        if (rePinModel.IsChkRepinOnTheSameBoard)
                        {
                            int alreadyRepinCountOnSameBoard = NumberOfTimesRepinOnSameBoard(queryInfo, newPinterestPin.PinId, board);
                            repinCountOnSameBoard = rePinModel.NumberOfTimesToRepinOnSameBoard - alreadyRepinCountOnSameBoard;
                            if (repinCountOnSameBoard <= 0)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                               JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                               String.Format("LangKeyPinHasReachedLimitTimesWithBoard".FromResourceDictionary(), newPinterestPin.PinId, rePinModel.NumberOfTimesToRepinOnSameBoard, board));
                                continue;
                            }
                        }
                        else if (AlreadyInteractedPin(queryInfo, newPinterestPin.PinId, board))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeyAlreadyInteractedPinWithBoard".FromResourceDictionary(), newPinterestPin.PinId, board));
                            continue;
                        }

                        for (int i = 0; i < repinCountOnSameBoard; i++)
                        {
                            Thread.Sleep(15000);
                            pinWithDiffBoards = DbAccountService.Get<InteractedPosts>(x => x.PinId == newPinterestPin.PinId
                                                                                       && x.OperationType == activity).Select(x => x.DestinationBoard).Distinct().ToList();
                            boardUrl = board;

                            if (!string.IsNullOrEmpty(boardUrl))
                                newPinterestPin.BoardUrlToRepin = boardUrl;

                            var NoOfBoardsToBeRepined = DbAccountService.Get<InteractedPosts>(x =>
                                x.PinId == newPinterestPin.PinId
                                && x.OperationType == activity).Select(x => x.DestinationBoard).Distinct().ToList().Count;
                            if (rePinModel.IsChkRepinToNumberOfBoards &&
                                NoOfBoardsToBeRepined >= rePinModel.NumberOfBoardsToRepin && !rePinModel.IsChkRepinOnTheSameBoard ||
                                (rePinModel.IsChkRepinToNumberOfBoards && NoOfBoardsToBeRepined >= rePinModel.NumberOfBoardsToRepin &&
                                !pinWithDiffBoards.Contains(boardUrl)))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    String.Format("LangKeyPinHasReachedLimitBoardsFromAPin".FromResourceDictionary(), newPinterestPin.PinId, rePinModel.NumberOfBoardsToRepin));
                                isBoardLimitReached = true;
                                break;
                            }

                            if (!CheckPostUniqueNess(jobProcessResult, newPinterestPin)) continue;

                            if (!JobProcess.ModuleSetting.IschkUniquePostForCampaign
                                && !ApplyCampaignLevelSettings(queryInfo, newPinterestPin.PinId, JobProcess.CampaignDetails)) continue;

                            StartFinalProcess(ref jobProcessResult, newPinterestPin, queryInfo);
                        }
                        if (isBoardLimitReached)
                            break;

                        if (rePinModel.RepinByQueryWithLimit && ++repinByQueryCount > rePinModel.RepinByQueryValue)
                        {
                            jobProcessResult.IsProcessCompleted = true;
                            jobProcessResult.HasNoResult = true;
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeyRepingPerAccountLimitReachedOnQuery").FromResourceDictionary() + $" {queryInfo.QueryValue} " +
                                "\n" + string.Format("LangKeySearchingForNextQueryIfAny").FromResourceDictionary());
                            return;
                        }
                    }
                }

                else if (ActivityType == ActivityType.EditPin)
                {
                    var editPinModel = JsonConvert.DeserializeObject<EditPinModel>(TemplateModel.ActivitySettings);
                    var pinInfo = editPinModel.PinDetails.FirstOrDefault(pin =>
                        pin.Account == JobProcess.DominatorAccountModel.UserName);
                    if (pinInfo != null)
                    {
                        newPinterestPin.BoardName = !string.IsNullOrEmpty(pinInfo.Board)
                            ? pinInfo.Board
                            : newPinterestPin.BoardName;
                        newPinterestPin.PinWebUrl = !string.IsNullOrEmpty(pinInfo.WebsiteUrl)
                            ? pinInfo.WebsiteUrl
                            : newPinterestPin.PinWebUrl;
                        newPinterestPin.Description = !string.IsNullOrEmpty(pinInfo.PinDescription)
                            ? pinInfo.PinDescription
                            : newPinterestPin.Description;
                        newPinterestPin.PinName = !string.IsNullOrEmpty(pinInfo.Title)
                            ? pinInfo.Title
                            : newPinterestPin.PinName;
                    }
                }
                else if (ActivityType == ActivityType.Comment)
                {
                    if (!CheckPostUniqueNess(jobProcessResult, newPinterestPin)) return;

                    if (!JobProcess.ModuleSetting.IschkUniquePostForCampaign
                        && !ApplyCampaignLevelSettings(queryInfo, newPinterestPin.PinId, JobProcess.CampaignDetails)) return;

                    var commentModel =
                        JsonConvert.DeserializeObject<CommentModel>(TemplateModel.ActivitySettings);
                    if (commentModel.IsChkAllowMultipleCommentsOnSamePost)
                    {
                        foreach (var comment in commentModel.LstDisplayManageCommentModel)
                        {
                            var isCommented = false;
                            var alreadyCommentedWithSamePost = DbAccountService.Get<InteractedPosts>(x =>
                                x.PinId == newPinterestPin.PinId);

                            if (alreadyCommentedWithSamePost.Any(y => y.Comment == comment.CommentText))
                                isCommented = true;

                            if (!isCommented)
                                StartFinalProcess(ref jobProcessResult, newPinterestPin, queryInfo);
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    String.Format("LangKeySkipedAlreadyInteractedPinWithComment".FromResourceDictionary(), newPinterestPin.PinId, comment.CommentText));
                        }
                    }
                    else
                    {
                        if (!AlreadyInteractedPin(queryInfo, newPinterestPin.PinId))
                            StartFinalProcess(ref jobProcessResult, newPinterestPin, queryInfo);
                    }
                }

                else
                {
                    StartFinalProcess(ref jobProcessResult, newPinterestPin, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StartFinalProcess(ref JobProcessResult jobProcessResult, PinterestPin newPinterestPin,
            QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = newPinterestPin,
                QueryInfo = queryInfo
            });
        }

        public bool IsAccountSelectedForRepinQuery(QueryInfo queryInfo)
        {
            try
            {
                var repinModel = JsonConvert.DeserializeObject<RePinModel>(TemplateModel.ActivitySettings);

                var query = queryInfo.QueryType + " [" + queryInfo.QueryValue + "]";
                var boardUrls = repinModel.AccountPagesBoardsPair.Where(x =>
                    x.LstofPinsToRepin.Value.Any(y => y.Content == query && y.IsContentSelected) &&
                    x.AccountId == JobProcess.AccountId).Select(x => x.LstofPinsToRepin.Key).ToList();

                if (boardUrls.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        string.Format("LangKeyQueryType".FromResourceDictionary()) + $" - {queryInfo.QueryType}, "
                        + string.Format("LangKeyQueryValue".FromResourceDictionary()) + $" - {queryInfo.QueryValue} "
                        + string.Format("LangKeyIsNotSelecteedForThisAccount".FromResourceDictionary()));

                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }
    }
}