using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinPoster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PinDominator.PDViews.Tools.Repost
{
    /// <summary>
    /// Interaction logic for RepostConfiguration.xaml
    /// </summary>
    public partial class RepostConfiguration : UserControl, INotifyPropertyChanged
    {

        public ObservableCollectionBase<string> lstAccounts = null;
        private IEnumerable<AccountModel> AccountDetails;
        public RepostConfiguration()
        {
            InitializeComponent();
            AccountDetails = AccountsFileManager.GetFor<AccountModel>();

            lstAccounts = new ObservableCollectionBase<string>(
                AccountDetails.Select(a => a.UserName).ToList()
            );

            DialogParticipation.SetRegister(this, this);
        }
        private static RepostConfiguration CurrentRepostConfiguration { get; set; } = null;

        /// <summary>
        /// USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF FollowConfiguration
        /// </summary>
        /// <returns></returns>
        public static RepostConfiguration GetSingeltonObjectRepostConfiguration()
        {
            return CurrentRepostConfiguration ?? (CurrentRepostConfiguration = new RepostConfiguration());
        }


        /// <summary>
        /// Implement the INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private RePosterViewModel _objRePosterViewModel = new RePosterViewModel();

        public RePosterViewModel ObjRePosterViewModel
        {
            get
            {
                return _objRePosterViewModel;
            }
            set
            {
                _objRePosterViewModel = value;
                OnPropertyChanged(nameof(ObjRePosterViewModel));
            }
        }
        private void AccountgrothHeader_OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {

            //Getting details of account
            var accounts = AccountsFileManager.GetFor<AccountModel>();                
            
            //serializing account detail to AccountDetails bin file
            foreach (var account in accounts)
            {
                if (account.UserName == accountgrothHeader.SelectedItem)
                {
                    if (account.IsCretedFromNormalMode)
                    {
                        account.IsCretedFromNormalMode = false;

                        //Getting details of account having the user name  as selected account
                        var SelectedAccountDetails = accounts.FirstOrDefault(x => x.UserName == accountgrothHeader.SelectedItem);

                        #region Deleting select account from existing campaign

                        CampaignsFileManager.DeleteSelectedAccount(SelectedAccountDetails.ActivityManager.RepostModule.TemplateId,
                            accountgrothHeader.SelectedItem);
                        
                        #endregion
                        //create new template
                        CreateTemplate(account);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(account.ActivityManager.RepostModule.TemplateId))
                        {
                            //create new template
                            CreateTemplate(account);

                            //var objTemplateModel = new TemplateModel();
                            //account.ActivityManager.RepostModule.TemplateId = objTemplateModel.SaveTemplate(ObjRePosterViewModel.RePosterModel,
                            //    GdActivityType.Repost.ToString(), SocialNetworks.Instagram,
                            //    accountgrothHeader.SelectedItem + "_" + GdActivityType.Repost.ToString() + "_Template");
                        }
                        else
                        {
                            #region Updating existing template

                            //Getting details of template
                            TemplatesFileManager.UpdateActivitySettings(account.ActivityManager.RepostModule.TemplateId,
                                JsonConvert.SerializeObject(ObjRePosterViewModel.RePosterModel));

                            #endregion
                        }

                    }

                    // account.ActivityManager.FollowModule.IsEnabled = false;
                }                
            }


            AccountsFileManager.SaveAccount(accounts);

            DialogCoordinator.Instance.ShowModalMessageExternal(this, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);
        }

        private void CreateTemplate(AccountModel account)
        {            
            account.ActivityManager.RepostModule.TemplateId = new TemplateModel().SaveTemplate(ObjRePosterViewModel.RePosterModel,
                PdActivityType.Repost.ToString(), SocialNetworks.Pinterest,
                accountgrothHeader.SelectedItem + "_" + PdActivityType.Follow.ToString() + "_Template");
        }

        private void accountgrothHeader_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetDataContext();
        }

        private void SetDataContext()
        {

            try
            {
                var accountDetails = AccountsFileManager.GetAccount<AccountModel>(accountgrothHeader.SelectedItem);
                TglStatus.IsChecked = accountDetails.ActivityManager.RepostModule.IsEnabled;
                var templateDetails = TemplatesFileManager.GetTemplateById(accountDetails.ActivityManager.RepostModule.TemplateId);

                ObjRePosterViewModel.RePosterModel =
                    JsonConvert.DeserializeObject<RePosterModel>(templateDetails.ActivitySettings);



            }
            catch (Exception ex)
            {
                ObjRePosterViewModel = new RePosterViewModel();
            }
            MainGrid.DataContext = ObjRePosterViewModel.RePosterModel;
        }

        private void RePosterSearchQueryControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            ObjRePosterViewModel.AddQueryOnSearchControl(sender);
            RePosterSearchQueryControl.CurrentQuery = new QueryInfo();
        }

        private void RepostConfiguration_OnLoaded(object sender, RoutedEventArgs e)
        {
            accountgrothHeader.AccountItemSource = lstAccounts;
            SetDataContext();
        }
    }
}
