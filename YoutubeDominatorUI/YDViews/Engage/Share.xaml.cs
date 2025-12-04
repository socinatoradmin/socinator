using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorCore.YoutubeViewModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using YoutubeDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

using static YoutubeDominatorCore.YDEnums.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore;

namespace YoutubeDominatorUI.YDViews.Engage
{
    public class Share_Base : ModuleSettingsUserControl<Share_ViewModel, Share_Model>
    {
        public override void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType)
        { }// => GlobalMethods.AddNewCampaign(lstSelectedAccounts, moduleType);

        public override void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
        { }// => GlobalMethods.SaveDetails(lstSelectedAccounts, moduleType);

        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one query.",
                    MessageDialogStyle.Affirmative);
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class Share : Share_Base
    {

        private static Share Current_Share { get; set; } = null;
        public static Share GetSingeltonObjectShare()
        {
            return Current_Share ?? (Current_Share = new Share());
        }
        public Share()
        {

            InitializeComponent();

            base.InitializeBaseClass
            (
                header: HeaderGrid,
                footer: ShareFooter,
                queryControl: ShareSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Share,
                moduleName: YdMainModule.Share.ToString()
            );

            Current_Share = this;
            try
            {

                base.SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private Share_ViewModel _objShare_ViewModel;

        public Share_ViewModel ObjShareViewModel
        {
            get
            {
                return _objShare_ViewModel;
            }
            set
            {
                _objShare_ViewModel = value;
                OnPropertyChanged(nameof(ObjShareViewModel));
            }
        }





        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShareSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void ShareFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
            => base.FooterControl_OnSelectAccountChanged(sender, e);


        private void ShareFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
            => base.CreateCampaign();
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e, Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithShareConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString(), ObjViewModel.Model.JobConfiguration.RunningTime);
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e);

        private void ShareFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
            => UpdateCampaign();
      //=> base.FooterControl_OnUpdateCampaignChanged(sender, e);

        private void ShareSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));

    }

}
