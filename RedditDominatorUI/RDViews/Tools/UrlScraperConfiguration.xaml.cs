using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.RDViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RedditDominatorUI.RDViews.Tools
{
    public class UrlScraperConfigurationBase : ModuleSettingsUserControl<UrlScraperViewModel, UrlScraperModel>
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
            return true;
        }
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.UrlScraperModel =
                        JsonConvert.DeserializeObject<UrlScraperModel>(templateModel.ActivitySettings);

                ObjViewModel.UrlScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }
    }
    /// <summary>
    /// Interaction logic for UrlScraperConfiguration.xaml
    /// </summary>
    public partial class UrlScraperConfiguration : UrlScraperConfigurationBase
    {
        public UrlScraperConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            base.InitializeBaseClass
           (
            MainGrid: MainGrid,
               activityType: ActivityType.UrlScraper,
               moduleName: Enums.RDMainModule.GrowUrlScrper.ToString(),
               accountGrowthModeHeader: accountGrowthHeader,
               queryControl: UrlScrperConfigurationSearchControl
          );
            // Help control links. 
            base.VideoTutorialLink = ConstantHelpDetails.UrlScraperVideoTutorialsLink;
            base.KnowledgeBaseLink = ConstantHelpDetails.UrlScraperKnowledgeBaseLink;
            base.ContactSupportLink = ConstantHelpDetails.UrlScraperContactLink;
            //var account = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit).Select(x=>x.UserName).ToList();
            //accountGrowthHeader.AccountItemSource = new ObservableCollectionBase<string>(account);
            //accountGrowthHeader.SelectedItem = SelectedDominatorAccounts.PdAccounts;
            MainGrid.DataContext = ObjViewModel.Model;
        }
        private static UrlScraperConfiguration CurrentUrlScraperConfiguration { get; set; } = null;
        public SearchQueryControl UrlScrperConfigurationSearchControl { get; private set; }

        public static UrlScraperConfiguration GetSingeltonObjectUrlScraperConfiguration()
         => CurrentUrlScraperConfiguration ?? (CurrentUrlScraperConfiguration = new UrlScraperConfiguration());

        private void accountgrowthHeader_SelectionChanged(object sender, RoutedEventArgs e)
            => base.SetAccountModeDataContext(SocialNetworks.Reddit);

        private void UrlScraperConfigurationSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnAddQuery(sender, e);
        private void AccountgrothHeader_OnSaveClick(object sender, RoutedEventArgs e)
            => base.SaveAccountGrowthSettings();
        private void TglStatus_IsCheckedChanged(object sender, EventArgs e)
        {
            ScheduleJobFromGrowthMode(ObjViewModel.Model.IsAccountGrowthActive, SelectedDominatorAccounts.RdAccounts, SocialNetworks.Reddit);
        }
        private void UrlScraperConfiguration_OnLoaded(object sender, RoutedEventArgs e)
        {
            base.SetSelectedAccounts(SocialNetworks.Reddit, SelectedDominatorAccounts.RdAccounts);
        }
    }
}
