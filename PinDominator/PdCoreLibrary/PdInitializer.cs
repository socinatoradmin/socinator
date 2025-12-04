using DominatorHouseCore.Enums;
using PinDominator.Utility.Boards.Accept_Board_Invitation;
using PinDominator.Utility.Boards.Create_Board;
using PinDominator.Utility.Boards.Send_Board_Invitation;
using PinDominator.Utility.CreateAccount;
using PinDominator.Utility.Grow_Followers.Follow;
using PinDominator.Utility.Grow_Followers.Follow_Back;
using PinDominator.Utility.Grow_Followers.Unfollow;
using PinDominator.Utility.Pin_Messanger.Auto_Reply_To_New_Message;
using PinDominator.Utility.Pin_Messanger.Broadcast_Messages;
using PinDominator.Utility.Pin_Messanger.Send_Message_To_New_Followers;
using PinDominator.Utility.Pin_Try_Comment.Comment;
using PinDominator.Utility.Pin_Try_Comment.Try;
using PinDominator.Utility.PinPoster.DeletePin;
using PinDominator.Utility.PinPoster.EditPin;
using PinDominator.Utility.PinPoster.RePin;
using PinDominator.Utility.PinScrap.Board_Scraper;
using PinDominator.Utility.PinScrap.Pin_Scraper;
using PinDominator.Utility.PinScrap.User_Scraper;
using PinDominatorCore.Utility;

namespace PinDominator.PdCoreLibrary
{
    public class PdInitializer
    {
        public static void RegisterModules()
        {
            PinterestInitialize.PdModulesRegister(ActivityType.CreateBoard, new CreateBoardBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.AcceptBoardInvitation,
                new AcceptBoardInvitationBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.SendBoardInvitation,
                new SendBoardInvitationBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.Follow, new FollowBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.FollowBack, new FollowBackBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.Unfollow, new UnfollowBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.Repin, new RePinBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.DeletePin, new DeletePinBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.EditPin, new EditPinBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.Try, new TryBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.BroadcastMessages, new BroadCastMessagesBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.AutoReplyToNewMessage,
                new AutoReplyToNewMessageBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.SendMessageToFollower,
                new SendMessageToNewFollowrsBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.UserScraper, new UserScraperBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.BoardScraper, new BoardScraperBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.PinScraper, new PinScraperBaseFactory());
            PinterestInitialize.PdModulesRegister(ActivityType.CreateAccount, new AccountCreatorBaseFactory());
        }
    }
}