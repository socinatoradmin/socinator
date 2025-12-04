using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;


namespace GramDominatorCore.GDLibrary
{
    public class AddInstagramDataIntoDatabase
    {
        public void AddDataIntoInteractedPostCampaign(ScrapeResultNew scrapeResult, string ReqData, IDbOperations CampaignDbOperation, InstagramUser userDetails,DominatorAccountModel DominatorAccountModel)
        {
            CampaignDbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
            {
                ActivityType = ActivityType.UserScraper.ToString(),
                Date = DateTimeUtilities.GetEpochTime(),
                QueryType = scrapeResult.QueryInfo.QueryType,
                Query = scrapeResult.QueryInfo.QueryValue,
                Username = DominatorAccountModel.AccountBaseModel.UserName,
                InteractedUsername = scrapeResult.ResultUser.Username,
                InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                IsPrivate = userDetails.IsPrivate,
                IsBusiness = userDetails.IsBusiness,
                IsVerified = userDetails.IsVerified,
                IsProfilePicAvailable = !userDetails.HasAnonymousProfilePicture,
                ProfilePicUrl = userDetails.ProfilePicUrl,
                RequiredData = ReqData
            });
        }
        public void AddDataIntoInteractedUserAccount(ScrapeResultNew scrapeResult, string ReqData, IDbOperations AccountDbOperation, InstagramUser userDetails, DominatorAccountModel DominatorAccountModel)
        {
            AccountDbOperation.Add(
                new InteractedUsers()
                {
                    ActivityType = ActivityType.UserScraper.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                    IsPrivate = userDetails.IsPrivate,
                    IsBusiness = userDetails.IsBusiness,
                    IsVerified = userDetails.IsVerified,
                    IsProfilePicAvailable = !userDetails.HasAnonymousProfilePicture,
                    ProfilePicUrl = userDetails.ProfilePicUrl,
                    RequiredData = ReqData
                });
        }

        //private void AddMessageDataToDataBase(ScrapeResultNew scrapeResult, string ReqData, IDbOperations CampaignDbOperation, InstagramUser userDetails, DominatorAccountModel DominatorAccountModel)
        //{
        //    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //    InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;

        //    // Add data to respected campaign InteractedUsers table
        //    try
        //    {
               

        //        for (int i = 0; i < 5; i++)
        //        {
        //            if (!string.IsNullOrEmpty(CampaignId))
        //            {
        //                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
        //                {
        //                    ActivityType = ActivityType.ToString(),
        //                    Date = DateTimeUtilities.GetEpochTime(),
        //                    DirectMessage = message,
        //                    InteractedUsername = instagramUser.Username,
        //                    InteractedUserId = instagramUser.Pk ?? instagramUser.UserId,
        //                    Username = DominatorAccountModel.AccountBaseModel.UserName
        //                });
        //            }
        //            bool isAdd = AccountDbOperation.Add(
        //           new InteractedUsers()
        //           {
        //               ActivityType = ActivityType.ToString(),
        //               Date = DateTimeUtilities.GetEpochTime(),
        //               DirectMessage = message,
        //               InteractedUsername = instagramUser.Username,
        //               InteractedUserId = instagramUser.Pk ?? instagramUser.UserId,
        //               Username = DominatorAccountModel.AccountBaseModel.UserName
        //           });

        //            if (isAdd)
        //                break;
        //            Thread.Sleep(new Random().Next(3000, 5000));
        //        }
        //        // var demo = AccountDbOperation.Get<InteractedUsers>();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}


    }
}
