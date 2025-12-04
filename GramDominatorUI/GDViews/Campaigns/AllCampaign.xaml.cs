using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using FluentScheduler;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Report;
using GramDominatorUI.GDViews.GrowFollowers;
using GramDominatorUI.GDViews.InstaLikeComment;
using GramDominatorUI.GDViews.InstaPoster;
using GramDominatorUI.GDViews.InstaScrape;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Reports = DominatorUIUtility.CustomControl.Reports;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using DominatorHouseCore.BusinessLogic;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.BusinessLogic.Scheduler;

namespace GramDominatorUI.GDViews.Campaigns
{
    /// <summary>
    /// Interaction logic for AllCampaign.xaml
    /// </summary>
    public partial class AllCampaign : UserControl
    {

        private CampaignDetails objCampaignDetails;

        private AllCampaign()
        {
            InitializeComponent();

            objCampaignDetails = new CampaignDetails();
            CmbCampaignType.Items.Add("All");
            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                CmbCampaignType.Items.Add(name);
            }
            
            DialogParticipation.SetRegister(this, this);
        }
        private static AllCampaign ObjAllCampaign = null;
        public static AllCampaign GetSingeltonObjectAllCampaign()
        {
            if (ObjAllCampaign == null)
                ObjAllCampaign = new AllCampaign();
            return ObjAllCampaign;
        }

        // Callback on Creating new Campaign or Updating existing
        // Raises when user Clicks 'Create Campaign' or Update already existing campaing or Delete campaign
        private void CampaignStatusChanged(object sender, EventArgs e)
        {
            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(CampaignsFileManager.Get());
            var lstAccountDetails = AccountsFileManager.GetAll();
            
            var campaignDetails = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var module = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule) ;
           
            foreach(var account in lstAccountDetails)
            {
                var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == module);
                if (moduleConfiguration != null && moduleConfiguration.TemplateId == campaignDetails.TemplateId)
                {
                    moduleConfiguration.IsEnabled = (bool)(sender as ToggleSwitch).IsChecked;
                    AccountsFileManager.Edit(account);
                }
            }
            

            try
            {
                Schedule schedule = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{module}-{campaignDetails.TemplateId}");
                if ((sender as ToggleSwitch)?.IsChecked ?? false)
                {
                    if (schedule != null && schedule.Disabled)
                    {
                        var accounts = AccountsFileManager.GetAll();
                        foreach (var account in accounts)
                        {
                            if (campaignDetails.SelectedAccountList.Contains(account.AccountBaseModel.UserName))
                                DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.Instagram, module);
                        }
                        schedule.Enable();
                    }
                    else
                    {
                        schedule?.Disable();     // <-- crash, schedule=null

                    }

                  
                }
                else
                {
                    campaignDetails.SelectedAccountList.ForEach(x =>
                    {
                        DominatorScheduler.StopActivity(x, module.ToString(), campaignDetails.TemplateId);
                    });
                    GlobusLogHelper.log.Info(module + "-" + campaignDetails.TemplateId + "  Job is disabled");
               
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("No job scheduled");
                ex.DebugLog();
            }
      

            try
            {
                foreach (var campaign in objCampaignDetails.ObjCampaignDetails)
                {
                    if (campaign.CampaignName == campaignDetails.CampaignName)
                    {

                        if ((sender as ToggleSwitch).IsChecked ?? false)
                        {
                            campaign.Status = "Active";
                            foreach (var accountModel in lstAccountDetails.Where(x => campaignDetails.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                            {
                                DominatorScheduler.ScheduleTodayJobs(accountModel, SocialNetworks.Instagram, module);
                            }
                        }
                        else
                        {
                            campaign.Status = "Paused";
                            campaign.SelectedAccountList.ForEach(x =>
                            {
                                TaskAndThreadUtility.StopTask(x, campaignDetails.TemplateId);
                            });
                         
                        }
                    }                    
                }

                CampaignsFileManager.Save(objCampaignDetails.ObjCampaignDetails.ToList());                
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void AllCampaign_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetDataContext();
        }

        private void SetDataContext()
        {
            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(CampaignsFileManager.Get());
            DgrAllCampaign.ItemsSource = objCampaignDetails.ObjCampaignDetails;
            MainGrid.DataContext = objCampaignDetails;
        }


        private void BtnCampaignSetting_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button)sender).ContextMenu.DataContext = ((Button)sender).DataContext;
                ((Button)sender).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }
        }


        private void DeleteSingleCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirmation", "If you delete it will delete [ " + campName.CampaignName + " ] Campaign permanently from campaign\nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());
            if (dialogResult != MessageDialogResult.Affirmative)
                return;


            var campaigns = CampaignsFileManager.Get();
            var toDelete = campaigns.FirstOrDefault(c => c.CampaignName == campName.CampaignName);
            if (toDelete != null)
            {
                campaigns.Remove(toDelete);
                objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(campaigns);
                SetDataContext();

                CampaignsFileManager.Save(campaigns);
            }            
        }

        private void CmbCampaignType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<CampaignDetails> moduleWiseDetail = null;
            if (!CmbCampaignType.SelectedItem.Equals("All"))
            {
                moduleWiseDetail = CampaignsFileManager.Get()
                       .Where(x => x.SubModule == CmbCampaignType.SelectedItem.ToString()).ToList();

            }
            else
            {
                moduleWiseDetail = CampaignsFileManager.Get();
            }
            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(moduleWiseDetail);
            DgrAllCampaign.ItemsSource = objCampaignDetails.ObjCampaignDetails;

        }

        private void EditCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            var campaignDetails =  CampaignsFileManager.GetCampaignById(campName.CampaignId);

            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            ManageCampaign(templateDetails, campaignDetails, false, Visibility.Visible, ConstantVariable.UpdateCampaign, campaignDetails.TemplateId);
        }
        private void DuplicateCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            var campaignDetails = CampaignsFileManager.GetCampaignById(campName.CampaignId);

            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            ManageCampaign(templateDetails, campaignDetails, true, Visibility.Collapsed, ConstantVariable.CreateCampaign, campaignDetails.TemplateId);
        }

        private static void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails, bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

                switch (ActivityType)
                {
                    case ActivityType.Follow:

                        Follower objFollower = Follower.GetSingeltonObjectFollower();
                        objFollower.IsEditCampaignName = IsEditCampaignName;
                        objFollower.CancelEditVisibility = CancelEditVisibility;
                        objFollower.TemplateId = TemplateID;
                        objFollower.CampaignName = campaignDetails.CampaignName;
                        objFollower.CampaignButtonContent = CampaignButtonContent;
                        objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objFollower.ObjViewModel.FollowerModel =
                            JsonConvert.DeserializeObject<FollowerModel>(templateDetails.ActivitySettings);

                        objFollower.MainGrid.DataContext = objFollower.ObjViewModel.FollowerModel;

                       DominatorHouseCore.Utility.TabSwitcher.ChangeTabIndex(1, 0);
                        break;

                    case ActivityType.Unfollow:

                        UnFollower objUnFollower = UnFollower.GetSingeltonObjectUnfollower();
                        objUnFollower.IsEditCampaignName = IsEditCampaignName;
                        objUnFollower.CancelEditVisibility = CancelEditVisibility;
                        objUnFollower.CampaignButtonContent = CampaignButtonContent;
                        objUnFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objUnFollower.TemplateId = TemplateID;
                        objUnFollower.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objUnFollower.CampaignName = campaignDetails.CampaignName;
                        objUnFollower.ObjViewModel.UnfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templateDetails.ActivitySettings);
                        objUnFollower.MainGrid.DataContext = objUnFollower.ObjViewModel.UnfollowerModel;

                      //  TabSwitcher.ChangeTabIndex(1, 1);
                        break;
                    case ActivityType.Post:
                       // TabSwitcher.ChangeTabIndex(2, 0);
                        break;
                    case ActivityType.Repost:
                        RePoster objRePoster = RePoster.GetSingeltonObjectRePoster();
                        objRePoster.IsEditCampaignName = IsEditCampaignName;
                        objRePoster.CancelEditVisibility = CancelEditVisibility;
                        objRePoster.CampaignButtonContent = CampaignButtonContent;
                        objRePoster.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objRePoster.TemplateId = TemplateID;
                        objRePoster.RePosterFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objRePoster.CampaignName = campaignDetails.CampaignName;
                        objRePoster.ObjRePosterViewModel.RePosterModel = JsonConvert.DeserializeObject<RePosterModel>(templateDetails.ActivitySettings);
                        objRePoster.MainGrid.DataContext = objRePoster.ObjRePosterViewModel.RePosterModel;

                       // TabSwitcher.ChangeTabIndex(2, 1);
                        break;
                    case ActivityType.Like:
                        Like objLike = Like.GetSingeltonObjectLike();
                        objLike.IsEditCampaignName = IsEditCampaignName;
                        objLike.CancelEditVisibility = CancelEditVisibility;
                        objLike.CampaignButtonContent = CampaignButtonContent;
                        objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objLike.TemplateId = TemplateID;
                        objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objLike.CampaignName = campaignDetails.CampaignName;
                        objLike.objLikeViewModel.LikeModel = JsonConvert.DeserializeObject<LikeModel>(templateDetails.ActivitySettings);
                        objLike.MainGrid.DataContext = objLike.objLikeViewModel.LikeModel;
                      //  TabSwitcher.ChangeTabIndex(4, 0);
                        break;

                    case ActivityType.Comment:
                        Comment objComment = Comment.GetSingeltonObjectComment();
                        objComment.IsEditCampaignName = IsEditCampaignName;
                        objComment.CancelEditVisibility = CancelEditVisibility;
                        objComment.CampaignButtonContent = CampaignButtonContent;
                        objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objComment.TemplateId = TemplateID;
                        objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objComment.CampaignName = campaignDetails.CampaignName;
                        objComment.ObjCommentViewModel.CommentModel = JsonConvert.DeserializeObject<CommentModel>(templateDetails.ActivitySettings);
                        objComment.MainGrid.DataContext = objComment.ObjCommentViewModel.CommentModel;
                      //  TabSwitcher.ChangeTabIndex(4, 1);
                        break;
                    case ActivityType.Unlike:

                     //   TabSwitcher.ChangeTabIndex(4, 2);
                        break;

                    case ActivityType.UserScraper:
                        UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                        objUserScraper.IsEditCampaignName = IsEditCampaignName;
                        objUserScraper.CancelEditVisibility = CancelEditVisibility;
                        objUserScraper.CampaignButtonContent = CampaignButtonContent;
                        objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objUserScraper.TemplateId = TemplateID;
                        objUserScraper.UserScraperFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objUserScraper.CampaignName = campaignDetails.CampaignName;
                        objUserScraper.ObjUserScraperViewModel.UserScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);
                        objUserScraper.MainGrid.DataContext = objUserScraper.ObjUserScraperViewModel.UserScraperModel;

                      //  TabSwitcher.ChangeTabIndex(5, 0);
                        break;
                    case ActivityType.DownloadScraper:
                        DownloadPhotos objDownloadPhotos = DownloadPhotos.GetSingeltonObjectDownloadPhotos();
                        objDownloadPhotos.IsEditCampaignName = IsEditCampaignName;
                        objDownloadPhotos.CancelEditVisibility = CancelEditVisibility;
                        objDownloadPhotos.CampaignButtonContent = CampaignButtonContent;
                        objDownloadPhotos.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objDownloadPhotos.TemplateId = TemplateID;
                        objDownloadPhotos.DownloadPhotosFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objDownloadPhotos.CampaignName = campaignDetails.CampaignName;
                        objDownloadPhotos.ObjDownloadPhotosViewModel.DownloadPhotosModel = JsonConvert.DeserializeObject<DownloadPhotosModel>(templateDetails.ActivitySettings);
                        objDownloadPhotos.MainGrid.DataContext = objDownloadPhotos.ObjDownloadPhotosViewModel.DownloadPhotosModel;

                       DominatorHouseCore.Utility.TabSwitcher.ChangeTabIndex(5, 1);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }
        }

        private void CampaignReports_OnClick(object sender, RoutedEventArgs e)
        {

            Reports ObjReports = new Reports();

            ObjReports.ReportModel = new DominatorHouseCore.Models.ReportModel();

            Dialog objDialog = new Dialog();

            DataBaseConnectionCodeFirst.DataBaseConnection ObjDataBaseConnection = null;
          
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var ActivitySettings= TemplatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

            ObservableCollectionBase<QueryInfo> lstSavedQuery =JsonConvert.DeserializeObject<FollowerModel>(ActivitySettings).SavedQueries;
            Dictionary<string, string> lstCurrentQueries = new Dictionary<string, string>();
                lstSavedQuery.ToList().ForEach(x =>
                   {
                       lstCurrentQueries.Add(x.QueryValue, x.QueryType.ToString());
                   }); 
           
            ObservableCollection<FollowReportDetails> FollowerReportModel = new ObservableCollection<FollowReportDetails>();
            try
            {

                DataBaseConnectionCodeFirst.DataBaseConnection dataBase =
                    DataBaseHandler.GetDataBaseConnectionInstance(campName.CampaignId, DatabaseType.CampaignType);
                    dataBase.Get<InteractedUsers>().ForEach(
                        report =>
                        {

                            if (lstCurrentQueries.ContainsKey(report.Query) && lstCurrentQueries.ContainsValue(report.QueryType))
                            {
                                FollowerReportModel.Add(new FollowReportDetails()
                                {
                                    AccountName = report.Username,
                                    Date = DateTimeUtilities.EpochToDateTimeUtc(report.Date),
                                    QueryType = report.QueryType,
                                    Query = report.Query,
                                    Username = report.InteractedUsername
                                    
                                });
                                if (ObjReports.ReportModel.QueryList.Count>1)
                                {
                                    if (ObjReports.ReportModel.QueryList.Any(query => query.Content == report.Query) == false)
                                        ObjReports.ReportModel.QueryList.Add(new ContentSelectGroup() { IsContentSelected = false, Content = report.Query });

                                }
                            }
                        });

                ObjReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(FollowerReportModel);

                campName.SelectedAccountList.ToList().ForEach(acc =>
                {
                    DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc);
                    
                    ObjReports.ReportModel.AccountList.Add(new ContentSelectGroup()
                    {
                        IsContentSelected = false,
                        Content = objDominatorAccountModel.AccountBaseModel.UserName
                    });

                    if (ObjReports.ReportModel.StatusList.Count > 1 &&
                        ObjReports.ReportModel.StatusList.Any(status => status.Content == objDominatorAccountModel.AccountBaseModel.Status) ==
                        false)
                        ObjReports.ReportModel.StatusList.Add(new ContentSelectGroup()
                        {
                            IsContentSelected = false,
                            Content = objDominatorAccountModel.AccountBaseModel.Status
                        });

                });

            }
            catch (Exception ex)
            {

            }

            campName.SelectedAccountList.ToList().ForEach(x =>
                {
                    ObjReports.ReportModel.GridViewColumn = new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor { ColumnHeaderText = "Account", ColumnBindingText = "AccountName" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType" },
                        new GridViewColumnDescriptor { ColumnHeaderText = "Query", ColumnBindingText ="Query"},
                        new GridViewColumnDescriptor { ColumnHeaderText = "Follower", ColumnBindingText ="Username"},
                        new GridViewColumnDescriptor { ColumnHeaderText = "Date", ColumnBindingText ="Date"},
                    };
                });
            Window win = objDialog.GetMetroWindow(ObjReports, "Reports");
            ObjReports.ExportReport.Click += (senders, events) =>
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                const string header = "AccountName,QueryType,Query,Follower,Date";

                var filename = $"{exportPath}\\ReportExport {ConstantVariable.DateasFileName}.csv";

                if (!File.Exists(filename))
                {
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(header);
                    }
                }

                FollowerReportModel.ToList().ForEach(report =>
                {
                    try
                    {
                        var csvData = report.AccountName + "," + report.QueryType + "," + report.Query + "," + report.Username + "," + report.Date ;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
                win.Close();
            };
          
            win.ShowDialog();
        }
    }
}
