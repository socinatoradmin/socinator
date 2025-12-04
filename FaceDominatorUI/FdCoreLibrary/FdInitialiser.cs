using DominatorHouseCore;
using DominatorHouseCore.Enums;
using FaceDominatorCore.Utility;
using FaceDominatorUI.Utilities.Events.EventCreater;
using FaceDominatorUI.Utilities.Friends.CancelSentRequest;
using FaceDominatorUI.Utilities.Friends.IncommingFriendRequest;
using FaceDominatorUI.Utilities.Friends.SendFriendRequest;
using FaceDominatorUI.Utilities.Friends.Unfollow;
using FaceDominatorUI.Utilities.Friends.Unfriend;
using FaceDominatorUI.Utilities.Groups.GroupJoiner;
using FaceDominatorUI.Utilities.Groups.GroupUnjoiner;
using FaceDominatorUI.Utilities.Groups.MakeGroupAdmin;
using FaceDominatorUI.Utilities.Inviter.EventInviter;
using FaceDominatorUI.Utilities.Inviter.GroupInviter;
using FaceDominatorUI.Utilities.Inviter.PageInviter;
using FaceDominatorUI.Utilities.Inviter.WatchPartyInviter;
using FaceDominatorUI.Utilities.LikerCommentor.CommentLiker;
using FaceDominatorUI.Utilities.LikerCommentor.FanpageLiker;
using FaceDominatorUI.Utilities.LikerCommentor.PostCommentor;
using FaceDominatorUI.Utilities.LikerCommentor.PostLiker;
using FaceDominatorUI.Utilities.LikerCommentor.PostLikerCommentor;
using FaceDominatorUI.Utilities.LikerCommentor.ReplyToComment;
using FaceDominatorUI.Utilities.Messanger;
using FaceDominatorUI.Utilities.Messanger.AutoReplyMessage;
using FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends;
using FaceDominatorUI.Utilities.Messanger.SendMessageToNewFriends;
using FaceDominatorUI.Utilities.Scraper.CommentScraper;
using FaceDominatorUI.Utilities.Scraper.CommnetrRepliesScraper;
using FaceDominatorUI.Utilities.Scraper.DownloadMedia;
using FaceDominatorUI.Utilities.Scraper.FanpageScraper;
using FaceDominatorUI.Utilities.Scraper.GroupScraper;
using FaceDominatorUI.Utilities.Scraper.PostScraper;
using FaceDominatorUI.Utilities.Scraper.ProfileScraper;
using System;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdInitialiser
    {
        public static void RegisterModules()
        {
            try
            {
                FacebookInitialize.FdModulesRegister(ActivityType.SendFriendRequest,
                    new SendFriendRequestBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.Unfriend, new UnFriendRequestBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.IncommingFriendRequest,
                    new IncommingFriendRequestBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.GroupJoiner, new GroupJoinerBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.GroupUnJoiner, new GroupUnjoinerBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.FanpageLiker, new FanpageLikerBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PostLikerCommentor,
                    new PostLikerCommentorBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.ProfileScraper, new ProfileScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.FanpageScraper, new FanpageScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.GroupScraper, new GroupScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.CommentScraper, new CommentScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PostScraper, new PostScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.BroadcastMessages, new BrodcastMessageBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.WithdrawSentRequest, new CancelRequestBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.AutoReplyToNewMessage,
                    new AutoReplyMessageBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PostLiker, new PostLikerBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PostCommentor, new PostCommentorBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.GroupInviter, new GroupInviterBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PageInviter, new FanpageInviterBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.LikeComment, new CommentLikerBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.ReplyToComment, new ReplyToCommentBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.DownloadScraper, new DownloadMediaBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.EventInviter, new EventInviterBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.SendMessageToNewFriends,
                    new SendMesageToNewFriendBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.WatchPartyInviter,
                    new WatchPartyInviterBaseFactory());


                FacebookInitialize.FdModulesRegister(ActivityType.SendGreetingsToFriends,
                    new SendGreetingsToFriendsBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.MessageToFanpages,
                    new MessageToFanpagesBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.MessageToPlaces, new MessageToPlacesBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.PlaceScraper, new PlaceScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.EventCreator, new EventCreaterBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.MakeAdmin, new MakeGroupAdminBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.CommentRepliesScraper,
                    new CommnetrRepliesScraperBaseFactory());

                FacebookInitialize.FdModulesRegister(ActivityType.Unfollow, new UnFollowRequestBaseFactory());

                //FacebookInitialize.FdModulesRegister(ActivityType.WebPostLikeComment, new WebPostCommentLikerBaseFactory());
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}