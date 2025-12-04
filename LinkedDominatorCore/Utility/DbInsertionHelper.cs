using System;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDModel.LDUtility;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json;
using InteractedUsers = DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedUsers;

namespace LinkedDominatorCore.Utility
{
    public interface IDbInsertionHelper
    {
        void DatabaseInsertionPost(ScrapeResultNew scrapeResult, LinkedinPost objLinkedinPost);

        void RemoveConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            RemoveConnectionModel removeOrWithdrawConnectionsModel);

        void WithdrawConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            WithdrawConnectionRequestModel withdrawConnectionRequestModel);

        void AutoReplyToNewMessage(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string message);

        void SendGreetingsToConnections(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedInfo);

        void SendMessageToNewConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string message);

        void SalesNavCompany(ScrapeResultNew scrapeResult, ICompany objLinkedinCompany,
            string detailedInfo);

        void BroadcastMessages(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string message, string status);

        void ExportConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string contactInfo);

        void ProfileEndorsement(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string endorsedSkillsCollection);

        void SalesNavUser(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedUserInfoJasonString);

        void ConnectionRequest(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string finalPersonalNote);

        void Followpages(ScrapeResultNew scrapeResult, LinkedinPage objLinkedinpage);


        void JobScraper(ScrapeResultNew scrapeResult, LinkedinJob objLinkedinJob, string detailedInfo);
        void GroupJoiner(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup);
        void GroupInviter(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup);
        void GroupUnJoiner(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup);
        void AcceptConnectionRequest(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser);

        void CompanyScraper(ScrapeResultNew scrapeResult, ICompany objLinkedinCompany,
            string detailedInfo);

        void UserScraper(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedUserInfoJasonString);
    }

    public class DbInsertionHelper : IDbInsertionHelper
    {
        private readonly string _activityType;

        private readonly string _campaignId;

        // private readonly IDbOperations _dbOperations;
        private readonly IDbAccountService _dbAccountService;

        private readonly IDbCampaignService _dbCampaignService;
        private readonly IDelayService _delayService;
        public readonly DominatorAccountModel AccountModel;


        public DbInsertionHelper(IDbAccountServiceScoped dbAccountService, IProcessScopeModel processScopeModel,
            IDbCampaignService campaignService, IDelayService delayService)
        {
            _delayService = delayService;
            try
            {
                _campaignId = processScopeModel.CampaignId;
                _activityType = processScopeModel.ActivityType.ToString();
                AccountModel = processScopeModel.Account;
                _dbAccountService =
                    dbAccountService;
                _dbCampaignService = campaignService;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void DatabaseInsertionPost(ScrapeResultNew scrapeResult, LinkedinPost objLinkedinPost)
        {
            try
            {
                var objAccountInteractedPosts = new InteractedPosts
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    PostOwnerFullName = objLinkedinPost.FullName,
                    PostOwnerProfileUrl = objLinkedinPost.ProfileUrl,
                    ConnectionType = objLinkedinPost.ConnectionType,
                    PostedDateTime = objLinkedinPost.PostedTime.EpochToDateTimeUtc().ToLocalTime(),
                    MediaType = objLinkedinPost.MediaType,
                    PostLink = objLinkedinPost.PostLink,
                    PostTitle = objLinkedinPost.PostTitle ?? "N/A",
                    PostDescription = objLinkedinPost.Caption,
                    LikeCount = objLinkedinPost.LikeCount,
                    CommentCount = objLinkedinPost.CommentCount,
                    ShareCount = objLinkedinPost.ShareCount,
                    InteractionDatetime = DateTime.Now
                };

                // assign data of interacted post as per activity type
                AssignActivityData(objLinkedinPost, objAccountInteractedPosts);
                var InteractedAccountPost = _dbAccountService.GetInteractedPosts(_activityType);
                if (InteractedAccountPost != null && InteractedAccountPost.Any(post => post.QueryType == objAccountInteractedPosts.QueryType && post.QueryValue == objAccountInteractedPosts.QueryValue && post.PostLink == objAccountInteractedPosts.PostLink))
                    _dbAccountService.RemoveMatch<InteractedPosts>(post => post.QueryType == objAccountInteractedPosts.QueryType && post.QueryValue == objAccountInteractedPosts.QueryValue && post.PostLink == objAccountInteractedPosts.PostLink);
                _dbAccountService.Add(objAccountInteractedPosts);
                AddInteractedUser(scrapeResult, objLinkedinPost);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objCampaignInteractedPosts =
                    new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedPosts
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        PostOwnerFullName = objLinkedinPost.FullName,
                        PostOwnerProfileUrl = objLinkedinPost.ProfileUrl,
                        ConnectionType = objLinkedinPost.ConnectionType,
                        PostedTime = objLinkedinPost.PostedTime,
                        MediaType = objLinkedinPost.MediaType,
                        PostLink = objLinkedinPost.PostLink,
                        PostTitle = objLinkedinPost.PostTitle ?? "N/A",
                        PostDescription = objLinkedinPost.Caption,
                        MyComment = objAccountInteractedPosts.MyComment,
                        LikeCount = objLinkedinPost.LikeCount,
                        CommentCount = objLinkedinPost.CommentCount,
                        ShareCount = objLinkedinPost.ShareCount,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                var InteractedPosts = _dbCampaignService.GetInteractedPosts(_activityType);
                if(InteractedPosts != null && InteractedPosts.Any(post=>post.QueryType==objCampaignInteractedPosts.QueryType && post.QueryValue==objCampaignInteractedPosts.QueryValue && post.PostLink==objCampaignInteractedPosts.PostLink))
                    _dbCampaignService.RemoveMatch<InteractedPosts>(post => post.QueryType == objCampaignInteractedPosts.QueryType && post.QueryValue == objCampaignInteractedPosts.QueryValue && post.PostLink == objCampaignInteractedPosts.PostLink);
                _dbCampaignService.Add(objCampaignInteractedPosts);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void RemoveConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            RemoveConnectionModel removeOrWithdrawConnectionsModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = ActivityType.RemoveConnections.ToString(),
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = objLinkedinUser.ConnectedTimeStamp != 0
                            ? "Remove Connection"
                            : "Withdraw Connection Request",
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedUsers);
                }

                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = objLinkedinUser.ConnectedTimeStamp != 0
                        ? "Remove Connection"
                        : "Withdraw Connection Request",
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);


                var objRemovedConnections = new RemovedConnections
                {
                    IsDetailedUserInfoStored = false,
                    IsDetailedUserInfoVisible = true,
                    FullName = objLinkedinUser.FullName,
                    ProfileId = objLinkedinUser.ProfileId,
                    ProfileUrl = objLinkedinUser.ProfileUrl,
                    HasAnonymousProfilePicture = objLinkedinUser.HasAnonymousProfilePicture != null &&
                                                 objLinkedinUser.HasAnonymousProfilePicture != false,
                    ProfilePicUrl = objLinkedinUser.ProfilePicUrl,
                    ConnectedTimeStamp = objLinkedinUser.ConnectedTimeStamp,
                    ConnectionType = objLinkedinUser.ConnectedTimeStamp == 0
                        ? ConnectionType.SeondDegree
                        : ConnectionType.FirstDegree,
                    Occupation = objLinkedinUser.Occupation,
                    CompanyName = objLinkedinUser.CompanyName,
                    DetailedUserInfo = objLinkedinUser.DetailedUserInfo,
                    RemovedTimeStamp = DateTimeUtilities.GetEpochTime()
                };
                _dbAccountService.Add(objRemovedConnections);


                if (!removeOrWithdrawConnectionsModel.IsChkAddToBlackList ||
                    !removeOrWithdrawConnectionsModel.IsChkAddToPrivateBlackList &&
                    !removeOrWithdrawConnectionsModel.IsChkAddToGroupBlackList)
                    return;

                var objManageBlacklistWhitelist = new ManageBlacklistWhitelist(_dbAccountService, _delayService);
                if (removeOrWithdrawConnectionsModel.IsChkAddToPrivateBlackList)
                    objManageBlacklistWhitelist.AddToPrivateBlackList(AccountModel, objLinkedinUser);
                if (removeOrWithdrawConnectionsModel.IsChkAddToGroupBlackList)
                    objManageBlacklistWhitelist.AddToGroupBlackList(objLinkedinUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void WithdrawConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            WithdrawConnectionRequestModel withdrawConnectionRequestModel)
        {
            try
            {
                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = string.IsNullOrEmpty(objLinkedinUser.EmailAddress)
                        ? "N/A"
                        : objLinkedinUser.EmailAddress,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = string.IsNullOrEmpty(objLinkedinUser.ProfileUrl)
                        ? "N/A"
                        : objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = string.Empty,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);

                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = string.IsNullOrEmpty(objLinkedinUser.EmailAddress)
                            ? "N/A"
                            : objLinkedinUser.EmailAddress,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = string.IsNullOrEmpty(objLinkedinUser.ProfileUrl)
                            ? "N/A"
                            : objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = string.Empty,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedUsers);
                }

                if (!withdrawConnectionRequestModel.IsChkAddToBlackList ||
                    !withdrawConnectionRequestModel.IsChkAddToPrivateBlackList &&
                    !withdrawConnectionRequestModel.IsChkAddToGroupBlackList)
                    return;

                var objManageBlacklistWhitelist = new ManageBlacklistWhitelist(_dbAccountService, _delayService);
                if (withdrawConnectionRequestModel.IsChkAddToPrivateBlackList)
                    objManageBlacklistWhitelist.AddToPrivateBlackList(AccountModel, objLinkedinUser);
                if (withdrawConnectionRequestModel.IsChkAddToGroupBlackList)
                    objManageBlacklistWhitelist.AddToGroupBlackList(objLinkedinUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AutoReplyToNewMessage(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser, string message)
        {
            try
            {
                message = Utils.InsertSpecialCharactersInCsv(message);


                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = message,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };
                    _dbCampaignService.Add(objInteractedUsers);
                }


                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = message,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SendGreetingsToConnections(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedInfo)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = detailedInfo,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedUsers);
                }

                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = detailedInfo,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SendMessageToNewConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string message)
        {
            try
            {
                message = Utils.InsertSpecialCharactersInCsv(message);


                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = message,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedUsers = new InteractedUsers
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = message,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _dbCampaignService.Add(objInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SalesNavCompany(ScrapeResultNew scrapeResult, ICompany objLinkedinCompany,
            string detailedInfo)
        {
            try
            {
                var objAccountInteractedCompanies = new InteractedCompanies
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    CompanyName = objLinkedinCompany.CompanyName,
                    CompanyUrl = objLinkedinCompany.CompanyUrl,
                    TotalEmployees = objLinkedinCompany.TotalEmployees,
                    Industry = objLinkedinCompany.Industry,
                    IsFollowed = objLinkedinCompany.IsFollowed,
                    DetailedInfo = detailedInfo,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedCompanies);

                if (string.IsNullOrEmpty(_campaignId))
                    return;
                var objInteractedCompanies =
                    new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedCompanies
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        CompanyName = objLinkedinCompany.CompanyName,
                        CompanyUrl = objLinkedinCompany.CompanyUrl,
                        TotalEmployees = objLinkedinCompany.TotalEmployees,
                        Industry = objLinkedinCompany.Industry,
                        IsFollowed = objLinkedinCompany.IsFollowed,
                        DetailedInfo = detailedInfo,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                _dbCampaignService.Add(objInteractedCompanies);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void BroadcastMessages(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser, string message,
            string status)
        {
            try
            {
                message = Utils.InsertSpecialCharactersInCsv(message);

                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = message.AsCsvData(),
                    InteractionDatetime = DateTime.Now,
                    Status = status
                };
                _dbAccountService.Add(objAccountInteractedUsers);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedUsers = new InteractedUsers
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = message.AsCsvData(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    Status = status
                };
                _dbCampaignService.Add(objInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ExportConnection(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser, string contactInfo)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = contactInfo,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedUsers);
                }

                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = contactInfo,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ProfileEndorsement(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string endorsedSkillsCollection)
        {
            try
            {
                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = endorsedSkillsCollection,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedUsers = new InteractedUsers
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = endorsedSkillsCollection,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _dbCampaignService.Add(objInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SalesNavUser(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedUserInfoJasonString)
        {
            try
            {
                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = detailedUserInfoJasonString,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedUsers = new InteractedUsers
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = detailedUserInfoJasonString,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };
                _dbCampaignService.Add(objInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ConnectionRequest(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string finalPersonalNote)
        {
            try
            {
                finalPersonalNote = Utils.InsertSpecialCharactersInCsv(finalPersonalNote);

                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = string.IsNullOrEmpty(objLinkedinUser.FullName) ? "N/A" : objLinkedinUser.FullName,
                    UserProfileUrl = string.IsNullOrEmpty(objLinkedinUser.ProfileUrl)
                        ? "N/A"
                        : objLinkedinUser.ProfileUrl,
                    PublicIdentifer = objLinkedinUser.PublicIdentifier,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = string.IsNullOrEmpty(finalPersonalNote) ? "N/A" : finalPersonalNote,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
                var connectionUser = ClassMapper.Instance.LinkedInUserToConnections(objLinkedinUser);
                _dbAccountService.Add(connectionUser);
                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedUsers = new InteractedUsers
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = string.IsNullOrEmpty(objLinkedinUser.FullName) ? "N/A" : objLinkedinUser.FullName,
                    UserProfileUrl = string.IsNullOrEmpty(objLinkedinUser.ProfileUrl)
                        ? "N/A"
                        : objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = string.IsNullOrEmpty(finalPersonalNote) ? "N/A" : finalPersonalNote,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };
                _dbCampaignService.Add(objInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void Followpages(ScrapeResultNew scrapeResult, LinkedinPage objLinkedinpage)
        {
            try
            {
                var objAccountInteractedPage = new InteractedPage
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    PageName = string.IsNullOrEmpty(objLinkedinpage.PageName) ? "N/A" : objLinkedinpage.PageName,
                    PageUrl = string.IsNullOrEmpty(objLinkedinpage.PageUrl) ? "N/A" : objLinkedinpage.PageUrl,
                    PageId = objLinkedinpage.PageId,
                    FollowerCount = objLinkedinpage.FollowerCount,
                    TotalEmployees = objLinkedinpage.StaffCount,
                    IsFollowed = objLinkedinpage.IsFollowed,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedPage);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objCampaignInteractedPage = new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedPage
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    ActivityType = _activityType,
                    PageName = string.IsNullOrEmpty(objLinkedinpage.PageName) ? "N/A" : objLinkedinpage.PageName,
                    PageUrl = string.IsNullOrEmpty(objLinkedinpage.PageUrl) ? "N/A" : objLinkedinpage.PageUrl,
                    PageId = objLinkedinpage.PageId,
                    FollowerCount = objLinkedinpage.FollowerCount,
                    TotalEmployees = objLinkedinpage.StaffCount,
                    IsFollowed = objLinkedinpage.IsFollowed,
                    InteractionDatetime = DateTime.Now
                };
                _dbCampaignService.Add(objCampaignInteractedPage);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void JobScraper(ScrapeResultNew scrapeResult, LinkedinJob objLinkedinJob, string detailedInfo)
        {
            try
            {
                var objAccountInteractedJobs = new InteractedJobs
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    JobTitle = objLinkedinJob.JobTitle,
                    JobPostUrl = objLinkedinJob.JobPostUrl,
                    DetailedInfo = detailedInfo,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedJobs);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedJobs = new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedJobs
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    JobTitle = objLinkedinJob.JobTitle,
                    JobPostUrl = objLinkedinJob.JobPostUrl,
                    DetailedInfo = detailedInfo,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _dbCampaignService.Add(objInteractedJobs);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void GroupJoiner(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup)
        {
            try
            {
                var objAccountInteractedGroups = new InteractedGroups
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,

                    #region LinkedinGroup Details

                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,

                    #endregion

                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedGroups);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedGroups = new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedGroups
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,

                    #region LinkedinGroup Details

                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,

                    #endregion

                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _dbCampaignService.Add(objInteractedGroups);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GroupInviter(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup)
        {
            try
            {
                // add fields(columns) for InvitedUserProfile and InvitedUserProfileId
                var objAccountInteractedGroups = new InteractedGroups
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,

                    #region LinkedinGroup Details

                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,

                    #endregion

                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedGroups);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedGroups = new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedGroups
                {
                    AccountEmail = AccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,

                    #region LinkedinGroup Details

                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,

                    #endregion

                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _dbCampaignService.Add(objInteractedGroups);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GroupUnJoiner(ScrapeResultNew scrapeResult, LinkedinGroup objLinkedinGroup)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedGroups = new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedGroups
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,

                        #region LinkedinGroup Details

                        GroupName = objLinkedinGroup.GroupName,
                        GroupUrl = objLinkedinGroup.GroupUrl,
                        TotalMembers = objLinkedinGroup.TotalMembers,
                        CommunityType = objLinkedinGroup.CommunityType,
                        MembershipStatus = objLinkedinGroup.MembershipStatus,

                        #endregion

                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedGroups);
                }


                var objAccountInteractedGroups = new InteractedGroups
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,

                    #region LinkedinGroup Details

                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,

                    #endregion

                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedGroups);


                var unJoinedGroups = new UnjoinedGroups
                {
                    GroupName = objLinkedinGroup.GroupName,
                    GroupUrl = objLinkedinGroup.GroupUrl,
                    TotalMembers = objLinkedinGroup.TotalMembers,
                    CommunityType = objLinkedinGroup.CommunityType,
                    MembershipStatus = objLinkedinGroup.MembershipStatus,
                    UnjoinedTimeStamp = DateTimeUtilities.GetEpochTime()
                };
                _dbAccountService.Add(unJoinedGroups);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void AcceptConnectionRequest(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser.FullName,
                        UserProfileUrl = objLinkedinUser.ProfileUrl,
                        ProfileId = objLinkedinUser.ProfileId,
                        DetailedUserInfo = string.Empty,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                    _dbCampaignService.Add(objInteractedUsers);
                }


                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser.FullName,
                    UserProfileUrl = objLinkedinUser.ProfileUrl,
                    ProfileId = objLinkedinUser.ProfileId,
                    DetailedUserInfo = string.Empty,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CompanyScraper(ScrapeResultNew scrapeResult, ICompany objLinkedinCompany,
            string detailedInfo)
        {
            try
            {
                var objAccountInteractedCompanies = new InteractedCompanies
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ActivityType = _activityType,
                    CompanyName = objLinkedinCompany.CompanyName,
                    CompanyUrl = objLinkedinCompany.CompanyUrl,
                    TotalEmployees = objLinkedinCompany.TotalEmployees,
                    Industry = objLinkedinCompany.Industry,
                    IsFollowed = objLinkedinCompany.IsFollowed,
                    DetailedInfo = detailedInfo,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedCompanies);

                if (string.IsNullOrEmpty(_campaignId))
                    return;

                var objInteractedCompanies =
                    new DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedCompanies
                    {
                        AccountEmail = AccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ActivityType = _activityType,
                        CompanyName = objLinkedinCompany.CompanyName,
                        CompanyUrl = objLinkedinCompany.CompanyUrl,
                        TotalEmployees = objLinkedinCompany.TotalEmployees,
                        Industry = objLinkedinCompany.Industry,
                        IsFollowed = objLinkedinCompany.IsFollowed,
                        DetailedInfo = detailedInfo,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };

                _dbCampaignService.Add(objInteractedCompanies);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UserScraper(ScrapeResultNew scrapeResult, LinkedinUser objLinkedinUser,
            string detailedUserInfoJasonString)
        {
            try
            {
                if (!string.IsNullOrEmpty(_campaignId))
                {
                    var objInteractedUsers = new InteractedUsers
                    {
                        AccountEmail = AccountModel.AccountBaseModel?.UserName,
                        QueryType = scrapeResult.QueryInfo?.QueryType,
                        QueryValue = scrapeResult.QueryInfo?.QueryValue,
                        ActivityType = _activityType,
                        UserFullName = objLinkedinUser?.FullName,
                        UserProfileUrl = objLinkedinUser?.ProfileUrl,
                        ProfileId = objLinkedinUser?.ProfileId,
                        AttachmentId = objLinkedinUser?.AttachmentId,
                        ConnectedTime = string.IsNullOrEmpty(objLinkedinUser.ConnectedTime)
                            ? "N/A"
                            : objLinkedinUser.ConnectedTime,
                        DetailedUserInfo = detailedUserInfoJasonString,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    };
                    _dbCampaignService.Add(objInteractedUsers);
                }


                var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
                {
                    QueryType = scrapeResult.QueryInfo?.QueryType,
                    QueryValue = scrapeResult.QueryInfo?.QueryValue,
                    ActivityType = _activityType,
                    UserFullName = objLinkedinUser?.FullName,
                    UserProfileUrl = objLinkedinUser?.ProfileUrl,
                    ProfileId = objLinkedinUser?.ProfileId,
                    AttachmentId = objLinkedinUser?.AttachmentId,
                    ConnectedTime = string.IsNullOrEmpty(objLinkedinUser.ConnectedTime)
                        ? "N/A"
                        : objLinkedinUser.ConnectedTime,
                    DetailedUserInfo = detailedUserInfoJasonString,
                    InteractionDatetime = DateTime.Now
                };
                _dbAccountService.Add(objAccountInteractedUsers);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddInteractedUser(ScrapeResultNew scrapeResult, LinkedinPost objLinkedinPost)
        {
            var detailedUserInfo = JsonConvert.SerializeObject(objLinkedinPost);
            var objAccountInteractedUsers = new DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers
            {
                QueryType = scrapeResult.QueryInfo.QueryType,
                QueryValue = scrapeResult.QueryInfo.QueryValue,
                ActivityType = _activityType,
                UserFullName = objLinkedinPost.FullName,
                UserProfileUrl = objLinkedinPost.ProfileUrl,
                ProfileId = objLinkedinPost.ProfileId,
                DetailedUserInfo = detailedUserInfo,
                InteractionDatetime = DateTime.Now
            };

            _dbAccountService.Add(objAccountInteractedUsers);
        }

        private void AssignActivityData(LinkedinPost objLinkedinPost, InteractedPosts objAccountInteractedPosts)
        {
            switch (_activityType)
            {
                case "Comment":
                    objAccountInteractedPosts.MyComment = objLinkedinPost.MyComment;
                    break;
                default:
                    objAccountInteractedPosts.MyComment = "N/A";
                    break;
            }
        }
    }
}