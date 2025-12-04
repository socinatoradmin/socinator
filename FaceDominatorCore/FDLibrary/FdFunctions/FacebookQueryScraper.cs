using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.BroadCastProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.EventProcessors;
using FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.FriendsProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.MarketplaceProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.PostProcessor;
using FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdFunctions
{
    public interface IFacebookScraperActionTables : IScraperActionTables { }

    public class FacebookQueryScraper : QueryScraper
    {
        public FacebookQueryScraper(IFdJobProcess jobProcess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable) { }
    }

    public class FacebookScraperActionTables : IFacebookScraperActionTables
    {
        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        #region Global properties

        //private readonly ModuleSetting _moduleSetting;

        //private IDbCampaignServiceScoped ObjDbCampaignService { get; }

        //private IDbAccountServiceScoped ObjDbAccountService { get; }

        //private DbGlobalService ObjDbGlobalService { get; }

        //private readonly IFdRequestLibrary ObjFdRequestLibrary;

        private ActivityType ActivityType { get; }

        private DominatorAccountModel DominatorAccountModel { get; }

        private readonly CancellationToken _token;

        private FdJobProcess JobProcess { get; }

        public JobProcess CurrentJobProcess { get; set; }


        private readonly IUnityContainer _unityContainer;

        protected BlackListWhitelistHandler BlackListWhitelistHandler { get; set; }

        #endregion

        public FacebookScraperActionTables(JobProcess jobProcess, IUnityContainer unityContainer)
        {
            CurrentJobProcess = jobProcess;
            JobProcess = (FdJobProcess)jobProcess;
            DominatorAccountModel = jobProcess.DominatorAccountModel;


            ActivityType = jobProcess.ActivityType;
            BlackListWhitelistHandler = new BlackListWhitelistHandler(JobProcess.ModuleSetting, DominatorAccountModel, ActivityType);

            _unityContainer = unityContainer;
            _token = CurrentJobProcess.JobCancellationTokenSource.Token;

            FdLoginProcess.RequestParameterInitialize(DominatorAccountModel);

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>()
            {
                { $"{ActivityType.PostLikerCommentor}",StartPostLikerCommentorProcess},
                { $"{ActivityType.PostScraper}",StartPostLikerCommentorProcess},
                { $"{ActivityType.PostLiker}",StartPostLikerCommentorProcess},
                { $"{ActivityType.PostCommentor}",StartPostLikerCommentorProcess},
                 { $"{ActivityType.DownloadScraper}",StartPostLikerCommentorProcess},

                { $"{ActivityType.IncommingFriendRequest}",StartIncommingFriendRequestProcess},
                { $"{ActivityType.Unfriend}",StartUnfriendRequestProcess},
                { $"{ActivityType.WithdrawSentRequest}",StartCancelRequestProcess},
                { $"{ActivityType.GroupUnJoiner}",StartNewGroupUnjoinerProcess},
                { $"{ActivityType.EventInviter}",StartEventInviterProcess},
                { $"{ActivityType.GroupInviter}",StartGroupInviterProcess},
                { $"{ActivityType.PageInviter}",StartPageInviterProcess},
                { $"{ActivityType.WatchPartyInviter}",StartWatchPartyInviterProcess},
                { $"{ActivityType.AutoReplyToNewMessage}",StartAutoReplyToNewMessageProcess},
                { $"{ActivityType.SendMessageToNewFriends}",StartSendMessageToNewFriendsProcess},
                { $"{ActivityType.SendGreetingsToFriends}",StartSendGreetingsRequestProcess},
                { $"{ActivityType.EventCreator}",StartEventCreaterProcess},
                { $"{ActivityType.MakeAdmin}",StartMakeGroupAdminPorcess },
                { $"{ActivityType.Unfollow}", StartUnfollowFriendProcess }
            };
            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>()
            {
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.Keywords)}",StartSendRequestForKeyword},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.Location)}",StartProcessForLocation},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.FriendofFriend)}",StartSendRequestProcessForFriendofFriend},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.GraphSearchUrl)}",StartSendRequestForGraphSearch},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.GroupMembers)}",StartSendRequestProcessForGroupMembers},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.FanpageLikers)}",StartSendRequestProcessForPageMembers},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.PostLikers)}",StartProfileScraperProcessForPostLikers},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.PostSharer)}",StartProfileScraperProcessForPostSharer},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.PostCommentor)}",StartProfileScraperProcessForPostCommentor},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.PagePostLikers)}",StartProfileScraperProcessPagePostLikers},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.PagePostPostCommenters)}",StartProfileScraperProcessPagePostCommenters},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.GroupPostLikers)}",StartProfileScraperProcessGroupPostLikers},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.GroupPostCommenters)}",StartProfileScraperProcessGroupPostCommenters},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.CustomProfileUrl)}",StartSendRequestProcessForCustomUserList},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.SuggestedFriends)}",StartSendRequestProcessForSuggestedFriend},
                { $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.UserFollowers)}",StartSendRequestProcessForUserFollowers},
                //{ $"{ActivityType.SendFriendRequest}{EnumUtility.GetQueryFromEnum(FdUserQueryParameters.EventUrl)}",StartProfileScraperProcessForEventUrl},

                { $"{ActivityType.GroupJoiner}{EnumUtility.GetQueryFromEnum(GroupJoinerParameter.Keywords)}",StartGroupJoinerProcessForKeywords},
                { $"{ActivityType.GroupJoiner}{EnumUtility.GetQueryFromEnum(GroupJoinerParameter.GraphSearchUrl)}",StartGroupJoinerProcessForGraphSearch},
                { $"{ActivityType.GroupJoiner}{EnumUtility.GetQueryFromEnum(GroupJoinerParameter.CustomGroupUrl)}",StartGroupJoinerProcessForCustomUserUrl},

                { $"{ActivityType.FanpageLiker}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.Keywords)}",StartFanpageLikerProcessForKeyword},
                { $"{ActivityType.FanpageLiker}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.GraphSearchUrl)}",StartFanpageLikerProcessGraphSearch},
                { $"{ActivityType.FanpageLiker}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.CustomPageList)}",StartFanpageLikerProcessForCustomUrl},
                { $"{ActivityType.FanpageLiker}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.PagesLikedByFriends)}",StartFanpageLikerProcessForPagesLikedByFriends},

                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.Keywords)}",StartSendRequestForKeyword},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.Location)}",StartProcessForLocation},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FriendofFriend)}",StartSendRequestProcessForFriendofFriend},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupMembers)}",StartSendRequestProcessForGroupMembers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FanpageLikers)}",StartSendRequestProcessForPageMembers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostLikers)}",StartProfileScraperProcessForPostLikers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostSharer)}",StartProfileScraperProcessForPostSharer},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostCommentor)}",StartProfileScraperProcessForPostCommentor},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GraphSearchUrl)}",StartSendRequestForGraphSearch},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PagePostLikers)}",StartProfileScraperProcessPagePostLikers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PagePostPostCommenters)}",StartProfileScraperProcessPagePostCommenters},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupPostLikers)}",StartProfileScraperProcessGroupPostLikers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupPostCommenters)}",StartProfileScraperProcessGroupPostCommenters},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.CustomProfileUrl)}",StartSendRequestProcessForCustomUserList},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.SuggestedFriends)}",StartSendRequestProcessForSuggestedFriend},
                //{ $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.EventUrl)}",StartProfileScraperProcessForEventUrl},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.UserFollowers)}",StartSendRequestProcessForUserFollowers},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.OwnFriends)}",StartSendRequestProcessForOwnFriends},
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FriendsBasicDetails)}",StartScrapeUserFriendsBasicDetails },
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupMemberBasicDetails)}",StartSendRequestProcessForGroupMembers },
                { $"{ActivityType.ProfileScraper}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.ConnectedPeopleInMessenger)}",StartScrapeConnectedPeopleProfiles},

                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.Keywords)}",StartSendRequestForKeyword},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.Location)}",StartProcessForLocation},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FriendofFriend)}",StartSendRequestProcessForFriendofFriend},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GraphSearchUrl)}",StartSendRequestForGraphSearch},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupMembers)}",StartSendRequestProcessForGroupMembers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FanpageLikers)}",StartSendRequestProcessForPageMembers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostLikers)}",StartProfileScraperProcessForPostLikers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostSharer)}",StartProfileScraperProcessForPostSharer},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PostCommentor)}",StartProfileScraperProcessForPostCommentor},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PagePostLikers)}",StartProfileScraperProcessPagePostLikers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.PagePostPostCommenters)}",StartProfileScraperProcessPagePostCommenters},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupPostLikers)}",StartProfileScraperProcessGroupPostLikers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupPostCommenters)}",StartProfileScraperProcessGroupPostCommenters},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.CustomProfileUrl)}",StartSendRequestProcessForCustomUserList},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.SuggestedFriends)}",StartSendRequestProcessForSuggestedFriend},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.UserFollowers)}",StartSendRequestProcessForUserFollowers},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.OwnFriends)}",StartSendRequestProcessForOwnFriends},
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.FriendsBasicDetails)}",StartScrapeUserFriendsBasicDetails },
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.GroupMemberBasicDetails)}",StartSendRequestProcessForGroupMembers },
                { $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.ConnectedPeopleInMessenger)}",StartScrapeConnectedPeopleProfiles },
                //{ $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(FdProfileQueryParameters.EventUrl)}",StartProfileScraperProcessForEventUrl },


                { $"{ActivityType.FanpageScraper}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.Keywords)}",StartFanpageLikerProcessForKeyword},
                { $"{ActivityType.FanpageScraper}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.GraphSearchUrl)}",StartFanpageLikerProcessGraphSearch},
                { $"{ActivityType.FanpageScraper}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.CustomPageList)}",StartFanpageLikerProcessForCustomUrl},
                { $"{ActivityType.FanpageScraper}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.PagesLikedByFriends)}",StartFanpageLikerProcessForPagesLikedByFriends},

                { $"{ActivityType.MessageToFanpages}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.Keywords)}",StartFanpageLikerProcessForKeyword},
                { $"{ActivityType.MessageToFanpages}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.GraphSearchUrl)}",StartFanpageLikerProcessGraphSearch},
                { $"{ActivityType.MessageToFanpages}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.CustomPageList)}",StartFanpageLikerProcessForCustomUrl},
                { $"{ActivityType.MessageToFanpages}{EnumUtility.GetQueryFromEnum(FanpageLikerQueryParameters.PagesLikedByFriends)}",StartFanpageLikerProcessForPagesLikedByFriends},

                { $"{ActivityType.MessageToPlaces}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.Keywords)}",StartPlaceScraperProcessForKeyword},
                { $"{ActivityType.MessageToPlaces}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.GraphSearchUrl)}",StartPlaceScraperProcessGraphSearch},
                { $"{ActivityType.MessageToPlaces}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.CustomPageList)}",StartPlaceScraperProcessForPostCommentor},

                { $"{ActivityType.PlaceScraper}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.Keywords)}",StartPlaceScraperProcessForKeyword},
                { $"{ActivityType.PlaceScraper}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.GraphSearchUrl)}",StartPlaceScraperProcessGraphSearch},
                { $"{ActivityType.PlaceScraper}{EnumUtility.GetQueryFromEnum(PlaceQueryParameters.CustomPageList)}",StartPlaceScraperProcessForPostCommentor},

                { $"{ActivityType.GroupScraper}{EnumUtility.GetQueryFromEnum(GroupScraperParameter.Keywords)}",StartGroupJoinerProcessForKeywords},
                { $"{ActivityType.GroupScraper}{EnumUtility.GetQueryFromEnum(GroupScraperParameter.GraphSearchUrl)}",StartGroupJoinerProcessForGraphSearch},
                { $"{ActivityType.GroupScraper}{EnumUtility.GetQueryFromEnum(GroupJoinerParameter.CustomGroupUrl)}",StartGroupJoinerProcessForCustomUserUrl},

                { $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(CommentScraperParameter.PostUrl)}",StartCommentScraperProcessForPostCommentor},
                { $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(CommentScraperParameter.PagePostComments)}",StartCommentScraperProcessForPagePosts},
                { $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(CommentScraperParameter.GroupPostComments)}",StartCommentScraperProcessForGroupPosts},
                { $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(CommentScraperParameter.NewsFeedPosts)}",StartCommentScraperProcessForNewsfeedPosts},

                { $"{ActivityType.LikeComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.PostUrl)}",StartCommentScraperProcessForPostCommentor},
                { $"{ActivityType.LikeComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.PagePostComments)}",StartCommentScraperProcessForPagePosts},
                { $"{ActivityType.LikeComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.GroupPostComments)}",StartCommentScraperProcessForGroupPosts},
                { $"{ActivityType.LikeComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.NewsFeedPosts)}",StartCommentScraperProcessForNewsfeedPosts},

                //{ $"{ActivityType.LikeComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.CommentUrl)}",StartCommentLikerProcessForCustomUrl}
                { $"{ActivityType.ReplyToComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.PostUrl)}",StartCommentScraperProcessForPostCommentor},
                { $"{ActivityType.ReplyToComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.PagePostComments)}",StartCommentScraperProcessForPagePosts},
                { $"{ActivityType.ReplyToComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.GroupPostComments)}",StartCommentScraperProcessForGroupPosts},
                { $"{ActivityType.ReplyToComment}{EnumUtility.GetQueryFromEnum(CommentLikerParameter.NewsFeedPosts)}",StartCommentScraperProcessForNewsfeedPosts},

                { $"{ActivityType.WebPostLikeComment}{EnumUtility.GetQueryFromEnum(WebCommentLikerParameter.Webpagecomment)}",StartCommentScraperProcessForWebPostcomment},
                { $"{ActivityType.WebPostLikeComment}{EnumUtility.GetQueryFromEnum(WebCommentLikerParameter.Webpagereplycomments)}",StartCommentScraperProcessForWebPostRecomment},

                { $"{ActivityType.MarketPlaceScraper}{EnumUtility.GetQueryFromEnum(MarketPlaceQueryParameter.Keywords)}",StartMarketplaceScraperProcessForKeyword},

                {$"{ActivityType.CommentRepliesScraper}{EnumUtility.GetQueryFromEnum(CommentRepliesScraperParameter.PostUrl)}",StartRepliesToCommentForPostUrl},
                {$"{ActivityType.CommentRepliesScraper}{EnumUtility.GetQueryFromEnum(CommentRepliesScraperParameter.CommentUrl)}",StartRepliesToCommentForCustomUrl},
                {$"{ActivityType.CommentRepliesScraper}{EnumUtility.GetQueryFromEnum(CommentRepliesScraperParameter.PagePostComments)}",StartRepliesToCommentForPagePostComments}

            };
        }

        private void StartRepliesToCommentForPagePostComments(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PagePostCommentRepliesProcessor>();

            processor.Start(queryInfo);
        }
        private void StartRepliesToCommentForCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomCommentRepliesProcessor>();

            processor.Start(queryInfo);
        }

        private void StartRepliesToCommentForPostUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostCommentRepliesProcessor>();

            processor.Start(queryInfo);
        }

        private void StartScrapeUserFriendsBasicDetails(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserFriendsBasicDetailsProcessor>();

            processor.Start(queryInfo);
        }

        private void StartMakeGroupAdminPorcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<MakeGroupAdminProcessor>();

            processor.Start(QueryInfo.NoQuery);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartFanpageLikerProcessForPagesLikedByFriends(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PagesLikedByFriendProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestProcessForOwnFriends(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserFrinedsProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestProcessForUserFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserFollowerProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartCommentScraperProcessForNewsfeedPosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<NewsfeedPostCommentProcessor>();

            processor.Start(queryInfo);
        }

        private void StartCommentScraperProcessForWebPostcomment(QueryInfo queryInfo)
        {
            //IQueryProcessor processor = _unityContainer.Resolve<CustomWebPostCommentLikerProcessor>();

            //processor.Start(queryInfo);
        }

        private void StartCommentScraperProcessForWebPostRecomment(QueryInfo queryInfo)
        {
            //IQueryProcessor processor = _unityContainer.Resolve<CustomWebPostReCommentLikerProcessor>();

            //processor.Start(queryInfo);
        }

        private void StartCommentScraperProcessForGroupPosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupPostCommentLikerProcessor>();

            processor.Start(queryInfo);
        }

        private void StartCommentScraperProcessForPagePosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PagePostCommentLikerProcessor>();

            processor.Start(queryInfo);
        }

        private void StartGroupJoinerProcessForCustomUserUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomGroupProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartFanpageLikerProcessForCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomFanpageProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartSendRequestForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordUserProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProcessForLocation(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<LocationUserProcessor>();

            processor.Start(queryInfo);
        }

        protected void StartSendRequestProcessForFriendofFriend(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FriendOFriendProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestForGraphSearch(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GraphSearchUserPrcessor>();

            processor.Start(queryInfo);
        }

        private void StartScrapeConnectedPeopleProfiles(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<ConnectedPeopleProfilesProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestProcessForGroupMembers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupMembersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestProcessForPageMembers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FanpageLikersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessForPostLikers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostLikersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessForPostSharer(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostSharerProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessForPostCommentor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostCommentorProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessPagePostLikers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PagePostLikersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessPagePostCommenters(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PagePostCommetersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessGroupPostLikers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupPostLikersProcessor>();

            processor.Start(queryInfo);
        }
        private void StartProfileScraperProcessGroupPostCommenters(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupPostCommentersProcessor>();

            processor.Start(queryInfo);
        }

        private void StartSendRequestProcessForCustomUserList(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        protected void StartSendRequestProcessForSuggestedFriend(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SuggestedFriendsProcessor>();

            processor.Start(queryInfo);
        }

        private void StartProfileScraperProcessForEventUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<EventsUserProcessor>();

            processor.Start(queryInfo);
        }

        private void StartGroupJoinerProcessForKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordGroupProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartGroupJoinerProcessForGraphSearch(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GraphSearchGroupProcessors>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartFanpageLikerProcessForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordFanpageProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartFanpageLikerProcessGraphSearch(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GraphSearchFnapageProcessor>();

            processor.Start(queryInfo);
        }

        private void StartCommentScraperProcessForPostCommentor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomCommentLikerProcessor>();

            processor.Start(queryInfo);
        }

        private void StartPlaceScraperProcessForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordPlaceProcessor>();

            processor.Start(queryInfo);
        }

        private void StartPlaceScraperProcessGraphSearch(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GraphSearchPlaceProcessor>();

            processor.Start(queryInfo);
        }

        private void StartPlaceScraperProcessForPostCommentor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomPlaceProcessor>();

            processor.Start(queryInfo);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartMarketplaceScraperProcessForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordMarketplaceScraper>();

            processor.Start(queryInfo);
        }

        private void StartWatchPartyInviterProcess()
        {
            List<FbEntityTypes> objListEntityTypes = new List<FbEntityTypes>();

            if (JobProcess.ModuleSetting.InviterDetailsModel.IsGroupMember)
                objListEntityTypes.Add(FbEntityTypes.Group);
            if (JobProcess.ModuleSetting.InviterDetailsModel.IsProfileUrl)
                objListEntityTypes.Add(FbEntityTypes.Friend);


            objListEntityTypes.ForEach(x =>
            {
                if (x != FbEntityTypes.Friend)
                {
                    IQueryProcessor processor = _unityContainer.Resolve<WpInviterForGroupMembers>();
                    processor.Start(QueryInfo.NoQuery);
                }
                else
                {
                    IQueryProcessor processor = _unityContainer.Resolve<WpInviterForFriends>();
                    processor.Start(QueryInfo.NoQuery);
                }

            });


        }

        private void StartSendMessageToNewFriendsProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<MessageToNewFriendsProcessor>();

            processor.Start(QueryInfo.NoQuery);
        }

        private void StartUnfriendRequestProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnfriendUserProcessor>();

            processor.Start(QueryInfo.NoQuery);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartUnfollowFriendProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnfollowUserProcessor>();

            processor.Start(QueryInfo.NoQuery);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartCancelRequestProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<WithdrawRequestProcessor>();

            processor.Start(QueryInfo.NoQuery);
        }

        private void StartSendGreetingsRequestProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendBirthdayGreetingsProcessor>();

            processor.Start(QueryInfo.NoQuery);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartEventCreaterProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<EventCreatorProcessors>();

            processor.Start(QueryInfo.NoQuery);

            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartPostLikerCommentorProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<StartPostScraperProcessor>();

            processor.Start(QueryInfo.NoQuery);
        }

        private void StartNewGroupUnjoinerProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupUnjoinerProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //StartNewGroupUnJoinerWithJobProcess();
        }

        private void StartIncommingFriendRequestProcess()
        {

            IQueryProcessor processor = _unityContainer.Resolve<IncommingFriendRequestProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //    ScrapIncommingFriendRequestWithJobProcess();

        }

        private void StartAutoReplyToNewMessageProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyToNewMessageProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //    StartAutoReplydWithJobProcess();

        }

        private void StartPageInviterProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<PageInviterProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //    StartPageInviterWithJobProcess();

        }

        private void StartEventInviterProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<EventInviterProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //StartEventInviterWithJobProcess();
        }

        private void StartGroupInviterProcess()
        {

            IQueryProcessor processor = _unityContainer.Resolve<GroupInviterProcessor>();

            processor.Start(QueryInfo.NoQuery);

            //StartGroupInviterWithJobProcess();
        }


    }
}
