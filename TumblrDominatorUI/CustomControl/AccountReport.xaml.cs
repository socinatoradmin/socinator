using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TumblrDominatorCore.ViewModels.CustomControls;

namespace TumblrDominatorUI.CustomControl
{
    public partial class AccountReport : INotifyPropertyChanged
    {
        private AccountReportViewModel _accountReportViewModel = new AccountReportViewModel();

        public AccountReport()
        {
            InitializeComponent();
            AccountReports.DataContext = AccountReportViewModel;
        }

        public AccountReport(ActivityType activityType) : this()
        {
            AccountReportViewModel.ActivityType = activityType;
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            try
            {
                var accounts = new ObservableCollectionBase<string>(accountsFileManager.GetAll()
                    .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Tumblr &&
                                x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
                CmbAccounts.SelectedIndex = -1;
                CmbAccounts.ItemsSource = accounts;
                CmbAccounts.SelectedItem =
                    string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr)
                        .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount)
                        ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                        : SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr).GetNetworkCoreFactory()
                            .AccountUserControlTools.RecentlySelectedAccount;
                SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount =
                    string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr)
                        .GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount)
                        ? CmbAccounts.SelectedItem.ToString()
                        : SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr).GetNetworkCoreFactory()
                            .AccountUserControlTools.RecentlySelectedAccount;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public AccountReportViewModel AccountReportViewModel
        {
            get => _accountReportViewModel;

            set
            {
                _accountReportViewModel = value;
                OnPropertyChanged(nameof(AccountReportViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }
    }
}