namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdPostDetailsScraperThroughBrowserProcess
    {
        //private readonly DominatorAccountModel _objDominatorAccountModel;

        //private readonly string JobId = string.Empty;

        //private readonly IUnityContainer _unityContainer;

        //private readonly IPostScraperConstants _postScraperConstants;

        //private readonly IAccountScopeFactory _accountScopeFactory;

        //protected IFdBrowserManager Browsermanager { get; set; }

        //public FdPostDetailsScraperThroughBrowserProcess(DominatorAccountModel account)
        //{
        //    try
        //    {
        //        _postScraperConstants = InstanceProvider
        //            .GetInstance<IPostScraperConstants>();

        //        var runningAccounts = _postScraperConstants.LstRunningAccountsAds;

        //        _objDominatorAccountModel = account;
        //        if (!runningAccounts.Contains(_objDominatorAccountModel.AccountId))
        //        {
        //            runningAccounts.Add(account.AccountId);
        //        }

        //        _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

        //        _unityContainer = _accountScopeFactory[$"{account.AccountId}_Scraper"];

        //        Browsermanager = _unityContainer.Resolve<IFdBrowserManager>();
        //    }
        //    catch (ArgumentException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        //public static SemaphoreSlim _threadLock = new SemaphoreSlim(1, 1);
        //public async Task StartScrapNewPostDetails()
        //{
        //    try
        //    {
        //        Browsermanager.BrowserLogin(_objDominatorAccountModel, _objDominatorAccountModel.Token);

        //        if (File.Exists(FdConstants.SaveAdsPath))
        //            File.Delete(FdConstants.SaveAdsPath);

        //        var fdRequestLibrary = _unityContainer.Resolve<IFdRequestLibrary>();

        //        var runningAccounts = _postScraperConstants.LstRunningAccountsAds;

        //        List<FacebookAdsDetails> listFacebookPostDetails = new List<FacebookAdsDetails>();

        //        await Task.Delay(1000);

        //        try
        //        {
        //            var locationDetails = await fdRequestLibrary.GetIpDetails(_objDominatorAccountModel);

        //            var localIpDeails = await fdRequestLibrary.GetIpDetails(_objDominatorAccountModel, true);

        //            IResponseHandler responseHandler = null;

        //            IResponseHandler userResponseHandler = null;

        //            FacebookUser objUser = new FacebookUser();
        //            if (!string.IsNullOrEmpty(_objDominatorAccountModel.AccountBaseModel.UserId))
        //                objUser = new FacebookUser { UserId = _objDominatorAccountModel.AccountBaseModel.UserId };
        //            else
        //                objUser = new FacebookUser { Username = _objDominatorAccountModel.AccountBaseModel.UserName };


        //            userResponseHandler = Browsermanager.GetDetailedInfoUserMobileAsync(_objDominatorAccountModel, objUser);

        //            await Browsermanager.BrowserWindow.GoToCustomUrl(FdConstants.FbHomeUrl , delayAfter:10);

        //            await fdRequestLibrary.UpdateProfileInfo(_objDominatorAccountModel, objUser);

        //            int noOfPages = 0;

        //            int lastCurrentCount = -1;

        //            var lstAds = new List<FacebookAdsDetails>();

        //            try
        //            {
        //                lstAds = await Browsermanager.ScrapeSideAds(_objDominatorAccountModel, locationDetails);
        //                foreach (var ads in lstAds)
        //                {
        //                    if (await fdRequestLibrary.CheckDuplicatesFromDb(_objDominatorAccountModel, ads))
        //                        continue;
        //                    listFacebookPostDetails.Add(ads);
        //                }
        //            }
        //            catch (Exception )
        //            { }


        //            bool hasMoreResults = true;

        //            List<Tuple<int, string, string, string, string>> tuples = null;

        //            while (hasMoreResults)
        //            {
        //                try
        //                {
        //                    tuples = Browsermanager.ScrollWindowAndGetDataForAds(_objDominatorAccountModel, FbEntityType.Post, 20, noOfPages, lastCurrentCount);
        //                    FacebookAdsDetails facebookAdsDetails = null;
        //                    Browsermanager.CloseBrowser(_objDominatorAccountModel);
        //                    tuples.Reverse();

        //                    foreach (var item in tuples)
        //                    {
        //                        try
        //                        {

        //                            facebookAdsDetails = new FacebookAdsDetails();
        //                            facebookAdsDetails.AdId = item.Item3;
        //                            facebookAdsDetails.Id = item.Item2;
        //                            if (string.IsNullOrEmpty(facebookAdsDetails.Id))
        //                                continue;
        //                            facebookAdsDetails.OwnerId = item.Item4;
        //                            facebookAdsDetails.PostIndex = item.Item1;
        //                            //facebookAdsDetails.PostedDateTime = DateTimeUtilities.EpochToDateTimeUtc(int.Parse(item.Item5));

        //                            facebookAdsDetails.PostUrl = $"{FdConstants.FbHomeUrl}{facebookAdsDetails.Id}";

        //                            if (await fdRequestLibrary.CheckDuplicatesFromDb(_objDominatorAccountModel, facebookAdsDetails))
        //                            {
        //                                continue;
        //                            }

        //                            await _threadLock.WaitAsync();
        //                            IFdBrowserManager userSpecificWindow = null;
        //                            try
        //                            {
        //                                //var customBrowserWindow = InstanceProvider.GetInstance<IFdBrowserManager>();
        //                                userSpecificWindow = _accountScopeFactory[$"{_objDominatorAccountModel.AccountId}{facebookAdsDetails.Id}"].Resolve<IFdBrowserManager>();
        //                                responseHandler = userSpecificWindow.GetFullPostDetailsAds(_objDominatorAccountModel, facebookAdsDetails);
        //                                facebookAdsDetails = responseHandler.ObjFdScraperResponseParameters.FacebookAdsDetails;
        //                                userSpecificWindow.CloseBrowser(_objDominatorAccountModel);
        //                                facebookAdsDetails.AdId = item.Item3;
        //                                _threadLock.Release();
        //                                if (facebookAdsDetails.AdMediaType == AdMediaType.NoMedia || facebookAdsDetails.AdMediaType == AdMediaType.ALBUM || string.IsNullOrEmpty(facebookAdsDetails.NavigationUrl))
        //                                    continue;    
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                userSpecificWindow.CloseBrowser(_objDominatorAccountModel);
        //                                _threadLock.Release();
        //                                ex.DebugLog();
        //                                continue;
        //                            }

        //                            await Task.Delay(TimeSpan.FromSeconds(5));

        //                            facebookAdsDetails.City = locationDetails[IpLocationDetails.City];
        //                            facebookAdsDetails.State = locationDetails[IpLocationDetails.State];
        //                            facebookAdsDetails.Country = locationDetails[IpLocationDetails.Country];

        //                            //this can be added at scraping post details

        //                            listFacebookPostDetails.Add(facebookAdsDetails);
        //                        }
        //                        catch (Exception ex)
        //                        { }
        //                    }

        //                    hasMoreResults = responseHandler.HasMoreResults;

        //                    await Task.Delay(1500);
        //                }
        //                catch (ArgumentException e)
        //                {
        //                    hasMoreResults = false;
        //                    Console.WriteLine(e.Message);
        //                }
        //                catch (Exception ex)
        //                {
        //                    hasMoreResults = false;
        //                    Console.WriteLine(ex.Message);
        //                }

        //                noOfPages++;

        //                if ((localIpDeails[IpLocationDetails.Country]).Contains("India") && noOfPages >= 2)
        //                    break;
        //                else if (noOfPages > 50)
        //                    break;
        //            }



        //            listFacebookPostDetails = listFacebookPostDetails.GroupBy(x => x.Id).Select(x => x.First()).ToList();

        //            await StartFinalAdScraperProcess(_objDominatorAccountModel, fdRequestLibrary,
        //                              listFacebookPostDetails);

        //            if (runningAccounts.Contains(_objDominatorAccountModel.AccountId))
        //                runningAccounts.Remove(_objDominatorAccountModel.AccountId);

        //            if (string.IsNullOrEmpty(JobId))
        //            {
        //                JobManager.RemoveJob(JobId);
        //            }
        //        }
        //        catch (ArgumentException e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //    catch (ArgumentException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            var runningAccounts = _postScraperConstants.LstRunningAccountsAds;

        //            if (runningAccounts.Contains(_objDominatorAccountModel.AccountId))
        //                runningAccounts.Remove(_objDominatorAccountModel.AccountId);
        //        }
        //        catch (ArgumentException e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //    Browsermanager.CloseBrowser(_objDominatorAccountModel);
        //}
        //private async Task StartFinalAdScraperProcess(DominatorAccountModel account, IFdRequestLibrary fdRequestLibrary,
        //    List<FacebookAdsDetails> listFacebookPostDetails)
        //{
        //    try
        //    {
        //        listFacebookPostDetails.Reverse();
        //        foreach (FacebookAdsDetails currentAd in listFacebookPostDetails)
        //        {
        //            try
        //            {
        //                await Task.Delay((new Random().Next(5000, 10000)));

        //                await fdRequestLibrary.SaveDetailsInDb(account, currentAd);
        //                await Task.Delay(1000);
        //            }
        //            catch (ArgumentException e)
        //            {
        //                Console.WriteLine(e.Message);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.StackTrace);
        //            }
        //        }
        //    }
        //    catch (ArgumentException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //}


    }
}
