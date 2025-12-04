using DominatorHouse.Social;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouse.Utilities;
using DominatorHouse.ViewModels;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.ViewModel.Startup;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
{
    // ReSharper disable once UnusedMember.Global
    // the class is used through the configuration (see app.config)
    public class MainContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // views

            // view models
            Container.RegisterSingleton<ICampaignStatusChange, CampaignStatusChange>();
            Container.RegisterSingleton<ICustomDialogue, CustomDialogue>();
            Container.RegisterSingleton<IMainViewModel, MainViewModel>();
            Container.RegisterSingleton<IPuppeteerBrowserManager, PuppeteerBrowserManager>();
            Container.RegisterSingleton<IPerfCounterViewModel, PerfCounterViewModel>();
            Container.RegisterSingleton<IDominatorAutoActivityViewModel, DominatorAutoActivityViewModel>();
            Container.RegisterSingleton<ISocialBrowserManager, SocialBrowserManager>();
            Container.RegisterSingleton<IAccountSyncManager,AccountSyncManager>();
            #region Startup ViewModel

            Container.RegisterType<IFollowViewModel, FollowViewModel>();
            Container.RegisterType<IUnFollowerViewModel, UnFollowerViewModel>();
            Container.RegisterType<ILikeViewModel, LikeViewModel>();
            Container.RegisterType<IUnlikeViewModel, UnlikeViewModel>();
            Container.RegisterType<ICommentViewModel, CommentViewModel>();
            Container.RegisterSingleton<ISelectActivityViewModel, SelectActivityViewModel>();
            Container.RegisterSingleton<ISaveSetting, SaveSetting>();
            Container.RegisterType<ICompanyScraperViewModel, CompanyScraperViewModel>();
            Container.RegisterType<IGroupMemberScraperViewModel, GroupMemberScraperViewModel>();
            Container.RegisterType<ISalesNavigatorUserScraperViewModel, SalesNavigatorUserScraperViewModel>();
            Container.RegisterType<IChannelScraperViewModel, ChannelScraperViewModel>();
            Container.RegisterType<IBroadcastMessagesViewModel, BroadcastMessagesViewModel>();
            Container.RegisterType<ISendMessageToFollowerViewModel, SendMessageToFollowerViewModel>();
            Container.RegisterType<IAutoReplyToNewMessageViewModel, AutoReplyToNewMessageViewModel>();
            Container.RegisterType<IAcceptConnectionRequestViewModel, AcceptConnectionRequestViewModel>();
            Container.RegisterType<IRemoveConnectionsViewModel, RemoveConnectionsViewModel>();
            Container.RegisterType<IProfileEndorsementViewModel, ProfileEndorsementViewModel>();
            Container.RegisterType<IReplyToCommentViewModel, ReplyToCommentViewModel>();
            Container.RegisterType<IReblogViewModel, ReblogViewModel>();
            Container.RegisterType<ITryViewModel, TryViewModel>();
            Container.RegisterType<ISendMessageToNewConnectionViewModel, SendMessageToNewConnectionViewModel>();
            Container.RegisterType<IUrlScraperViewModel, UrlScraperViewModel>();
            Container.RegisterType<ISendGreetingsToConnectionsViewModel, SendGreetingsToConnectionsViewModel>();
            Container.RegisterType<IReplyViewModel, ReplyViewModel>();
            Container.RegisterType<IDeleteViewModel, DeleteViewModel>();
            Container.RegisterType<IIncommingFriendRequestViewModel, IncommingFriendRequestViewModel>();
            Container.RegisterType<IDownvoteViewModel, DownvoteViewModel>();
            Container.RegisterType<IUpvoteViewModel, UpvoteViewModel>();
            Container.RegisterType<IDeletePinViewModel, DeletePinViewModel>();
            Container.RegisterType<IRepinViewModel, RepinViewModel>();
            Container.RegisterType<IWithdrawConnectionRequestViewModel, WithdrawConnectionRequestViewModel>();
            Container.RegisterType<IRemoveVoteViewModel, RemoveVoteViewModel>();
            Container.RegisterType<IExportConnectionViewModel, ExportConnectionViewModel>();
            Container.RegisterType<IPostLikerViewModel, PostLikerViewModel>();
            Container.RegisterType<IPostCommentorViewModel, PostCommentorViewModel>();
            Container.RegisterType<IRemoveVoteCommentViewModel, RemoveVoteCommentViewModel>();
            Container.RegisterType<IUpvoteCommentViewModel, UpvoteCommentViewModel>();
            Container.RegisterType<IDownvoteCommentViewModel, DownvoteCommentViewModel>();
            Container.RegisterType<IDislikeViewModel, DislikeViewModel>();
            Container.RegisterType<IAnswerOnQuestionsViewModel, AnswerOnQuestionsViewModel>();
            Container.RegisterType<IWelcomeTweetViewModel, WelcomeTweetViewModel>();
            Container.RegisterType<ISalesNavigatorCompanyScraperViewModel, SalesNavigatorCompanyScraperViewModel>();
            Container.RegisterType<ITweetToViewModel, TweetToViewModel>();
            Container.RegisterType<ISendMessageToNewFriendsViewModel, SendMessageToNewFriendsViewModel>();
            Container.RegisterType<IWatchPartyInviterViewModel, WatchPartyInviterViewModel>();
            Container.RegisterType<IMarketPlaceScraperViewModel, MarketPlaceScraperViewModel>();
            Container.RegisterType<ISendGreetingsToFriendsViewModel, SendGreetingsToFriendsViewModel>();
            Container.RegisterType<IWebPostLikeCommentViewModel, WebPostLikeCommentViewModel>();
            Container.RegisterType<IEditPinViewModel, EditPinViewModel>();
            Container.RegisterType<IAcceptBoardInvitationViewModel, AcceptBoardInvitationViewModel>();
            Container.RegisterType<ISendBoardInvitationViewModel, SendBoardInvitationViewModel>();
            Container.RegisterType<IBlockUserViewModel, BlockUserViewModel>();
            Container.RegisterType<IMessageToFanpagesViewModel, MessageToFanpagesViewModel>();
            Container.RegisterType<IMessageToPlacesViewModel, MessageToPlacesViewModel>();
            Container.RegisterType<IPlaceScraperViewModel, PlaceScraperViewModel>();
            Container.RegisterType<IEditCommentViewModel, EditCommentViewModel>();
            Container.RegisterType<IDeleteCommentViewModel, DeleteCommentViewModel>();
            Container.RegisterType<IPostViewModel, PostViewModel>();
            Container.RegisterType<IDeletePostViewModel, DeletePostViewModel>();
            Container.RegisterType<IUserScraperViewModel, UserScraperViewModel>();
            Container.RegisterType<IDownloadScraperViewModel, DownloadScraperViewModel>();
            Container.RegisterType<IReposterViewModel, ReposterViewModel>();
            Container.RegisterType<IRetweetViewModel, RetweetViewModel>();
            Container.RegisterType<IQuestionsScraperViewModel, QuestionsScraperViewModel>();
            Container.RegisterType<IAnswersScraperViewModel, AnswersScraperViewModel>();
            Container.RegisterType<IVoteAnswersViewModel, VoteAnswersViewModel>();
            Container.RegisterType<IDownvoteAnswersViewModel, DownvoteAnswersViewModel>();
            Container.RegisterType<IReportQuestionsViewModel, ReportQuestionsViewModel>();
            Container.RegisterType<IReportAnswersViewModel, ReportAnswersViewModel>();
            Container.RegisterType<IReportUsersViewModel, ReportUsersViewModel>();
            Container.RegisterType<IBoardScraperViewModel, BoardScraperViewModel>();
            Container.RegisterType<IPinScraperViewModel, PinScraperViewModel>();
            Container.RegisterType<ISendFriendRequestViewModel, SendFriendRequestViewModel>();
            Container.RegisterType<ICancelSentRequestViewModel, CancelSentRequestViewModel>();
            Container.RegisterType<IUnfriendViewModel, UnfriendViewModel>();
            Container.RegisterType<IGroupScraperViewModel, GroupScraperViewModel>();
            Container.RegisterType<IFanpageScraperViewModel, FanpageScraperViewModel>();
            Container.RegisterType<ICommentScraperViewModel, CommentScraperViewModel>();
            Container.RegisterType<IPostScraperViewModel, PostScraperViewModel>();
            Container.RegisterType<IGroupJoinerViewModel, GroupJoinerViewModel>();
            Container.RegisterType<IGroupUnJoinerViewModel, GroupUnJoinerViewModel>();
            Container.RegisterType<IGroupInviterViewModel, GroupInviterViewModel>();
            Container.RegisterType<IPageInviterViewModel, PageInviterViewModel>();
            Container.RegisterType<IEventInviterViewModel, EventInviterViewModel>();
            Container.RegisterType<IGroupCreatorViewModel, GroupCreatorViewModel>();
            Container.RegisterType<IEventCreatorViewModel, EventCreatorViewModel>();
            Container.RegisterType<ITweetViewModel, TweetViewModel>();
            Container.RegisterType<IProfileScraperViewModel, ProfileScraperViewModel>();
            Container.RegisterType<IDownvoteQuestionsViewModel, DownvoteQuestionsViewModel>();
            Container.RegisterType<IUpvoteAnswersViewModel, UpvoteAnswersViewModel>();
            Container.RegisterType<IJoinViewModel, JoinViewModel>();
            Container.RegisterType<IUnjoinViewModel, UnjoinViewModel>();
            Container.RegisterType<IPostLikerCommentorViewModel, PostLikerCommentorViewModel>();
            Container.RegisterType<IFanpageLikerViewModel, FanpageLikerViewModel>();
            Container.RegisterType<IWebpageLikerCommentorViewModel, WebpageLikerCommentorViewModel>();
            Container.RegisterType<ITweetScraperViewModel, TweetScraperViewModel>();
            Container.RegisterType<IMakeAdminViewModel, MakeAdminViewModel>();
            Container.RegisterType<IConnectionRequestViewModel, ConnectionRequestViewModel>();
            Container.RegisterType<ISubscribeViewModel, SubscribeViewModel>();
            Container.RegisterType<IShareViewModel, ShareViewModel>();
            Container.RegisterType<IUnsubscribeViewModel, UnsubscribeViewModel>();
            Container.RegisterType<IViewIncreaserViewModel, ViewIncreaserViewModel>();
            Container.RegisterType<IBlockFollowerViewModel, BlockFollowerViewModel>();
            Container.RegisterType<ILikeCommentViewModel, LikeCommentViewModel>();
            Container.RegisterType<IHashtagsScraperViewModel, HashtagsScraperViewModel>();
            Container.RegisterType<ICreateBoardViewModel, CreateBoardViewModel>();
            Container.RegisterType<IFollowBackViewModel, FollowBackViewModel>();
            Container.RegisterType<IDeleteTweetViewModel, DeleteTweetViewModel>();
            Container.RegisterType<IMuteViewModel, MuteViewModel>();
            Container.RegisterType<ICommunityScraperViewModel, CommunityScraperViewModel>();
            Container.RegisterType<IJobScraperViewModel, JobScraperViewModel>();
            #endregion
        }
    }
}
