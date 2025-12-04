using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
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
using YoutubeDominatorUI.Utility;
using static YoutubeDominatorCore.YDEnums.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore;
using MahApps.Metro.Controls.Dialogs;

namespace YoutubeDominatorUI.YDViews.Scraper
{
    public class UserScraper_Base : ModuleSettingsUserControl<UserScraper_ViewModel, UserScraperModel>
    {
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

    public partial class UserScraper : UserScraper_Base
    {

        private static UserScraper Current_UserScraper { get; set; } = null;
        public static UserScraper GetSingeltonObjectUserScraper()
        {
            return Current_UserScraper ?? (Current_UserScraper = new UserScraper());
        }
        public UserScraper()
        {

            InitializeComponent();

            base.InitializeBaseClass
            (
                header: HeaderGrid,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: YdMainModule.UserScraper.ToString()
            );

            Current_UserScraper = this;
            try
            {
                DialogParticipation.SetRegister(this, this);
                base.SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private UserScraper_ViewModel _objUserScraper_ViewModel;

        public UserScraper_ViewModel ObjUserScraperViewModel
        {
            get
            {
                return _objUserScraper_ViewModel;
            }
            set
            {
                _objUserScraper_ViewModel = value;
                OnPropertyChanged(nameof(ObjUserScraperViewModel));
            }
        }





        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void UserScraperSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void UserScraperFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
            => base.FooterControl_OnSelectAccountChanged(sender, e);


        private void UserScraperFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
            => base.CreateCampaign();
            //=> base.FooterControl_OnCreateCampaignChanged(sender, e, Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithUserscraperConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString(), ObjViewModel.Model.JobConfiguration.RunningTime);
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e);

        private void UserScraperFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
            => UpdateCampaign();
      //=> base.FooterControl_OnUpdateCampaignChanged(sender, e);

        private void UserScraperSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));

    }
}