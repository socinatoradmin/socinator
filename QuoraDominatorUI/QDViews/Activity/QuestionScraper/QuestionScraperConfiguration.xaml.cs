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

namespace QuoraDominatorUI.QDViews.Activity.QuestionScraper
{
    /// <summary>
    ///     Interaction logic for QuestionScraperConfiguration.xaml
    /// </summary>
    public class
        QuestionScraperConfigurationBase : ModuleSettingsUserControl<QuestionScraperViewModel, QuestionsScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.QuestionsScraperModel =
                        JsonConvert.DeserializeObject<QuestionsScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new QuestionScraperViewModel();


                ObjViewModel.QuestionsScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class QuestionScraperConfiguration
    {
        public QuestionScraperConfiguration()
        {
            InitializeComponent();

            VideoTutorialLink = ConstantHelpDetails.QuestionsScraperVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.QuestionsScraper,
                QdMainModule.Scrape.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: QuestionsScraperSearchControl
            );
        }
    }
}