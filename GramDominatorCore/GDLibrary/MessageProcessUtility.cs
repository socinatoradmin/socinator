using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using NReco.VideoConverter;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace GramDominatorCore.GDLibrary
{
    public class MessageProcessUtility
    {
        public string MakeMentionInMessage(string message, InstagramUser instagramUser, DominatorAccountModel DominatorAccountModel, ActivityType ActivityType)
        {
            var mention = message.Replace("[Full Name]", instagramUser.FullName)
                .Replace("[User Name]", "@" + instagramUser.Username);
            if (message.Contains("[Full Name]"))
            {
                if (string.IsNullOrEmpty(instagramUser.FullName))
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Full Name is not avilable in user Account");
            }
            return mention;
        }

        public SendMessageIgResponseHandler SendMessageResponse(DominatorAccountModel DominatorAccountModel, AccountModel AccountModel,
            InstagramUser instagramUser, string message, string mediaPath, IInstaFunction instaFunct, string threadId,
            CancellationTokenSource JobCancellationTokenSource,List<string>Medias=null,bool SkipAlreadyReceivedMessage=false)
        {
            SendMessageIgResponseHandler response = null;
            try
            {
                var browser = GramStatic.IsBrowser;
                if (message.Contains("@" + instagramUser.Username))
                {
                    message = message?.Replace("@" + instagramUser.Username, instagramUser.Username);
                }
                if (!string.IsNullOrEmpty(message) || (Medias != null && Medias.Count > 0))
                {
                    {
                        {
                            if (browser)
                            {
                                if (string.IsNullOrEmpty(threadId))
                                {
                                    var userInfo = instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token);
                                    threadId = userInfo.ThreadId;
                                }
                                response = instaFunct.GdBrowserManager.SendMessage(DominatorAccountModel, instagramUser, threadId, message,mediaPath, JobCancellationTokenSource.Token, Medias,SkipAlreadyReceivedMessage:SkipAlreadyReceivedMessage);
                            }
                            else
                            {
                                InstagramUser user = instaFunct.SearchUserInfoById(DominatorAccountModel, new AccountModel(DominatorAccountModel), instagramUser.UserId ?? instagramUser.Pk, JobCancellationTokenSource.Token).Result;
                                if (string.IsNullOrEmpty(threadId))
                                {
                                    var threadIDDetails = instaFunct.GetThreadID(DominatorAccountModel, instagramUser.UserId ?? instagramUser.Pk, instagramUser.Username, false).Result;
                                    if (user != null && !user.CanMessage)
                                        return new SendMessageIgResponseHandler(new ResponseParameter { Response = "User is not allowed to message" }, "User is not allowed to message");
                                    threadId = threadIDDetails?.ThreadId;
                                }
                                if (Medias != null && Medias.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(message))
                                        response = instaFunct.SendMessage(DominatorAccountModel, AccountModel, instagramUser.Pk, message, threadId, JobCancellationTokenSource.Token).Result;

                                    if (response != null && response.Success)
                                    {
                                        foreach (var mention in Medias)
                                        {
                                            response = instaFunct.SendPhotoAsDirectMessage(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.Pk, mention, response?.ThreadId).Result;
                                        }
                                    }

                                }
                                else
                                {
                                    response = instaFunct.SendMessage(DominatorAccountModel, AccountModel, instagramUser.Pk, message, threadId, JobCancellationTokenSource.Token).Result;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return response;
        }

        public SendMessageIgResponseHandler MediaSendMessageResponse(DominatorAccountModel DominatorAccountModel, AccountModel AccountModel,
            string userid, string userName, string mediaPath, IInstaFunction instaFunct, ActivityType activity,
            CancellationToken token)
        {
            SendMessageIgResponseHandler sendPhotoResponse = null;
            UploadMediaResponse response = null;
            try
            {
                if (mediaPath.Contains(".mp4"))
                {
                    string thumbnailFilePath = string.Empty;
                    string convertedMediaFilePath = string.Empty;
                    if (SetThumbnailAndVideoFormat(mediaPath, ref thumbnailFilePath, ref convertedMediaFilePath, userName))
                        response = instaFunct.SendVideoMessage(DominatorAccountModel, AccountModel, mediaPath, thumbnailFilePath, userid, token);

                    if (response != null && response.Success)
                    {
                        sendPhotoResponse = new SendMessageIgResponseHandler(new ResponseParameter { Response = response.ToString() });
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, activity, $"video File has been sent to user {userName}");
                    }

                }
                else
                {
                    sendPhotoResponse = instaFunct.IgSendUploadPhoto(DominatorAccountModel, AccountModel, token, userid, mediaPath);
                    if ((sendPhotoResponse == null || !sendPhotoResponse.Success) && sendPhotoResponse.Issue != null && sendPhotoResponse.Issue.Error.ToString() != "Challenge")
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(7));
                        Image image = Image.FromFile(mediaPath);
                        string filePaths = mediaPath.Split('.')[0];
                        string newfilePath = $"{filePaths}.jpeg";
                        string newpath = ImageHelper.GetPnGtoJpegFormat(newfilePath, image);
                        sendPhotoResponse = instaFunct.SendPhotoAsDirectMessage(DominatorAccountModel, AccountModel, token, userid, newpath).Result;
                    }
                }
                //sendPhotoResponse = instaFunct.SendPhotoAsDirectMessage(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.Pk, mediaPath);  
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return sendPhotoResponse;
        }

        public void AddMessageDataToDataBase(ScrapeResultNew scrapeResult, string message, AccountModel AccountModel
            , DominatorAccountModel DominatorAccountModel, IDbOperations CampaignDbOperation, IDbOperations AccountDbOperation, ActivityType ActivityType
            , string CampaignId, string threadId, CancellationTokenSource JobCancellationTokenSource)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;

            // Add data to respected campaign InteractedUsers table
            try
            {
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                    {
                        ActivityType = ActivityType.ToString(),
                        Date = DateTimeUtilities.GetEpochTime(),
                        DirectMessage = message,
                        InteractedUsername = instagramUser.Username,
                        InteractedUserId = instagramUser.Pk ?? instagramUser.UserId,
                        Username = DominatorAccountModel.AccountBaseModel.UserName
                    });
                }
                AccountDbOperation.Add(
                 new InteractedUsers()
                 {
                     ActivityType = ActivityType.ToString(),
                     Date = DateTimeUtilities.GetEpochTime(),
                     DirectMessage = message,
                     InteractedUsername = instagramUser.Username,
                     InteractedUserId = instagramUser.Pk ?? instagramUser.UserId,
                     Username = DominatorAccountModel.AccountBaseModel.UserName
                 });
                AccountDbOperation.Add(
                new UserConversation()
                {
                    ActivityType = ActivityType,
                    Date = Convert.ToString(DateTimeUtilities.GetEpochTime()),
                    SenderName = instagramUser.Username,
                    SenderId = instagramUser.UserId ?? instagramUser.Pk,
                    ThreadId = threadId,
                    ConversationType = "Software"
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public bool SetThumbnailAndVideoFormat(string mediaPath, ref string thumbnailFilePath, ref string convertedMediaFilePath, string userName)
        {
            try
            {
                thumbnailFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)}.jpg";
                convertedMediaFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)} Compressed.mp4";

                //FFMpegConverter ffMpegConverter = new FFMpegConverter();
                //FFProbe ffProbe = new FFProbe();
                //var mediaInfo = ffProbe.GetMediaInfo(mediaPath);
                var mediaInfo = Utility.GramStatic.GetMediaInfo(mediaPath);

                if (mediaInfo.Duration.TotalSeconds > 70)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, userName, "Reposter",
                        $"Instagram only allows to upload video size max: 60 secs. Got size {mediaInfo.Duration.TotalSeconds} secs");
                    return false;
                }
                thumbnailFilePath = Utility.GramStatic.GetVideoThumb(mediaPath, convertedMediaFilePath);
                //ffMpegConverter.GetVideoThumbnail(
                //    (!File.Exists(convertedMediaFilePath) ? mediaPath : convertedMediaFilePath), thumbnailFilePath, 3);
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
    }
}
