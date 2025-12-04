using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.CommonResponse;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThreadUtils;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class FdPostDetailsScraperProcess
    {
        private readonly DominatorAccountModel _objDominatorAccountModel;

        private readonly string JobId = string.Empty;

        private readonly IUnityContainer _unityContainer;

        private readonly IPostScraperConstants _postScraperConstants;
        private readonly IDelayService _delayService;

        public FdPostDetailsScraperProcess(DominatorAccountModel account)
        {
            try
            {
                _postScraperConstants = InstanceProvider
                    .GetInstance<IPostScraperConstants>();
                _delayService = InstanceProvider.GetInstance<IDelayService>();

                var runningAccounts = _postScraperConstants.LstRunningAccountsAds;

                _objDominatorAccountModel = account;
                if (!runningAccounts.Contains(_objDominatorAccountModel.AccountId))
                {
                    runningAccounts.Add(account.AccountId);
                }

                var AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

                _unityContainer = AccountScopeFactory[$"{account.AccountId}_Scraper"];
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task StartScrapNewPostDetails()
        {
            try
            {
                if (Directory.Exists(FdConstants.SaveAdsPath))
                    Directory.Delete(FdConstants.SaveAdsPath);

                var fdRequestLibrary = _unityContainer.Resolve<IFdRequestLibrary>();

                List<FacebookAdsDetails> listFacebookPostDetails = new List<FacebookAdsDetails>();

                await _delayService.DelayAsync(1000);

                try
                {

                    var locationDetails = await fdRequestLibrary.GetIpDetails(_objDominatorAccountModel);

                    var localIpDeails = await fdRequestLibrary.GetIpDetails(_objDominatorAccountModel, true);

                    ScrapNewPostListFromNewsFeedResponseHandler objScrapAdsListFromNewsFeedResponseHandler = null;

                    bool hasMoreResults = true;

                    await fdRequestLibrary.ScrapOwnProfileInfoAsync(_objDominatorAccountModel);

                    int noOfPages = 0;

                    while (hasMoreResults)
                    {
                        try
                        {
                            objScrapAdsListFromNewsFeedResponseHandler = await fdRequestLibrary.GetPostListFromNewsFeedAsync
                                           (_objDominatorAccountModel, objScrapAdsListFromNewsFeedResponseHandler);

                            listFacebookPostDetails.AddRange(objScrapAdsListFromNewsFeedResponseHandler.ListFacebookAdsDetails);

                            hasMoreResults = objScrapAdsListFromNewsFeedResponseHandler.HasMoreResults;

                            await _delayService.DelayAsync(3000);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        noOfPages++;

                        if ((localIpDeails[IpLocationDetails.Country]).Contains("India") && noOfPages > 25)
                            break;
                        else if (noOfPages > 50)
                            break;
                    }

                    listFacebookPostDetails = listFacebookPostDetails.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                    FdConstants.ListOfAds.DeepCloneObject().ForEach(x =>
                    {
                        var ad = new FacebookAdsDetails();
                        FdConstants.ListOfAds.TryRemove(x.Key, out ad);
                        listFacebookPostDetails.Add(ad);
                    });

                    await StartFinalAdScraperProcess(_objDominatorAccountModel, fdRequestLibrary,
                                      listFacebookPostDetails, locationDetails);

                    if (string.IsNullOrEmpty(JobId))
                    {
                        JobManager.RemoveJob(JobId);
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }
        }

        private async Task StartFinalAdScraperProcess(DominatorAccountModel account, IFdRequestLibrary fdRequestLibrary,
            List<FacebookAdsDetails> listFacebookPostDetails, Dictionary<IpLocationDetails, string> ipDetails)
        {
            try
            {
                var composerId = string.Empty;

                await _delayService.DelayAsync((new Random().Next(5000, 10000)));

                foreach (FacebookAdsDetails currentAd in listFacebookPostDetails)
                {
                    try
                    {

                        currentAd.City = ipDetails[IpLocationDetails.City];
                        currentAd.State = ipDetails[IpLocationDetails.State];
                        currentAd.Country = ipDetails[IpLocationDetails.Country];

                        if (await fdRequestLibrary.CheckDuplicatesFromDb(account, currentAd))
                        {
                            continue;
                        }

                        if (currentAd.AdType != "SIDE")
                        {
                            var responseHandler = await fdRequestLibrary.GetPostDetailWithDestinationUrl(account, currentAd, composerId);
                            composerId = string.IsNullOrEmpty(responseHandler.ComposerId) ? composerId : responseHandler.ComposerId;
                        }
                        else
                            composerId = currentAd.ComposerId;

                        if (currentAd.Title.Contains("Facebook - Log In or Sign Up") ||
                            currentAd.SubDescription.Contains("Create an account or log into Facebook. Connect with friends, family and other people you know. Share photos and videos, send messages and get updates"))
                        {
                            continue;
                        }
                        await fdRequestLibrary.ViewersDetailsParser(account, currentAd);
                        await _delayService.DelayAsync(2000);

                        if (currentAd == null)
                            continue;

                        if (string.IsNullOrEmpty(currentAd.OwnerId))
                        {
                            if (currentAd.AdType == "SIDE")
                                FdConstants.ListOfAds.TryAdd(currentAd.AdId, currentAd);

                            continue;
                        }

                        var pageDetails = await fdRequestLibrary.GetPageDetails(account, currentAd);
                        await _delayService.DelayAsync(2000);

                        if (pageDetails != null && !string.IsNullOrEmpty(pageDetails.ObjFacebookAdsDetails.OwnerCategory))
                        {
                            currentAd.OwnerId = pageDetails.ObjFacebookAdsDetails.OwnerId;
                            currentAd.OwnerName = pageDetails.ObjFacebookAdsDetails.OwnerName;
                            currentAd.OwnerCategory = pageDetails.ObjFacebookAdsDetails.OwnerCategory;
                            currentAd.OwnerLogoUrl = pageDetails.ObjFacebookAdsDetails.OwnerLogoUrl;
                        }
                        else
                            continue;

                        if (currentAd.AdType != "SIDE")
                            await fdRequestLibrary.ScrapeComments(account, currentAd);
                        await _delayService.DelayAsync(2000);
                        await fdRequestLibrary.SaveDetailsInDb(account, currentAd);
                        await _delayService.DelayAsync(2000);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

}
