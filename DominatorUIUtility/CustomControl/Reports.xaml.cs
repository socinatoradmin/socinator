using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : INotifyPropertyChanged
    {
        private ReportModel _reportModel = new ReportModel();

        public Reports()
        {
            InitializeComponent();
        }

        public Reports(CampaignDetails campaign) : this()
        {
            Campaign = campaign;
            ReportModel.CampaignId = campaign.CampaignId;
            ReportModel.ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaign.SubModule);
            MainGrid.DataContext = this;
        }
        public CampaignDetails Campaign { get; set; }

        public ReportModel ReportModel
        {
            get => _reportModel;
            set
            {
                if (_reportModel == value)
                    return;
                _reportModel = value;
                OnPropertyChanged(nameof(ReportModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    $"{Campaign.CampaignName}-Reports[{DateTimeUtilities.GetEpochTime()}]",
                    "[\\/:*?<>|\"]",
                    "-");

                filename = $"{exportPath}\\{filename}.csv";
                var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), Campaign.SubModule);

                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory
                    .ExportReports(activityType, filename, ReportType.Campaign);
            }
            catch (Exception ex)
            {
                Dialog.ShowDialog("Fail", "Export failed !!");
                ex.DebugLog();
            }
        }

        private void ClkFollowRate(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory =
                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory();
            var reportDetails =
                networkCoreFactory.ReportFactory.GetReportDetail(ReportModel, ReportModel.LstCurrentQueries, Campaign);
            var reportControl = InstanceProvider.GetInstance<IQueryFollowedControl>();
            reportControl.AssignReportDetailsToModel(reportDetails, Campaign);
            var objDialog = new Dialog();
            var win = objDialog.GetMetroWindow(reportControl, "Get Follow Rate");
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        private void RefreshReport(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory =
                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory();
            var reportDetails =
                networkCoreFactory.ReportFactory.GetReportDetail(ReportModel, ReportModel.LstCurrentQueries, Campaign);
            if (reportDetails.Count == ReportModel.LstReports.Count) return;
            ReportModel.LstReports = reportDetails;
            ReportModel.TotalPages = ReportModel.LstReports.Count > 0 ? (ReportModel.LstReports.Count + ReportModel.PageSize - 1) / ReportModel.PageSize : 1;
            ReportModel.TotalReportCount = ReportModel.LstReports.Count;
            ReportModel.LoadNextEnable = ReportModel.CurrentPage < ReportModel.TotalPages;
            ReportModel.CurrentPageLstReports = new ObservableCollection<object>(ReportModel.LstReports.Skip((ReportModel.CurrentPage - 1) * ReportModel.PageSize).Take(ReportModel.PageSize).ToList());
            ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReportModel.CurrentPageLstReports);
            // Clear existing reports
            //ReportModel.LstReports = new ObservableCollection<object>();
            // ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReportModel.LstReports);

            // Temporarily hold items before bulk-adding to the UI thread
            //var tempList = new List<object>(reportDetails);

            //Task.Factory.StartNew(() =>
            //{
            //    // Populate the ObservableCollection on the UI thread
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        foreach (var item in tempList)
            //        {
            //            ReportModel.LstReports.Add(item);
            //        }
            //        CurrentPage = 1; // Also triggers LoadPage
            //        LoadPage();
            //    });
            //});
        }


        #region Pagination
        private void LoadPage()
        {
            if (ReportModel.LstReports == null || ReportModel.LstReports.Count == 0)
                return;
            ReportModel.CurrentPageLstReports.Clear();
            var pageData = ReportModel.LstReports
                .Skip((ReportModel.CurrentPage - 1) * ReportModel.PageSize)
                .Take(ReportModel.PageSize)
                .ToList();
            ReportModel.CurrentPageLstReports = new ObservableCollection<object>(pageData);
            ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReportModel.CurrentPageLstReports);
        }


        private void LoadNext(object sender, RoutedEventArgs e)
        {
            var CanLoadMore = ReportModel.CurrentPage < ReportModel.TotalPages;
            if (CanLoadMore)
            {
                ReportModel.CurrentPage += 1;
                LoadPage();
                ReportModel.LoadPreviousEnable = true;
                ReportModel.LoadNextEnable = ReportModel.CurrentPage < ReportModel.TotalPages;
            }
        }

        private void LoadPrevious(object sender, RoutedEventArgs e)
        {
            var CanGoBack = ReportModel.CurrentPage > 1;
            if (CanGoBack)
            {
                ReportModel.CurrentPage -= 1;
                LoadPage();
                ReportModel.LoadNextEnable = true;
                ReportModel.LoadPreviousEnable = ReportModel.CurrentPage > 1;
            }
        }
        #endregion
    }
}