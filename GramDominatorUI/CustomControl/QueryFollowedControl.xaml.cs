using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Report;

namespace GramDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for QueryFollowedControl.xaml
    /// </summary>
    public partial class QueryFollowedControl : UserControl, IQueryFollowedControl, INotifyPropertyChanged
    {
        public GetReportFollowCountModel _GetReportFollowCountModel = new GetReportFollowCountModel();

        private Dictionary<string, string> lstData = new Dictionary<string, string>();
        private List<FollowReportDetails> lstReports = new List<FollowReportDetails>();

        private readonly GetCampaignReportData objGetCampaignReportData = new GetCampaignReportData();

        private int RowNo = 1;

        public QueryFollowedControl()
        {
            InitializeComponent();
            MainGrid.DataContext = GetReportFollowCountModel;
        }

        private string Account { get; set; }
        public CampaignDetails campaign { get; set; }

        public GetReportFollowCountModel GetReportFollowCountModel
        {
            get => _GetReportFollowCountModel;
            set
            {
                _GetReportFollowCountModel = value;
                OnPropertyChanged(nameof(GetReportFollowCountModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AssignReportDetailsToModel(ObservableCollection<object> FollowRepoertList, CampaignDetails Campaign)
        {
            lstReports = new List<FollowReportDetails>();
            foreach (var follwDetails in FollowRepoertList)
            {
                var details = (FollowReportDetails) follwDetails;
                lstReports.Add(details);
            }

            GetReportFollowCountModel.LstAccounts = GetAccount();
            GetReportFollowCountModel.LstQuery = GetQuery();
            GetReportFollowCountModel.lstReportModel = new List<GetReportModel>();
            RowNo = 1;
            campaign = Campaign;
        }

        private ObservableCollection<ContentSelectGroup> GetQuery()
        {
            GetReportFollowCountModel.LstQuery = new ObservableCollection<ContentSelectGroup>();
            if (GetReportFollowCountModel.LstQuery.Count == 0)
            {
                GetReportFollowCountModel.LstQuery.Add(new ContentSelectGroup
                {
                    Content = "All",
                    IsContentSelected = false
                });
                lstReports.Select(x => x.Query).Distinct().ToList().ForEach(account =>
                {
                    GetReportFollowCountModel.LstQuery.Add(new ContentSelectGroup
                    {
                        Content = account
                    });
                });
            }

            return GetReportFollowCountModel.LstQuery;
        }

        private List<string> GetAccount()
        {
            var lstAccount = new List<string>();
            lstAccount = lstReports.Select(x => x.AccountUsername).Distinct().ToList();
            return lstAccount;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account = string.Empty;
            var cmb = sender as ComboBox;
            var account = cmb.SelectedValue;
            Account = account.ToString();
            GetReportFollowCountModel.LstQuery.ToList().Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();
            GetReportFollowCountModel.lstReportModel = new List<GetReportModel>();
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.Content.ToString() == "All")
                GetReportFollowCountModel.LstQuery.ToList().Select(query =>
                {
                    query.IsContentSelected = true;
                    return query;
                }).ToList();
            else
                SelectDeselectQueries(true);
            if ((sender as CheckBox)?.Content.ToString() != "All")
            {
                var query = (sender as CheckBox)?.Content;
                var FollowedCount = string.Empty;
                var FollwowedBackCount = string.Empty;
                var queryType = lstReports.Where(x => x.Query == query.ToString() && x.AccountUsername == Account)
                    .ToList();
                var count = GetReportFollowCountModel.lstReportModel
                    .Where(x => x.Query == query.ToString() && x.UserName == Account.ToString()).Count();
                if (count == 1)
                    return;
                if (queryType.Count == 0)
                    return;
                Task.Factory.StartNew(() =>
                {
                    objGetCampaignReportData.GetCamaignFollowerCount(Account, query.ToString(), campaign, lstReports,
                        out FollowedCount, out FollwowedBackCount);
                    bindingData(Account, query.ToString(), FollowedCount, FollwowedBackCount, queryType[0].QueryType);
                }).Wait();
            }
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.Content.ToString() == "All")
                GetReportFollowCountModel.LstQuery.ToList().Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();
            else
                SelectDeselectQueries(false);
        }

        public void SelectDeselectQueries(bool isChecked)
        {
            try
            {
                var checkedGroup =
                    GetReportFollowCountModel.LstQuery.Where(group => group.IsContentSelected == isChecked);
                GetReportFollowCountModel.LstSelectQuery.ForEach(account =>
                {
                    checkedGroup.ForEach(group =>
                    {
                        if (account.GroupName == group.Content)
                            account.IsAccountSelected = isChecked;
                    });
                });
                var uncheckedQuery = checkedGroup.Where(x => x.IsContentSelected == false).ToList();
                foreach (var query in uncheckedQuery)
                {
                    if (query.Content == "All")
                        continue;
                    GetReportFollowCountModel.lstReportModel.RemoveAll(x => query.Content.ToString() == x.Query);
                    GetReportFollowCountModel.lstReportModel =
                        new List<GetReportModel>(GetReportFollowCountModel.lstReportModel);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void bindingData(string account, string query, string FollowedCount, string FollowedBack,
            string queryType)
        {
            var objGetReportModel = new GetReportModel();
            objGetReportModel.Query = query;
            objGetReportModel.NoOfFollowed = FollowedCount;
            objGetReportModel.UserName = account;
            objGetReportModel.RowNo = RowNo++;
            objGetReportModel.QueryType = queryType;
            objGetReportModel.NoOfFollowedBack = FollowedBack; //item.Value[1].ToString();
            GetReportFollowCountModel.lstReportModel.Add(objGetReportModel);

            GetReportFollowCountModel.lstReportModel =
                new List<GetReportModel>(GetReportFollowCountModel.lstReportModel);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}