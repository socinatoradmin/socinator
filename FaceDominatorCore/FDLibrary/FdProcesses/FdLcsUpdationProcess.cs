namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdLcsUpdationProcess
    {
        //private readonly DominatorAccountModel _objDominatorAccountModel;

        //private readonly string JobId = string.Empty;

        //private readonly IUnityContainer _unityContainer;

        //private readonly IPostScraperConstants _postScraperConstants;

        //public FdLcsUpdationProcess(DominatorAccountModel account)
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

        //        var AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

        //        _unityContainer = AccountScopeFactory[$"{account.AccountId}_Scraper"];
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

        //public async Task StartScrapNewPostDetails()
        //{
        //    try
        //    {
        //        if (Directory.Exists(FdConstants.SaveAdsPath))
        //            Directory.Delete(FdConstants.SaveAdsPath);

        //        var fdRequestLibrary = _unityContainer.Resolve<IFdRequestLibrary>();

        //        var runningAccounts = _postScraperConstants.LstRunningAccountsAds;

        //        await Task.Delay(1000);

        //        try
        //        {

        //            bool hasMoreResults = true;

        //            while (hasMoreResults)
        //            {
        //                try
        //                {
        //                    AdDetailsResponseHandler objAdDetailsResponseHandler = await fdRequestLibrary.GetAdCountryDetails
        //                                   (_objDominatorAccountModel);

        //                    AdReactionDetailsResponseHandler objAdReactionDetailsResponseHandler=
        //                        await fdRequestLibrary.GetAdReactionDetails(_objDominatorAccountModel, objAdDetailsResponseHandler.ObjFdScraperResponseParameters.FacebookAdsDetails);

        //                    await fdRequestLibrary.UpdateLcsDetails(_objDominatorAccountModel, objAdReactionDetailsResponseHandler.ObjFdScraperResponseParameters.FacebookAdsDetails);

        //                    await Task.Delay(new Random().Next(12000,15000));
        //                }
        //                catch (ArgumentException e)
        //                {
        //                    Console.WriteLine(e.Message);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine(ex.Message);
        //                }
        //            }

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
        //}

    }
}
