using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public interface IDominatorAutoActivityViewModel
    {
        void CallRespectiveView(SocialNetworks networks);
        bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts);
    }

    public class DominatorAutoActivityViewModel : BindableBase, IDominatorAutoActivityViewModel
    {
        private readonly object _syncObject = new object();
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountCollectionViewModel _accountCollectionViewModel;
        private ICampaignStatusChange campaignStatusChange;

        private UserControl _selectedUserControl;
        /// <summary>
        /// To bind the initial view for each dominator
        /// </summary>
        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                SetProperty(ref _selectedUserControl, value);
                OnPropertyChanged(nameof(ShowContent));
                OnPropertyChanged(nameof(ShowSocial));
            }
        }

        public DelegateCommand<AccountsActivityDetailModel> GoToToolsCmd { get; }

        public DelegateCommand<ActivityDetailsModel> ChangeActivityStatusCmd { get; }
        public DelegateCommand<AccountsActivityDetailModel> ShowMoreCommand { get; }
        public ICommand CustomizeCommand { get; }
        public ICommand StopAllCommand { get; }

        void StopAllActivities()
        {
            if (Dialog.ShowCustomDialog("LangKeyStopAllActivity".FromResourceDictionary(), "LangKeyWannaStopAllActivity".FromResourceDictionary(),
                                                       "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
                return;

            // If no activity is running then return.
            if (!AccountsCollection.Any(x => x.ActivityDetailsCollections.Any(y => y.Status)))
                return;
            
            var listOfActsPerNet = AvailableNetworksActivity(NetworksFromAccCollection());

            foreach (var x in AccountsCollection)
            {
                new TaskFactory().StartNew(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            {
                                var actDetail = x.ActivityDetailsCollections[0];
                                actDetail.Status = false;
                                ChangeActivitiesStatus(actDetail, listOfActsPerNet[x.AccountNetwork],x.AccountNetwork);
                            });
                    }
                    catch (Exception ex) { ex.DebugLog(); }
                });
            }
        }

        /// <summary>
        /// To hold all accounts important activities enable status
        /// </summary>
        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; }

        public bool ShowContent => SelectedUserControl != null;
        public bool ShowSocial => SelectedUserControl == null;

        public DominatorAutoActivityViewModel(IAccountsFileManager accountsFileManager, IAccountCollectionViewModel accountCollectionViewModel)
        {
            _accountsFileManager = accountsFileManager;
            _accountCollectionViewModel = accountCollectionViewModel;
            _selectedUserControl = new UserControl();
            AccountsCollection = new ObservableCollection<AccountsActivityDetailModel>();
            campaignStatusChange = InstanceProvider.GetInstance<ICampaignStatusChange>();
            BindingOperations.EnableCollectionSynchronization(AccountsCollection, _syncObject);
            GoToToolsCmd = new DelegateCommand<AccountsActivityDetailModel>(GoToTools);
            ChangeActivityStatusCmd = new DelegateCommand<ActivityDetailsModel>(ChangeActivityStatus);
            ShowMoreCommand = new DelegateCommand<AccountsActivityDetailModel>(ShowMore);
            CustomizeCommand = new DelegateCommand(CustomizeExecute);
            StopAllCommand = new DelegateCommand(StopAllActivities);
        }

        public bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts)
        {
            try
            {
                CallRespectiveView(soicalNetworks);

                SocinatorInitialize.GetSocialLibrary(soicalNetworks)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = selectedAccounts;

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        private void CustomizeExecute()
        {
            try
            {
                var dialog = new Dialog();
                var model = GetCustomizeUiModel();
                var ui = new CustomizeAutoActivity(model);

                var custAuto = dialog.GetMetroWindow(ui, "Customize Auto Activity");
                custAuto.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                custAuto.ShowDialog();

                if (ui.ViewModel.IsSaved)
                    CallRespectiveView(SocialNetworks.Social);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ShowMore(AccountsActivityDetailModel actDetalsModel)
        {
            try
            {
                if (actDetalsModel.ShowMoreButtonText != "LangKeyMore".FromResourceDictionary())
                {
                    for (int i = actDetalsModel.ActivityDetailsCollections.Count - 1; i > 6; --i)
                        actDetalsModel.ActivityDetailsCollections.RemoveAt(i);

                    actDetalsModel.ShowMoreButtonText = "LangKeyMore".FromResourceDictionary();
                    return;
                }

                var actsListPerNet = AvailableNetworksActivity(new List<SocialNetworks> { actDetalsModel.AccountNetwork }, getImportants: false)[actDetalsModel.AccountNetwork].Others;

                var jobActConfigManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                foreach (var x in actsListPerNet)
                    AddToActDetails(x, actDetalsModel, jobActConfigManager);

                actDetalsModel.ShowMoreButtonText = "LangKeyLess".FromResourceDictionary();
            }
            catch (Exception ex) { ex.DebugLog(); }
        }

        /// <summary>
        /// To bind the respective network view for auto activity
        /// </summary>
        /// <param name="networks">pass the social network for which UI gets bind </param>
        public void CallRespectiveView(SocialNetworks networks)
        {
            try
            {
                // collect the UI
                var accountToolsView = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().AccountUserControlTools;

                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        SelectedUserControl = networks == SocialNetworks.Social ? null : accountToolsView.GetStartupToolsView());
                }
                else
                {
                    SelectedUserControl = networks == SocialNetworks.Social ? null : accountToolsView.GetStartupToolsView();
                }

                SocinatorInitialize.AccountModeActiveSocialNetwork = networks;
                // If passed network is social then initialize the account details
                if (networks == SocialNetworks.Social)
                    InitializeAccounts();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChangeActivityStatus(ActivityDetailsModel currentDataContext)
        {
            var currentAccount = AccountsCollection.FirstOrDefault(x => x.AccountId == currentDataContext.AccountId);
            if (currentAccount == null)
                return;

            var actsListPerNet = AvailableNetworksActivity(new List<SocialNetworks> { currentAccount.AccountNetwork })[currentAccount.AccountNetwork];

            ChangeActivitiesStatus(currentDataContext, actsListPerNet,currentAccount.AccountNetwork);
        }


        private void ChangeActivitiesStatus(ActivityDetailsModel currentDataContext, Activities activites,SocialNetworks socialNetworks)
        {
            if (currentDataContext.Title == ActivityType.StopAll && currentDataContext.Status)
            {
                currentDataContext.Status = false;
                return;
            }

            var account = _accountsFileManager.GetAccountById(currentDataContext.AccountId);
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            if (currentDataContext.Title == ActivityType.StopAll)
            {
                AccountsCollection.FirstOrDefault(x => x.AccountId == currentDataContext.AccountId)?.ActivityDetailsCollections?.ForEach(y =>
                {
                    if (y.Title == ActivityType.StopAll || !y.Status) return;
                    y.Status = currentDataContext.Status;
                    ChangeActivity(y, account, true);
                    campaignStatusChange.ChangeCampaignStatus(socialNetworks, account, y.Title.ToString(), false);
                });

                foreach (var act in activites.Others)
                {
                    if (jobActivityConfigurationManager[account.AccountId, act]?.IsEnabled ?? false)
                    {
                        ChangeActivity(new ActivityDetailsModel { AccountId = currentDataContext.AccountId, Title = act, Status = false }, account, true);
                        campaignStatusChange.ChangeCampaignStatus(socialNetworks, account, currentDataContext?.ActivityTitle, false);
                    }
                }
            }
            else
            {
                ChangeActivity(currentDataContext, account);
                campaignStatusChange.ChangeCampaignStatus(socialNetworks, account,currentDataContext.Title.ToString(),false);
                var getOne = AccountsCollection.FirstOrDefault(x => x.AccountId == currentDataContext.AccountId)?
                    .ActivityDetailsCollections;
                if (getOne == null)
                    return;

                ActivityType act = (activites.Important.Contains(currentDataContext.Title)
                    ? activites.Important : activites.Others).First(x => x == currentDataContext.Title);

                getOne[0].Status = jobActivityConfigurationManager[account.AccountId, act]?.IsEnabled ?? false;
            }
        }

        void ChangeActivity(ActivityDetailsModel currentDataContext, DominatorAccountModel account, bool IsStopping = false)
        {
            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var currentAccountActivity = jobActivityConfigurationManager[account.AccountId, currentDataContext.Title];
            var campaignStatus = campaignFileManager.FirstOrDefault(x => x.TemplateId == currentAccountActivity?.TemplateId)?.Status;
            if (campaignStatus == "Paused" && currentDataContext.Status)
            {
                if (!IsStopping)
                    ToasterNotification.ShowInfomation("LangKeyErrorCampaignConfigurationIsPaused".FromResourceDictionary());
                currentDataContext.Status = false;
                return;
            }

            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            var status = dominatorScheduler.ChangeAccountsRunningStatus(currentDataContext.Status, currentDataContext.AccountId,
                currentDataContext.Title);

            if (status) return;
            try
            {
                if (!IsStopping)
                    ToasterNotification.ShowInfomation(String.Format("LangKeyConfigureYourSettings".FromResourceDictionary(), currentDataContext.Title));
                currentDataContext.Status = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GoToTools(AccountsActivityDetailModel accountsActivityDetailModel)
        {
            if (accountsActivityDetailModel == null)
                return;

            SocinatorInitialize.GetSocialLibrary(accountsActivityDetailModel.AccountNetwork).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount =
                accountsActivityDetailModel.AccountName;

            CallRespectiveView(accountsActivityDetailModel.AccountNetwork);
        }

        /// <summary>
        /// To Initialize the account details with enable status 
        /// </summary>
        private void InitializeAccounts()
        {
            // if accounts count more than one means generate the activities
            AccountsCollection.Clear();

            Task.Factory.StartNew(() =>
            {
                var accountCollection = _accountCollectionViewModel.GetCopySync()
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success || x.AccountBaseModel.Status == AccountStatus.UpdatingDetails).ToList();
                
                var listOfActsPerNet = AvailableNetworksActivity(NetworksFromAccCollection(accountCollection));

                var jobActivityConfigurationManager = InstanceProvider
                                .GetInstance<IJobActivityConfigurationManager>();

                foreach (var account in accountCollection)
                {
                    try
                    {
                        // initialize the activity details
                        var accountsActivityDetailModel = new AccountsActivityDetailModel
                        {
                            AccountName = account.AccountBaseModel.UserName,
                            AccountId = account.AccountBaseModel.AccountId,
                            AccountNetwork = account.AccountBaseModel.AccountNetwork,
                            ActivityDetailsCollections = new ObservableCollection<ActivityDetailsModel>()
                        };

                        accountsActivityDetailModel.ActivityDetailsCollections
                                        .Add(new ActivityDetailsModel
                                        {
                                            Status = false,
                                            Title = ActivityType.StopAll,
                                            ActivityTitle = "LangKeyStopAllActivity".FromResourceDictionary(),
                                            AccountId = account.AccountId
                                        });

                        var important = listOfActsPerNet[account.AccountBaseModel.AccountNetwork].Important;

                        foreach (var x in important)
                        {
                            if (x.ToString() != "Try")
                            {
                                AddToActDetails(x, accountsActivityDetailModel, jobActivityConfigurationManager);
                            }
                        }
                        var othersAct = listOfActsPerNet[account.AccountBaseModel.AccountNetwork].Others;
                        var allActivity = new List<ActivityType>(important);
                        allActivity.AddRange(othersAct);

                        foreach (var x in allActivity)
                        {
                            if (!(jobActivityConfigurationManager[account.AccountId, x]?.IsEnabled ?? false)) continue;
                            accountsActivityDetailModel.ActivityDetailsCollections[0].Status = true;
                            break;
                        }

                        if (!Application.Current.Dispatcher.CheckAccess())
                        {
                            // add the item to account collection
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                lock (_syncObject)
                                {
                                    if (AccountsCollection.All(x =>
                                        x.AccountId != account.AccountBaseModel.AccountId))
                                        AccountsCollection.Add(accountsActivityDetailModel);
                                }
                            }), DispatcherPriority.Render);
                        }
                        else
                        {
                            lock (_syncObject)
                            {
                                // add the item to account collection
                                if (AccountsCollection.All(x => x.AccountId != account.AccountBaseModel.AccountId))
                                    AccountsCollection.Add(accountsActivityDetailModel);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            });

        }

        void AddToActDetails(ActivityType act, AccountsActivityDetailModel accountsActivityDetailModel, IJobActivityConfigurationManager jobActivityConfigurationManager)
        {
            try
            {
                // get the activity details                    
                var activityData = jobActivityConfigurationManager[accountsActivityDetailModel.AccountId, act];

                accountsActivityDetailModel.ActivityDetailsCollections
                    .Add(new ActivityDetailsModel
                    {
                        // if activity present then add to list with status
                        // and if activity not present then add to list with default status
                        Status = activityData != null && activityData.IsEnabled,
                        Title = act,
                        ActivityTitle = GetActivityTitle(act),
                        AccountId = accountsActivityDetailModel.AccountId
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Utils
        /// <summary>
        /// Get Important and other Activities(totally all activities) from all available network
        /// </summary>
        /// <param name="networks">Social Network</param>
        /// <param name="getImportants">condition for getting Important activities</param>
        /// <param name="getOthers">condition for getting other than important activities</param>
        /// <returns></returns>
        Dictionary<SocialNetworks, Activities> AvailableNetworksActivity(IEnumerable<SocialNetworks> networks, bool getImportants = true, bool getOthers = true)
        {
            var getCustomAuto = GetCustomizeUiModel();
            return AvailableNetworksActivity(getCustomAuto, networks, getImportants, getOthers);
        }

        /// <summary>
        /// Get Important and other Activities(totally all activities) from all available network
        /// </summary>
        /// <param name="getCustomAuto">NetworksActivityCustomizeModel model</param>
        /// <param name="networks">Social Network</param>
        /// <param name="getImportants">condition for getting Important activities</param>
        /// <param name="getOthers">condition for getting other than important activities</param>
        /// <returns></returns>
        Dictionary<SocialNetworks, Activities> AvailableNetworksActivity(NetworksActivityCustomizeModel getCustomAuto, IEnumerable<SocialNetworks> networks, bool getImportants = true, bool getOthers = true)
        {
            var returnDict = new Dictionary<SocialNetworks, Activities>();

            if (networks.Contains(SocialNetworks.Social))
                networks = networks.Where(x => x != SocialNetworks.Social);

            if (getCustomAuto?.NetworksActListCollection != null && getCustomAuto.NetworksActListCollection.Count > 0)
            {
                var newAdded = false;
                foreach (var net in networks)
                {
                    var getData = getCustomAuto.NetworksActListCollection.FirstOrDefault(x => x.SocialNetwork == net);
                    if (getData == null)
                        continue;

                    var show = getImportants ? getData.NetworkActivityTypeModelCollections.Where(x => x.IsSelected).Select(y => y.Title).ToList() : null;
                    var hide = getOthers ? getData.NetworkActivityTypeModelCollections.Where(x => !x.IsSelected).Select(y => y.Title).ToList() : null;

                    var all = show.DeepCloneObject() ?? new List<ActivityType>();
                    if (hide != null)
                        all.AddRange(hide);

                    var newOne = hide == null
                        ? new List<ActivityType>()
                        : SocinatorInitialize.GetSocialLibrary(net).GetNetworkCoreFactory().AccountUserControlTools
                            .GetOtherActivityTypes().Except(all).ToList();
                    if (newOne.Any())
                    {
                        hide?.AddRange(newOne);
                        newAdded = true;
                        newOne.ForEach(x =>
                        {
                            getCustomAuto.NetworksActListCollection?.FirstOrDefault(y => y.SocialNetwork == net)?.NetworkActivityTypeModelCollections.Add(new NetworkCustomizeActivityTypeModel
                            {
                                Network = net,
                                Title = x
                            });
                        });
                    }
                    var collected = new Activities(show, hide);
                    returnDict.Add(net, collected);
                }
                if (newAdded)
                    InstanceProvider.GetInstance<IBinFileHelper>().SaveAutoActivityCustomized(getCustomAuto);

            }
            else
            {
                foreach (var net in networks)
                {
                    var initializer = SocinatorInitialize.GetSocialLibrary(net)
                                                .GetNetworkCoreFactory()
                                                .AccountUserControlTools;
                    var collected = new Activities(getImportants ? initializer.GetImportantActivityTypes() : null,
                                                   getOthers ? initializer.GetOtherActivityTypes() : null);
                    returnDict.Add(net, collected);

                }
            }
            return returnDict;
        }

        NetworksActivityCustomizeModel GetCustomizeUiModel()
        {
            var custModel = InstanceProvider.GetInstance<IBinFileHelper>().GetCustomizedAutoActivity();
            
            if (custModel.NetworksActListCollection != null && custModel.NetworksActListCollection.Count>0 && custModel.NetworksActListCollection.ToList().Count == SocinatorInitialize.GetRegisterNetwork().Count()-1)
                return custModel;

            custModel.NetworksActListCollection = new ObservableCollection<EachNetworkActivityCustomizeModel>();

            var netActs = AvailableNetworksActivity(custModel, SocinatorInitialize.GetRegisterNetwork());

            foreach (var x in netActs)
            {
                var eachModel = new EachNetworkActivityCustomizeModel
                {
                    SocialNetwork = x.Key
                };

                x.Value.Important.ForEach(y =>
                {
                    eachModel.NetworkActivityTypeModelCollections.Add(new NetworkCustomizeActivityTypeModel
                    {
                        Network = x.Key,
                        Title = y,
                        IsSelected = true
                    });
                });

                x.Value.Others.ForEach(y =>
                {
                    eachModel.NetworkActivityTypeModelCollections.Add(new NetworkCustomizeActivityTypeModel
                    {
                        Network = x.Key,
                        Title = y
                    });
                });

                custModel.NetworksActListCollection.Add(eachModel);
            }

            return custModel;
        }

        string GetActivityTitle(ActivityType act)
        {
            var activityTitle = string.Empty;
            var titleData = act.GetDescriptionAttr()?.Split(',');
         
            activityTitle = titleData.LastOrDefault() != null && titleData.LastOrDefault().Contains("LangKey") ?
                                titleData.LastOrDefault().FromResourceDictionary() : act.ToString();
            return activityTitle;
        }

        IEnumerable<SocialNetworks> NetworksFromAccCollection(IEnumerable<DominatorAccountModel> accounts = null)
        {
            var returnList = new List<SocialNetworks>();
            if (accounts == null && AccountsCollection == null)
                return returnList;
            var list = accounts != null ? accounts.Select(x => x.AccountBaseModel.AccountNetwork) : AccountsCollection.Select(x => x.AccountNetwork);
            foreach (var each in list)
            {
                if (returnList.Contains(each))
                    continue;
                returnList.Add(each);
            }
            return returnList;
        }

        #endregion
    }

    struct Activities
    {
        public Activities(IEnumerable<ActivityType> important, IEnumerable<ActivityType> others)
        {
            Important = important;
            Others = others;
        }

        public IEnumerable<ActivityType> Important;
        public IEnumerable<ActivityType> Others;
    }
}
