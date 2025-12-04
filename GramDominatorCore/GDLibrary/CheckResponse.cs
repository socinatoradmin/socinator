using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using System;
using System.Threading;

namespace GramDominatorCore.GDLibrary
{
    public class CheckResponse
    {

        public static bool CheckProcessResponse(IGResponseHandler response, DominatorAccountModel DominatorAccountModel,
            ActivityType ActivityType, ScrapeResultNew scrapeResult,ref int ActionBlockedCount, int delay = 0)
        {            
            try
            {
                if (response!=null)
                {
                    if (response.ToString().Contains("Please wait a few minutes before you try again.")&& !response.ToString().Contains("\"feedback_required\""))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Please wait atleast one hour before you try again its going to fast");
                        Thread.Sleep(TimeSpan.FromHours(1));
                        return true;
                    }
                    else if (response.ToString().Contains("This block will expire on"))// && response.ToString().Contains("\"feedback_required\"")
                    {
                        string expireDate = Utilities.GetBetween(response.ToString(), "This block will expire on", ".");
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                        return false;
                    }
                    else if (response.ToString().Contains("Sorry, you're following the max limit of accounts"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Sorry, you're following the max limit of accounts. You'll need to unfollow some accounts to start following more.");
                        return false;
                    }
                    else if (response.ToString().Contains("Caption too long"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName,
                       ActivityType, $"your post caption is too long please check");
                    }
                    else if (response.ToString().Contains("login_required") || response.ToString().Contains("challenge_required"))//&& response.Issue.Status == "Response Response Login session expired")
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                               $"Log in required please update your account");
                        DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                        return false;
                    }
                    else if (response.ToString().Contains("Please wait a few minutes before you try again.")|| response.ToString().Contains("\"feedback_required\"") || response.ToString().Contains("Action Blocked"))
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            scrapeResult.ResultUser?.Username ?? scrapeResult.ResultPost.Code, ": action has been blocked.");
                        if (response.ToString().Contains("It looks like you were misusing this feature by going too fast"))
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                           scrapeResult.ResultUser?.Username ?? scrapeResult.ResultPost.Code, "It looks like you were misusing this feature by going too fast. You have been temporarily blocked from using it, please try after some time");
                            return false;
                        }
                        if(response.ToString().Contains("Action Blocked"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Your account has been blocked for this {ActivityType} activity");
                            return false;
                        }
                        ActionBlockedCount++;

                        if (ActionBlockedCount >= 4)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Your account has been blocked for {ActivityType} operation for last 4 times.Hence your account {DominatorAccountModel.AccountBaseModel.UserName} Activity {ActivityType} has been stop for 12 hours");// Hence {ActivityType} activity will automatically disabled for current Job and will start on next Job");
                            Thread.Sleep(TimeSpan.FromHours(12));
                            return false;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Your account has been blocked for {ActivityType} operation. Hence {ActivityType} activity will be enable after a {delay} minutes to keep your account secure.");
                            Thread.Sleep(TimeSpan.FromMinutes(delay));
                            return false;
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, (scrapeResult.ResultPost?.Code??"") + $" {response?.Issue?.Message}");// response.Issue.Message
                        
                    } 
                }
            }
            catch (Exception)
            {
                //ignored
            }
            return true;
        }
    }
}
