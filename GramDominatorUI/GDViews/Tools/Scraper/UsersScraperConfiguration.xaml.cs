using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace GramDominatorUI.GDViews.Tools.Scraper
{
    public class UsersScraperConfigurationBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UserScraperModel =
                        JsonConvert.DeserializeObject<UserScraperModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new UserScraperViewModel();
                ObjViewModel.UserScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for UsersScraperConfiguration.xaml
    /// </summary>
    public partial class UsersScraperConfiguration : UsersScraperConfigurationBase
    {
        private UsersScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                Enums.GdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
        }

        private static UsersScraperConfiguration CurrentUsersScraperConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUsersScraperConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UsersScraperConfiguration GetSingeltonObjectUsersScraperConfiguration()
        {
            return CurrentUsersScraperConfiguration ??
                   (CurrentUsersScraperConfiguration = new UsersScraperConfiguration());
        }

        private void Chk_RequiredData(object sender, RoutedEventArgs e)
        {
            ObjViewModel.CheckAllReqData(sender);
        }
    }
}