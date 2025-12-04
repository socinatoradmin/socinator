using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.Scrape;

namespace QuoraDominatorUI.QDViews.Activity.UserScraper
{
    /// <summary>
    ///     Interaction logic for UserScraperConfiguration.xaml
    /// </summary>
    public class UserScraperConfigurationBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UserScraperModel =
                        JsonConvert.DeserializeObject<UserScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UserScraperViewModel();

                ObjViewModel.UserScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class UserScraperConfiguration
    {
        public UserScraperConfiguration()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.UserScrapersVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                QdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperSearchControl
            );
        }
    }
}