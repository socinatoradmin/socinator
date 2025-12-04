using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.Converters;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel
{
    public class CampaignViewModel : BindableBase
    {
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDataBaseHandler _dataBaseHandler;
        public CancellationTokenSource CancellationSource = new CancellationTokenSource();

        private string _filterByName;

        public string FilterByName
        {
            get { return _filterByName; }
            set { SetProperty(ref _filterByName, value); }
        }


        public CampaignViewModel()
        {
            _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            _dataBaseHandler = InstanceProvider.GetInstance<IDataBaseHandler>();
            SettingCommand = new BaseCommand<object>(sender => true, SettingExecute);
            DeleteCommand = new BaseCommand<object>(sender => true, DeleteExecute);
            EditCommand = new BaseCommand<object>(sender => true, EditExecute);
            DuplicateCommand = new BaseCommand<object>(sender => true, DuplicateExecute);
            StatusChangeCommand = new BaseCommand<object>(sender => true, StatusChangeExecute);
            ReportCommand = new BaseCommand<object>(sender => true, ReportExecute);
            SelectedAccountsCommand = new BaseCommand<object>(sender => true, ShowSelectedAccountsExecute);
            ExportReportCommand = new BaseCommand<object>(sender => true, ExportReportExecute);
            CampaignTypeSelectionChange = new BaseCommand<object>(sender => true, FilterCampaign);
            SelectionCommand = new BaseCommand<object>(sender => true, SelectionExecute);
            CopyCampaignIdCommand = new BaseCommand<object>(sender => true, CopyCampaignIdExecute);
            FilterByNameCommand = new BaseCommand<object>(sender => true, FilterByNameExecute);
            LoadedCommand = new BaseCommand<object>(sender => true, LoadCampaign);
            BindingOperations.EnableCollectionSynchronization(LstCampaignDetails, _lock);
        }

        private DbOperations _dbOperations { get; }

        public void LoadCampaign(object sender)
        {
            try
            {
                FilterCampaign(sender);
                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                if (LstCampaignDetails.Count == campaignFileManager.Count())
                    return;

                Task.Factory.StartNew(() =>
                {
                    campaignFileManager.ForEach(camp =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (LstCampaignDetails.All(x => x.CampaignId != camp.CampaignId))
                            {
                                LstCampaignDetails.Add(camp);
                                FilterCampaign(sender);
                                FilterByNameExecute(sender);
                            }
                        }), DispatcherPriority.Render);
                        Thread.Sleep(5);
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChangeAllCampStatus()
        {
            if (LstCampaignDetails.Any(x =>
                x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork && x.Status == "Active"))
                AllCampStatus = true;
            else
                AllCampStatus = false;
        }

        public void SetActivityTypes()
        {
            CampaignModel.ActivityType.Add("All");

            foreach (var name in Enum.GetNames(typeof(ActivityType)))
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name))
                    .Contains(SocialNetworks.ToString()) && name != "Try")
                    CampaignModel.ActivityType.Add(name);
        }

        private void ReportExecute1(object sender)
        {
            try
            {
                var campName = sender as CampaignDetails;
                if (campName != null)
                {
                    var reportControl = new Reports(campName);
                    var objDialog = new Dialog();

                    var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

                    var activitySettings = templatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campName.SubModule);

                    var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks)
                        .GetNetworkCoreFactory();

                    var lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, activitySettings);

                    lstSavedQuery?.ToList().ForEach(x =>
                    {
                        reportControl.ReportModel.LstCurrentQueries.Add(
                            new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));

                        #region Update QueryList for combobox

                        if (reportControl.ReportModel.QueryList.All(query => query.Content != x.QueryType))
                            reportControl.ReportModel.QueryList.Add(new ContentSelectGroup { Content = x.QueryType });

                        #endregion
                    });
                    try
                    {
                        #region Update AccountList & StatusList for combobox

                        campName.SelectedAccountList.ToList().ForEach(acc =>
                        {
                            var objDominatorAccountModel =
                                _accountsFileManager.GetAccount(acc, campName.SocialNetworks);

                            reportControl.ReportModel.AccountList.Add(new ContentSelectGroup
                            {
                                IsContentSelected = false,
                                Content = objDominatorAccountModel.AccountBaseModel.UserName
                            });

                            if (reportControl.ReportModel.StatusList.Count > 1 &&
                                reportControl.ReportModel.StatusList.All(status =>
                                    status.Content != objDominatorAccountModel.AccountBaseModel.Status.ToString()))
                                reportControl.ReportModel.StatusList.Add(new ContentSelectGroup
                                {
                                    IsContentSelected = false,
                                    Content = objDominatorAccountModel.AccountBaseModel.Status.ToString()
                                });
                        });

                        #endregion

                        var reportDetails = networkCoreFactory.ReportFactory.GetReportDetail(reportControl.ReportModel,
                            reportControl.ReportModel.LstCurrentQueries, campName);
                        if (reportDetails.Count == 0)
                        {
                            Dialog.ShowDialog("LangKeyReport".FromResourceDictionary(),
                                $"{"LangKeyReportForCampaign".FromResourceDictionary()} {campName.CampaignName} {"LangKeyIsNotAvailable".FromResourceDictionary()}");
                            return;
                        }

                        var win = objDialog.GetMetroWindow(reportControl, "LangKeyReport".FromResourceDictionary());
                        win.Owner = Application.Current.MainWindow;
                        win.WindowStartupLocation = WindowStartupLocation.Manual;
                        win.Top = 0;
                        win.Left = 0;
                        reportControl.ReportModel.LstReports = new ObservableCollection<object>();
                        reportControl.ReportModel.ReportCollection =
                            CollectionViewSource.GetDefaultView(reportControl.ReportModel.LstReports);

                        Task.Factory.StartNew(() =>
                        {
                            reportDetails.ForEach(item =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                    reportControl.ReportModel.LstReports.Add(item));
                                Thread.Sleep(10);
                            });
                        });

                        win.ShowDialog();
                    }
                    catch (AggregateException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void ReportExecute(object sender)
        {
            try
            {
                var campName = sender as CampaignDetails;
                if (campName != null)
                {
                    var reportControl = new Reports(campName);
                    var objDialog = new Dialog();

                    var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

                    var activitySettings = templatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

                    var activityType = campName.SubModule.ToActivityType();

                    var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks)
                        .GetNetworkCoreFactory();

                    var lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, activitySettings);

                    lstSavedQuery?.ToList().ForEach(x =>
                    {
                        reportControl.ReportModel.LstCurrentQueries.Add(
                            new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));

                        #region Update QueryList for combobox

                        if (reportControl.ReportModel.QueryList.All(query => query.Content != x.QueryType))
                            reportControl.ReportModel.QueryList.Add(new ContentSelectGroup { Content = x.QueryType });

                        #endregion
                    });
                    try
                    {
                        #region Update AccountList & StatusList for combobox

                        campName.SelectedAccountList.ToList().ForEach(acc =>
                        {
                            var objDominatorAccountModel =
                                _accountsFileManager.GetAccount(acc, campName.SocialNetworks);
                            if (objDominatorAccountModel is null)
                                return;
                            reportControl.ReportModel.AccountList.Add(new ContentSelectGroup
                            {
                                IsContentSelected = false,
                                Content = objDominatorAccountModel.AccountBaseModel.UserName
                            });

                            if (reportControl.ReportModel.StatusList.Count > 1 &&
                                reportControl.ReportModel.StatusList.All(status =>
                                    status.Content != objDominatorAccountModel.AccountBaseModel.Status.ToString()))
                                reportControl.ReportModel.StatusList.Add(new ContentSelectGroup
                                {
                                    IsContentSelected = false,
                                    Content = objDominatorAccountModel.AccountBaseModel.Status.ToString()
                                });
                        });

                        #endregion

                        var reportDetails = networkCoreFactory.ReportFactory.GetReportDetail(reportControl.ReportModel,
                            reportControl.ReportModel.LstCurrentQueries, campName);
                        if (reportDetails.Count == 0)
                        {
                            Dialog.ShowDialog("LangKeyReport".FromResourceDictionary(),
                                $"{"LangKeyReportForCampaign".FromResourceDictionary()} {campName.CampaignName} {"LangKeyIsNotAvailable".FromResourceDictionary()}");
                            return;
                        }

                        var win = objDialog.GetMetroWindow(reportControl, "LangKeyReport".FromResourceDictionary());
                        win.Owner = Application.Current.MainWindow;
                        win.WindowStartupLocation = WindowStartupLocation.Manual;
                        win.Top = 0;
                        win.Left = 0;
                        reportControl.ReportModel.LstReports = reportDetails;
                        reportControl.ReportModel.PageSize = 30;
                        reportControl.ReportModel.TotalPages = reportControl.ReportModel.LstReports.Count > 0 ? (reportControl.ReportModel.LstReports.Count + reportControl.ReportModel.PageSize - 1) / reportControl.ReportModel.PageSize : 1;
                        reportControl.ReportModel.TotalReportCount = reportControl.ReportModel.LstReports.Count;
                        reportControl.ReportModel.LoadNextEnable = reportControl.ReportModel.LstReports.Count > reportControl.ReportModel.PageSize;
                        reportControl.ReportModel.CurrentPageLstReports = new ObservableCollection<object>(reportControl.ReportModel.LstReports.Skip((reportControl.ReportModel.CurrentPage - 1) * reportControl.ReportModel.PageSize).Take(reportControl.ReportModel.PageSize).ToList());
                        reportControl.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(reportControl.ReportModel.CurrentPageLstReports);

                        //Task.Factory.StartNew(() =>
                        //{
                        //    _currentPageLstReports.ForEach(item =>
                        //    {
                        //        Application.Current.Dispatcher.Invoke(async () =>
                        //            reportControl.ReportModel.CurrentPageLstReports.Add(item));
                        //        Thread.Sleep(10);
                        //    });
                        //});
                        //reportControl.LoadPageData();
                        win.ShowDialog();
                    }
                    catch (AggregateException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void ShowSelectedAccountsExecute(object sender)
        {
            try
            {
                var campaign = (CampaignDetails)sender;
                SelectedAccountListCampaign =
                    $"{"LangKeySelectedAccounts".FromResourceDictionary()}\n[{campaign.CampaignName}]";
                SelectedAccountList = campaign.SelectedAccountList;
                SelectedAccountsFlyoutVisibility = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ExportReportExecute(object sender)
        {
            try
            {
                var campaign = sender as CampaignDetails;

                if (campaign != null)
                {
                    var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

                    var activitySettings = templatesFileManager.GetTemplateById(campaign.TemplateId).ActivitySettings;

                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaign.SubModule);

                    var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campaign.SocialNetworks)
                        .GetNetworkCoreFactory();

                    var lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, activitySettings);

                    var ReportModel = new ReportModel();
                    lstSavedQuery?.ToList().ForEach(x =>
                    {
                        ReportModel.LstCurrentQueries.Add(
                            new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));
                    });

                    var reportDetails = networkCoreFactory.ReportFactory.GetReportDetail(ReportModel,
                        ReportModel.LstCurrentQueries, campaign);

                    if (reportDetails.Count == 0)
                    {
                        Dialog.ShowDialog("LangKeyReport".FromResourceDictionary(),
                            $"{"LangKeyReportForCampaign".FromResourceDictionary()} {campaign.CampaignName} {"LangKeyIsNotAvailable".FromResourceDictionary()}");
                        return;
                    }

                    var exportPath = FileUtilities.GetExportPath();

                    if (string.IsNullOrEmpty(exportPath))
                        return;

                    var filename = Regex.Replace(
                        $"{campaign.CampaignName}-Reports[{DateTimeUtilities.GetEpochTime()}]",
                        "[\\/:*?<>|\"]",
                        "-");

                    filename = $"{exportPath}\\{filename}.csv";
                    SocinatorInitialize.GetSocialLibrary(campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory
                        .ExportReports(activityType, filename, ReportType.Campaign);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StatusChangeExecute(object sender)
        {
            //if Active/Pause all campaign Toggle changed
            if (sender == null)
            {
                //Get all selected campaign
                var lstSelectedCampaign = LstCampaignDetails.Where(item =>
                    item.IsCampaignChecked && item.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork);
                //if no campaign is selected show the Warnning message and revert back the toggle
                if (lstSelectedCampaign.Count() == 0)
                {
                    AllCampStatus = !AllCampStatus;
                    Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeySelectAtleastOneCampaign".FromResourceDictionary());
                    return;
                }

                try
                {
                    //if Active/Pause all campaign Toggle changed and atleast one campaign is selected
                    Task.Factory.StartNew(() =>
                    {
                        if (!AllCampStatus)
                        {
                            var campignNotHavingAccount = lstSelectedCampaign.Where(x => x.Status == "Active");
                            if (campignNotHavingAccount.Count() == 0)
                            {
                                AllCampStatus = true;
                                return;
                            }

                            campignNotHavingAccount.ForEach(camp => { ActivePauseCampaign(camp, AllCampStatus); });
                        }

                        if (AllCampStatus)
                        {
                            var campignHavingAccount = lstSelectedCampaign.Where(x => x.SelectedAccountList.Count > 0);
                            if (campignHavingAccount.Count() == 0)
                            {
                                AllCampStatus = !AllCampStatus;
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, "",
                                    "LangKeyCampaignActivation".FromResourceDictionary(),
                                    "LangKeyCampaignHaveNoAccount".FromResourceDictionary());
                                return;
                            }

                            campignHavingAccount.ForEach(camp => { ActivePauseCampaign(camp, AllCampStatus); });
                        }
                    }).Wait();
                }
                catch (AggregateException ex)
                {
                    ex.DebugLog();
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            }
            else
            {
                //if individual campaign toggle changed
                try
                {
                    CampaignDetails selectedCampaign = null;
                    if(sender is CampaignDetails campaign)
                    {
                        selectedCampaign = campaign;
                    }else
                        selectedCampaign = ((FrameworkElement)sender).DataContext as CampaignDetails;

                    if (selectedCampaign?.SelectedAccountList.Count == 0)
                    {
                        if (selectedCampaign.Status == "Paused")
                            return;
                        GlobusLogHelper.log.Info(Log.CustomMessage, selectedCampaign.SocialNetworks,
                            selectedCampaign.CampaignName, "LangKeyStatusChangeFailed".FromResourceDictionary(),
                            $"{"LangKeyAccountIsntPresentIn".FromResourceDictionary()} {selectedCampaign.CampaignName}");
                        selectedCampaign.Status = "Paused";
                        return;
                    }

                    var isChecked = false;
                    if(sender is ToggleSwitch toggle)
                    {
                        isChecked = (bool)((ToggleSwitch)sender).IsChecked;
                    }
                    else if(sender is CampaignDetails campaign1)
                    {
                        isChecked = campaign1.Status == "Active";
                    }
                    var isToggleSwitchSelected = isChecked;
                    ChangeAllCampStatus();
                    Task.Factory.StartNew(() => { ActivePauseCampaign(selectedCampaign, isToggleSwitchSelected); })
                        .Wait();
                }
                catch (AggregateException ex)
                {
                    ex.DebugLog();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void DuplicateExecute(object sender)
        {
            try
            {
                var camp = sender as CampaignDetails;
                var campName = camp.DeepCloneObject();
                campName.CampaignName = camp?.CampaignName.Split('[')[0] +
                                        $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

                SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                    .ViewCampaigns(campName.CampaignId, ConstantVariable.CreateCampaign());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditExecute(object sender)
        {
            try
            {
                var campName = sender as CampaignDetails;

                SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                    .ViewCampaigns(campName.CampaignId, ConstantVariable.UpdateCampaign());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteExecute(object sender)
        {
            if (sender is CampaignDetails)
                try
                {
                    var campaign = sender as CampaignDetails;
                    var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                        $"[ {campaign.CampaignName} ] {"LangKeyConfirmToDeleteCampaign".FromResourceDictionary()}",
                        "LangKeyDeleteAnyway".FromResourceDictionary(), "LangKeyDontDelete".FromResourceDictionary());
                    if (dialogResult != MessageDialogResult.Affirmative)
                        return;
                    var selectedAccount = campaign.SelectedAccountList;

                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.Delete(campaign);
                    DeletedCampaignTempModel model = new DeletedCampaignTempModel();
                    model.CampignDeletedTemps = campaign;
                    campaignFileManager.SaveTemp(model);
                    DeleteDuplicateCampaign(campaign, campaignFileManager);

                    _dataBaseHandler.DeleteDatabase(new List<string> { campaign.CampaignId }, DatabaseType.CampaignType);
                    LstCampaignDetails.Remove(
                        LstCampaignDetails.FirstOrDefault(x => x.CampaignId == campaign.CampaignId));
                    Uncheck();
                    var allAccounts = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
                    UpdateAccount(allAccounts, campaign, selectedAccount);
                    GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork,
                        campaign.CampaignName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            else
                try
                {
                    var campaign = LstCampaignDetails.Where(x => x.IsCampaignChecked).ToList();
                    if (campaign?.Count == 0)
                    {
                        Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                            "LangKeyWarningSelectCampaignToDelete".FromResourceDictionary());
                        return;
                    }

                    var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                        "LangKeyConfirmToDeleteSelectedCampaign".FromResourceDictionary(),
                        "LangKeyDeleteAnyway".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                    if (dialogResult != MessageDialogResult.Affirmative)
                        return;
                    var allAccounts = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    Task.Factory.StartNew(() =>
                    {
                        Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            try
                            {
                                campaign.ForEach(camp =>
                                {
                                    var selectedAccount = camp.SelectedAccountList;
                                    campaignFileManager.Delete(camp);
                                    DeletedCampaignTempModel model = new DeletedCampaignTempModel();
                                    model.CampignDeletedTemps = camp;
                                    campaignFileManager.SaveTemp(model);
                                    DeleteDuplicateCampaign(camp, campaignFileManager);
                                    UpdateAccount(allAccounts, camp, selectedAccount);
                                    LstCampaignDetails.Remove(
                                        LstCampaignDetails.FirstOrDefault(x => x.CampaignId == camp.CampaignId));
                                });
                                _dataBaseHandler.DeleteDatabase(campaign.Select(acct => acct.CampaignId),
                                    DatabaseType.CampaignType);
                                Uncheck();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                ToasterNotification.ShowError(
                                    "LangKeyErrorOnDeletingCampaigns".FromResourceDictionary());
                            }
                        });
                        GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork,
                            $"[ {campaign.Count} ] {"LangKeyCampaigns".FromResourceDictionary()}");
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        private static void DeleteDuplicateCampaign(CampaignDetails campaign, ICampaignsFileManager campaignFileManager)
        {
            var duplicateCampaign = campaignFileManager.FirstOrDefault(x => x.CampaignId == campaign.CampaignId);
            if (duplicateCampaign?.CampaignId == campaign.CampaignId)
                campaignFileManager.Delete(campaign);
        }

        private void Uncheck()
        {
            if (LstCampaignDetails.Count(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork) == 0 &&
                IsAllCampaignChecked)
            {
                IsAllCampaignChecked = false;
                AllCampStatus = false;
            }
        }

        private void CopyCampaignIdExecute(object sender)
        {
            try
            {
                var campName = sender as CampaignDetails;

                if (!string.IsNullOrEmpty(campName.CampaignId))
                {
                    Clipboard.SetText(campName.CampaignId);
                    ToasterNotification.ShowSuccess("LangKeyCampaignIdCopied".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SettingExecute(object sender)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    var button = (Button)sender;
                    var contextMenu = button.ContextMenu;
                    if (contextMenu != null)
                    {
                        contextMenu.DataContext = button.DataContext;
                        contextMenu.PlacementTarget = button;
                        contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        contextMenu.IsOpen = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectionExecute(object sender)
        {
            try
            {
                // To check whether all destinations are selected, then make the tick mark on column header
                if (LstCampaignDetails.Where(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork)
                    .All(y => y.IsCampaignChecked))
                {
                    IsAllCampaignChecked = true;
                }
                else
                {
                    if (IsAllCampaignChecked)
                        _isUncheckedFromList = true;
                    // If not so, dont tick the column header 
                    IsAllCampaignChecked = false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterCampaign(object sender)
        {
            try
            {
                if (CampaignModel.SelectedActivity == "All" || string.IsNullOrEmpty(CampaignModel.SelectedActivity))
                    CampaignCollection.Filter = x =>
                        ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                else
                    CampaignCollection.Filter = x =>
                        ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork &&
                        ((CampaignDetails)x).SubModule == CampaignModel.SelectedActivity;

                ChangeAllCampStatus();
            }
            catch (Exception ex)
            {
                CampaignCollection.Filter =
                    x => ((CampaignDetails)x)?.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                ChangeAllCampStatus();
                ex.DebugLog();
            }
        }

        private void FilterByNameExecute(object sender)
        {
            try
            {
                if (string.IsNullOrEmpty(FilterByName))
                    CampaignCollection.Filter = x =>
                        ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                else
                    CampaignCollection.Filter = x =>
                    ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork &&
                        ((CampaignDetails)x).CampaignName.ToLower().Contains(FilterByName.ToLower());

                ChangeAllCampStatus();
            }
            catch (Exception ex)
            {
                CampaignCollection.Filter =
                    x => ((CampaignDetails)x)?.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                ChangeAllCampStatus();
                ex.DebugLog();
            }
        }

        private void ActivePauseCampaign(CampaignDetails selectedCampaign, bool isToggleSwitchSelected)
        {
            try
            {
                CancelPriviousTask();

                var currentNetworkAccounts = InstanceProvider.GetInstance<IAccountCollectionViewModel>()
                    .BySocialNetwork(selectedCampaign.SocialNetworks);
                var lstSelectedAccountDetails =
                    _accountsFileManager.GetAllAccounts(selectedCampaign.SelectedAccountList,
                        selectedCampaign.SocialNetworks);
                var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

                Task.Factory.StartNew(() =>
                {
                    lstSelectedAccountDetails.ForEach(account =>
                    {
                        try
                        {
                            if (!currentNetworkAccounts.Any(x => x.AccountId == account.AccountId))
                            {
                                #region New Added Code for checking campaign Active/Pause issue

                                var user = currentNetworkAccounts.FirstOrDefault(x =>
                                    x.AccountBaseModel.UserName == account.AccountBaseModel.UserName);
                                if (user != null)
                                {
                                    account.AccountBaseModel.AccountId = account.AccountId =
                                        user.AccountId ?? user.AccountBaseModel.AccountId;
                                    _accountsFileManager.Edit(account);
                                    GlobusLogHelper.log.Debug($"ACCOUNT ID CHANGED for user -> {user.UserName}");
                                }

                                #endregion

                                else
                                {
                                    return;
                                }
                            }

                            UpdateAccountCampaignsStatus(selectedCampaign, isToggleSwitchSelected, account, module);
                            Thread.Sleep(5);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                }, CancellationSource.Token).Wait();


                if (isToggleSwitchSelected)
                {
                    LstCampaignDetails.FirstOrDefault(x => x.CampaignId == selectedCampaign.CampaignId).Status =
                        "Active";
                    GlobusLogHelper.log.Info(Log.ActivatedCampaign, SocinatorInitialize.ActiveSocialNetwork,
                        selectedCampaign.CampaignName);
                }
                else
                {
                    LstCampaignDetails.FirstOrDefault(x => x.CampaignId == selectedCampaign.CampaignId).Status =
                        "Paused";
                    GlobusLogHelper.log.Info(Log.CampaignPaused, SocinatorInitialize.ActiveSocialNetwork,
                        selectedCampaign.CampaignName);
                }

                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                campaignFileManager.UpdateCampaigns(LstCampaignDetails.ToList());
            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CancelPriviousTask()
        {
            CancellationSource.Cancel();
            CancellationSource.Dispose();
            CancellationSource = new CancellationTokenSource();
        }

        private void UpdateAccountCampaignsStatus(CampaignDetails selectedCampaign, bool isToggleSwitchSelected,
            DominatorAccountModel account, ActivityType module)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                var moduleConfiguration = jobActivityConfigurationManager[account.AccountId, module];
                try
                {
                    #region New Added Code for checking campaign Active/Pause issue

                    if (string.IsNullOrEmpty(moduleConfiguration.TemplateId))
                    {
                        GlobusLogHelper.log.Debug("TEMPLATE ID IS NULL");
                        if (moduleConfiguration.ActivityType.ToString() == selectedCampaign.SubModule)
                            moduleConfiguration.IsEnabled = isToggleSwitchSelected;
                    }

                    #endregion

                    if (moduleConfiguration.TemplateId == selectedCampaign.TemplateId)
                        moduleConfiguration.IsEnabled = isToggleSwitchSelected;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId,
                    moduleConfiguration.ActivityType, moduleConfiguration);
                accountsCacheService.UpsertAccounts(account);

                //Update ActivityManager of account in Db
                _dbOperations.UpdateAccountActivityManager(account);

                if (isToggleSwitchSelected)
                    dominatorScheduler.ScheduleNextActivity(account, module);
                else
                    dominatorScheduler.StopActivity(account, selectedCampaign.SubModule, selectedCampaign.TemplateId,
                        false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateAccount(List<DominatorAccountModel> allAccounts, CampaignDetails camp,
            List<string> selectedAccount)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                // remove template from each account
                allAccounts.ForEach(x =>
                {
                    var moduleConfig = jobActivityConfigurationManager[x.AccountId]
                        .FirstOrDefault(mc => mc.TemplateId == camp.TemplateId);
                    if (moduleConfig != null)
                    {
                        // Stop active task related to campaign
                        dominatorScheduler.StopActivity(x, camp.SubModule, camp.TemplateId, false);

                        // Remove task from list
                        foreach (var moduleConfiguration in jobActivityConfigurationManager[x.AccountId]
                            .Where(mc => mc.TemplateId == camp.TemplateId).ToList())
                        {
                            jobActivityConfigurationManager.Delete(x.AccountId, moduleConfiguration.ActivityType);

                            //Update ActivityManager of account in Db
                            _dbOperations.UpdateAccountActivityManager(x);
                        }
                    }
                });

                accountsCacheService.UpsertAccounts(allAccounts.ToArray());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Command

        public ICommand SettingCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DuplicateCommand { get; set; }
        public ICommand StatusChangeCommand { get; set; }
        public ICommand ReportCommand { get; set; }
        public ICommand SelectedAccountsCommand { get; set; }
        public ICommand ExportReportCommand { get; set; }
        public ICommand CampaignTypeSelectionChange { get; set; }
        public ICommand SelectionCommand { get; set; }
        public ICommand CopyCampaignIdCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand FilterByNameCommand { get; set; }

        #endregion

        #region Properties

        private static readonly object _lock = new object();

        private ICollectionView _campaignCollection;

        public ICollectionView CampaignCollection
        {
            get => _campaignCollection;
            set
            {
                if (_campaignCollection != null && _campaignCollection == value)
                    return;
                _campaignCollection = value;
                OnPropertyChanged(nameof(CampaignCollection));
            }
        }

        private ObservableCollection<CampaignDetails> _campaignDetails = new ObservableCollection<CampaignDetails>();

        public ObservableCollection<CampaignDetails> LstCampaignDetails
        {
            get => _campaignDetails;
            set
            {
                if (_campaignDetails != null && _campaignDetails == value)
                    return;
                _campaignDetails = value;
                OnPropertyChanged(nameof(LstCampaignDetails));
            }
        }

        private CampaignDetails _campaignModel = new CampaignDetails();

        public CampaignDetails CampaignModel
        {
            get => _campaignModel;
            set
            {
                _campaignModel = value;
                OnPropertyChanged(nameof(CampaignModel));
            }
        }

        private List<string> _selectedAccountList = new List<string>();

        public List<string> SelectedAccountList
        {
            get => _selectedAccountList;
            set
            {
                _selectedAccountList = value;
                OnPropertyChanged(nameof(SelectedAccountList));
            }
        }

        private string _selectedAccountListCampaign;

        public string SelectedAccountListCampaign
        {
            get => _selectedAccountListCampaign;
            set
            {
                _selectedAccountListCampaign = value;
                OnPropertyChanged(nameof(SelectedAccountListCampaign));
            }
        }


        private SocialNetworks _socialNetworks;

        public SocialNetworks SocialNetworks
        {
            get => _socialNetworks;
            set
            {
                _socialNetworks = value;
                OnPropertyChanged(nameof(SocialNetworks));
            }
        }

        private bool _isAllCampaignChecked;

        public bool IsAllCampaignChecked
        {
            get => _isAllCampaignChecked;
            set
            {
                if (_isAllCampaignChecked == value)
                    return;
                _isAllCampaignChecked = value;

                OnPropertyChanged(nameof(IsAllCampaignChecked));
                SelectAllCampaign(_isAllCampaignChecked);
                _isUncheckedFromList = false;
            }
        }

        private bool _isUncheckedFromList;
        private bool _allCampStatus;

        public bool AllCampStatus
        {
            get => _allCampStatus;
            set => SetProperty(ref _allCampStatus, value);
        }

        private bool _selectedAccountsFlyoutVisibility;

        public bool SelectedAccountsFlyoutVisibility
        {
            get => _selectedAccountsFlyoutVisibility;
            set => SetProperty(ref _selectedAccountsFlyoutVisibility, value);
        }


        public void SelectAllCampaign(bool isAllSelected)
        {
            try
            {
                if (_isUncheckedFromList)
                    return;
                if (CampaignModel.SelectedActivity == "All")
                    LstCampaignDetails.Where(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork).Select(
                        x =>
                        {
                            x.IsCampaignChecked = isAllSelected;
                            return x;
                        }).ToList();
                else
                    LstCampaignDetails.Where(x =>
                        x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork &&
                        x.SubModule == CampaignModel.SelectedActivity).Select(x =>
                    {
                        x.IsCampaignChecked = isAllSelected;
                        return x;
                    }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }

    public class BindingData : Freezable
    {
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingData), new UIPropertyMetadata(null));

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingData();
        }

        #endregion
    }
}