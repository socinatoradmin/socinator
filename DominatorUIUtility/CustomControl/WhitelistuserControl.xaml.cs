using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for WhitelistuserControl.xaml
    /// </summary>
    public partial class WhitelistuserControl
    {
        private WhiteListViewModel _whiteListViewModel = new WhiteListViewModel();

        public WhitelistuserControl()
        {
            InitializeComponent();

            MainGrid.DataContext = WhiteListViewModel;
            WhiteListViewModel.InitializeData();
        }

        public WhiteListViewModel WhiteListViewModel
        {
            get => _whiteListViewModel;
            set
            {
                if (_whiteListViewModel == value)
                    return;
                _whiteListViewModel = value;
                OnPropertyChanged(nameof(WhiteListViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //private void BtnAddtoWhitelist_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(Txtusername.Text.Trim()))
        //    {
        //        GlobusLogHelper.log.Info("Error:- Please enter an username to add to the Whitelist.");
        //    }
        //    else
        //    {
        //        DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
        //        dbContext = DataBaseConnectionGlb.GetDbContext(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
        //        var blackListdbOperations = new DbOperations(dbContext);
        //        var blacklistUser = blackListdbOperations.Get<BlackListUser>();
        //        Txtusername.Text.Split('\n').ForEach(user =>
        //        {
        //            var userName = user.Trim();
        //            if (!string.IsNullOrEmpty(userName))
        //            {
        //                if (!WhitelistUserModel.LstWhiteListUsers.Any(x =>string.Compare(x.WhitelistUser,userName,StringComparison.InvariantCultureIgnoreCase)==0)
        //                    && !blacklistUser.Any(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0 ))
        //                {
        //                    WhitelistUserModel.LstWhiteListUsers.Add(
        //                        new WhitelistUserModel()
        //                        {
        //                            WhitelistUser = userName
        //                        });
        //                    dbOperations.Add<WhiteListUser>(new WhiteListUser()
        //                    {
        //                        UserName = userName,
        //                        AddedDateTime = DateTime.Now
        //                    });
        //                }
        //                else
        //                    GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork,  userName, UserType.WhiteListedUser, $"{userName} already added to Whitelist/Blacklist");

        //            }
        //        });
        //        Txtusername.Clear();
        //    }
        //}

        //private void SelectAll_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    CheckUncheckAll(true);
        //}

        //private void SelectAll_OnUnchecked(object sender, RoutedEventArgs e)
        //{
        //    CheckUncheckAll(false);
        //}

        //private void ChkWhitlistuser_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (!WhitelistUserModel.IsAllWhiteListUserChecked)
        //    {
        //        if (WhitelistUserModel.LstWhiteListUsers.All(x => x.IsWhiteListUserChecked))
        //        {
        //            SelectAll.Checked -= SelectAll_OnChecked;
        //            WhitelistUserModel.IsAllWhiteListUserChecked = true;
        //            SelectAll.Checked += SelectAll_OnChecked;

        //        }
        //    }
        //}

        //private void ChkWhitlistuser_OnUnchecked(object sender, RoutedEventArgs e)
        //{
        //    if (WhitelistUserModel.IsAllWhiteListUserChecked)
        //    {
        //        if (WhitelistUserModel.LstWhiteListUsers.Any(x => !x.IsWhiteListUserChecked))
        //        {
        //            IsUnCheckedFromUser = true;
        //            if (!WhitelistUserModel.IsAllWhiteListUserChecked)
        //                return;
        //            SelectAll.Unchecked -= SelectAll_OnUnchecked;
        //            WhitelistUserModel.IsAllWhiteListUserChecked = false;
        //            SelectAll.Unchecked += SelectAll_OnUnchecked;
        //            IsUnCheckedFromUser = false;
        //        }
        //    }
        //}

        //private void DeletedSelected_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var selectedUser = WhitelistUserModel.LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
        //    if (selectedUser.Count == 0)
        //    {
        //        Dialog.ShowDialog(Application.Current.MainWindow, "Alert",
        //            "Please select atleast on user");
        //        return;
        //    }
        //    selectedUser.ForEach(x =>
        //    {
        //        WhitelistUserModel.LstWhiteListUsers.Remove(x);
        //        dbOperations.Remove<WhiteListUser>(user => user.UserName == x.WhitelistUser);
        //    });
        //}
        //private void CheckUncheckAll(bool isChecked)
        //{
        //    WhitelistUserModel.LstWhiteListUsers.Select(x =>
        //    {
        //        x.IsWhiteListUserChecked = isChecked;
        //        return x;
        //    }).ToList();
        //}

        //private void Refresh_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    WhitelistUserModel.LstWhiteListUsers.Clear();
        //    ThreadFactory.Instance.Start(() =>
        //    {
        //        dbOperations.Get<WhiteListUser>()?.ForEach(user =>
        //        {
        //            Application.Current.Dispatcher.Invoke(() => WhitelistUserModel.LstWhiteListUsers.Add(new WhitelistUserModel
        //            {
        //                WhitelistUser = user.UserName
        //            }));
        //        });
        //    });
        //}
    }
}