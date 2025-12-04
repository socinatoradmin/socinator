#region

using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using FluentScheduler;

#endregion

namespace DominatorHouseCore.Settings
{
    public interface ISoftwareSettings
    {
        void InitializeOnLoadConfigurations();
        void ScheduleAutoUpdation();
        Task ScheduleAdsScraping(SocialNetworks currentNetwork = SocialNetworks.Facebook);

        Task<bool> ScrapAdsProduceAsync(ActionBlock<ScrapAdsDetails> adsActionBuffer,
            DominatorAccountModel currentAccount = null
            , SocialNetworks currentNetwork = SocialNetworks.Facebook);

        SoftwareSettingsModel Settings { get; set; }
        bool Save();
        ObservableCollection<LocationModel> AssignLocationList();
    }

    public class SoftwareSettings : BindableBase, ISoftwareSettings
    {
        private readonly ISoftwareSettingsFileManager _softwareSettingsFileManager;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IGenericFileManager _genericFileManager;

        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountSyncManager SyncManager;
        public SoftwareSettings(ISoftwareSettingsFileManager softwareSettingsFileManager,
            IFileSystemProvider fileSystemProvider, IGenericFileManager genericFileManager,
            IAccountsFileManager accountsFileManager)
        {
            _softwareSettingsFileManager = softwareSettingsFileManager;
            _fileSystemProvider = fileSystemProvider;
            _genericFileManager = genericFileManager;
            _accountsFileManager = accountsFileManager;
            SyncManager = InstanceProvider.GetInstance<IAccountSyncManager>();
        }

        private SoftwareSettingsModel _settings;

        public SoftwareSettingsModel Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public void InitializeOnLoadConfigurations()
        {
            CheckSocinatorIcon();

            if (CheckConfigurationFiles())
            {
                Settings = _softwareSettingsFileManager.GetSoftwareSettings();
                if (!(Settings.RunQueriesRandomly || Settings.RunQueriesBottomToTop || Settings.RunQueriesTopToBottom))
                    Settings.RunQueriesRandomly = true;
                if (!Settings.SortByNikename && !Settings.SortByUsername)
                    Settings.SortByUsername = true;
            }

            //OtherInitializers();


            if (_fileSystemProvider.Exists(ConstantVariable.GetURLShortnerServicesFile()))
            {
                var shortnerServices =
                    _genericFileManager.GetModel<UrlShortnerServicesModel>(
                        ConstantVariable.GetURLShortnerServicesFile());
                ConstantVariable.BitlyLogin = shortnerServices.Login;
                ConstantVariable.BitlyApiKey = shortnerServices.ApiKey;
            }
        }

        private bool CheckConfigurationFiles()
        {
            if (!_fileSystemProvider.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                Settings = new SoftwareSettingsModel
                {
                    IsEnableAdvancedUserMode = true,
                    IsStopAutoSynchronizeAccount = true
                };
                if (!(Settings.RunQueriesRandomly || Settings.RunQueriesBottomToTop || Settings.RunQueriesTopToBottom))
                    Settings.RunQueriesRandomly = true;

                _softwareSettingsFileManager.SaveSoftwareSettings(Settings);

                return false;
            }

            return true;
        }

        public bool Save()
        {
            return _softwareSettingsFileManager.SaveSoftwareSettings(Settings);
        }

        private void CheckSocinatorIcon()
        {
            if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
            {
                FileUtilities.Copy(ConstantVariable.MyAppFolderPath + @"\" + $"{"LangKeySocinator".FromResourceDictionary()}{ConstantVariable.GetIconExtension()}",
                    ConstantVariable.GetSocinatorIcon());
                if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
                    Utilities.DownloadSocinatorIcon();
            }
        }

        #region Producer Consumer Solution for Account Update

        public void ScheduleAutoUpdation()
        {
            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var socinatorSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!socinatorSettings.IsStopAutoSynchronizeAccount)
                return;

            var cancellationtokenSource = new CancellationTokenSource();

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;
            JobManager.AddJob(() =>
            {
                var registeredNetwork = SocinatorInitialize.GetRegisterNetwork();
                var accounts = _accountsFileManager.GetAll().Where(x =>
                    registeredNetwork.Contains(x.AccountBaseModel.AccountNetwork));

                var accountsToUpdate = accounts.Where(x =>
                    DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();
                if (accountsToUpdate.Count != 0)
                    Task.Factory.StartNew(async () =>
                    {
                        foreach(var account in accountsToUpdate)
                        {
                            while (await SyncManager.GetBrowserCount() >= socinatorSettings.SimultaneousAccountUpdateCount)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(10), cancellationtokenSource.Token);
                            }
                            await SyncManager.AddBrowser(account?.AccountBaseModel?.UserName);
                            UpdateAccount(account, cancellationtokenSource);
                            await Task.Delay(TimeSpan.FromSeconds(2), cancellationtokenSource.Token);
                        }
                        //accountsToUpdate.ForEach(async account =>
                        //{
                        //    while (await SyncManager.GetBrowserCount() >= socinatorSettings.SimultaneousAccountUpdateCount)
                        //    {
                        //        await Task.Delay(TimeSpan.FromSeconds(10), cancellationtokenSource.Token);
                        //    }
                        //    await SyncManager.AddBrowser(account?.AccountBaseModel?.UserName);
                        //    UpdateAccount(account, cancellationtokenSource);
                        //    Thread.Sleep(TimeSpan.FromSeconds(2));
                        //});
                        await SyncManager.RemoveAll();
                    }, cancellationtokenSource.Token);
            }, x => x.ToRunNow().AndEvery(accountSynchronizationHours).Hours().At(5));
        }

        #region Old AutoSchedule code

        //private void AccountUpdateProducer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource, int accountSynchronizationHours)
        //{
        //    var accounts = _accountsFileManager.GetAll();

        //    var accountsToUpdate = accounts.Where(x =>
        //        DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

        //    #region Schedule jobs for account update

        //    var scheduleUpdateAccount = accounts.Except(accountsToUpdate);

        //    foreach (var account in scheduleUpdateAccount)
        //    {
        //        var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeUtc();

        //        JobManager.AddJob(() =>
        //        {
        //            try
        //            {
        //                accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
        //                if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
        //                    Thread.Sleep(5000);
        //            }
        //            catch (ArgumentException ex)
        //            {
        //                ex.DebugLog();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //        }, s => s.ToRunOnceAt(dateTime).AndEvery(accountSynchronizationHours).Hours());
        //    }

        //    #endregion

        //    foreach (var account in accountsToUpdate)
        //    {
        //        try
        //        {
        //            var status = accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
        //            if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
        //                Thread.Sleep(15000);
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            accountUpdateCollection.CompleteAdding();
        //            break;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }

        //    accountUpdateCollection.CompleteAdding();

        //}

        //private void AccountUpdateConsumer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource)
        //{
        //    while (!accountUpdateCollection.IsCompleted)
        //    {
        //        try
        //        {
        //            DominatorAccountModel dominatorAccountModel;

        //            if (accountUpdateCollection.TryTake(out dominatorAccountModel, -1, cancellationTokenSource.Token))
        //            {
        //                UpdateAccount(dominatorAccountModel, cancellationTokenSource);
        //            }

        //            Thread.SpinWait(500000);
        //        }
        //        catch (OperationCanceledException ex)
        //        {
        //            ex.DebugLog("Operation Cancelled!");
        //            break;
        //        }
        //    }
        //}

        #endregion


        public void UpdateAccount(DominatorAccountModel account, CancellationTokenSource cancellationTokenSource)
        {
            var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory().AccountUpdateFactory;

            var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

            if (asyncAccount == null)
            {
                var removed = SyncManager.RemoveBrowser(account?.AccountBaseModel?.UserName).Result;
                return;
            }
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

                    if (!checkResult)
                        return;

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    await asyncAccount.UpdateDetailsAsync(account, cancellationTokenSource.Token);

                    SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                        .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                        .SaveToBinFile();
                    var globalDbOperation =
                        new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                    globalDbOperation.UpdateAccountDetails(account);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    await SyncManager.RemoveBrowser(account?.AccountBaseModel?.UserName);
                }
            }, account.Token);
        }

        #endregion

        public async Task ScheduleAdsScraping(SocialNetworks currentNetwork = SocialNetworks.Facebook)
        {
            var adScraperblock = new ActionBlock<ScrapAdsDetails>(
                async job => { await job.StartAdScarperAsync(); },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            await ScrapAdsProduceAsync(adScraperblock, null, currentNetwork);

            //var adScraperblockQuora = new ActionBlock<ScrapAdsDetails>(
            //    async job =>
            //    {
            //        await job.StartAdScarperAsync();
            //    },
            //    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 });

            //await ScrapAdsProduceAsync(adScraperblockQuora, currentNetwork: SocialNetworks.Quora);

            //var adScraperblockTikTok = new ActionBlock<ScrapAdsDetails>(
            //    async job =>
            //    {
            //        await job.StartAdScarperAsync();
            //    },
            //    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 });

            //await ScrapAdsProduceAsync(adScraperblockQuora, currentNetwork: SocialNetworks.TikTok);
        }


        public async Task<bool> ScrapAdsProduceAsync(ActionBlock<ScrapAdsDetails> adsActionBuffer,
            DominatorAccountModel currentAccount = null, SocialNetworks currentNetwork = SocialNetworks.Facebook)
        {
            var accounts = _accountsFileManager.GetAll(currentNetwork);

            if (currentAccount != null)
                accounts = accounts.Where(x => x.AccountId == currentAccount.AccountId).ToList();

            accounts.Shuffle();

            foreach (var account in accounts)
                await adsActionBuffer.SendAsync(new ScrapAdsDetails(account, adsActionBuffer));

            return true;
        }

        public ObservableCollection<LocationModel> AssignLocationList()
        {
            var countrySet = new ObservableCollection<LocationModel>();
            try
            {
                var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                var dbGlobalContext = dataBaseConnectionGlb?.GetSqlConnection();
                var _dbGlobalListOperations = new DbOperations(dbGlobalContext);
                var locationList = _dbGlobalListOperations?.Get<LocationList>();

                new NonStaticUtilities().CountriesList().ForEach(x =>
                {
                    var country = locationList?.FirstOrDefault(y => y.CountryName.Equals(x, StringComparison.OrdinalIgnoreCase));
                    countrySet.Add(new LocationModel
                    {
                        CountryName = x,
                        CityName = country?.CityName ?? string.Empty,
                        IsSelected = country != null
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return countrySet;
        }
    }

    public class ScrapAdsDetails
    {
        public string AccountId { get; set; }

        public DominatorAccountModel account { get; set; }

        public ActionBlock<ScrapAdsDetails> _adsActionBuffer { get; set; }
        private ConcurrentDictionary<SocialNetworks, AdUpdationType> CurrentUpdationType { get; set; } = new ConcurrentDictionary<SocialNetworks, AdUpdationType>();
        public ScrapAdsDetails(DominatorAccountModel AccountModel, ActionBlock<ScrapAdsDetails> adsActionBuffer)
        {
            account = AccountModel;
            _adsActionBuffer = adsActionBuffer;
            if (CurrentUpdationType.Count == 0)
            {
                CurrentUpdationType.TryAdd(SocialNetworks.Facebook, AdUpdationType.FbAds);
                CurrentUpdationType.TryAdd(SocialNetworks.TikTok, AdUpdationType.Ads);
                CurrentUpdationType.TryAdd(SocialNetworks.Reddit, AdUpdationType.RedditAds);
                CurrentUpdationType.TryAdd(SocialNetworks.Quora, AdUpdationType.QuoraAds);
            }
        }

        public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken
        {
            get;
            set;
        }
            = new ConcurrentDictionary<string, CancellationTokenSource>();


        public async Task StartAdScarperAsync()
        {
            try
            {
                var cancellationTokenSource =
                    AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

                //var postScraperConstants = InstanceProvider
                //    .GetInstance<IPostScraperConstants>();

                //if ((DateTime.Now - postScraperConstants.LastLcsJobTime).TotalHours > 10000)
                //    postScraperConstants.LastLcsJobTime = DateTime.Now.Subtract(TimeSpan.FromHours(4));

                CurrentUpdationType.TryGetValue(account.AccountBaseModel.AccountNetwork, out AdUpdationType currentUpdationType);

                //currentUpdationType = account.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook
                //    ?
                //    AdUpdationType.FbAds
                //    :
                //    account.AccountBaseModel.AccountNetwork == SocialNetworks.TikTok
                //        ? AdUpdationType.Ads
                //        :account.AccountBaseModel.AccountNetwork==SocialNetworks.Quora?AdUpdationType.QuoraAds:
                //        AdUpdationType.RedditAds;

                var asyncAdScraperFactory =
                    InstanceProvider.GetInstance<IAdScraperFactory>(currentUpdationType.ToString());

                if (asyncAdScraperFactory == null)
                    return;

                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var checkResult =
                        await asyncAdScraperFactory.CheckStatusAsync(account, cancellationTokenSource.Token);

                    var jobId = Guid.NewGuid().ToString();

                    if (checkResult)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        await asyncAdScraperFactory.ScrapeAdsAsync(account, cancellationTokenSource.Token);

                        JobManager.AddJob(
                            async () =>
                            {
                                await InstanceProvider.GetInstance<ISoftwareSettings>()
                                    .ScrapAdsProduceAsync(_adsActionBuffer, account,
                                        account.AccountBaseModel.AccountNetwork);
                            },
                            s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddHours(2)));
                    }
                    else
                    {
                        JobManager.AddJob(
                            async () =>
                            {
                                await InstanceProvider.GetInstance<ISoftwareSettings>()
                                    .ScrapAdsProduceAsync(_adsActionBuffer, account,
                                        account.AccountBaseModel.AccountNetwork);
                            },
                            s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddHours(2)));
                    }
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}