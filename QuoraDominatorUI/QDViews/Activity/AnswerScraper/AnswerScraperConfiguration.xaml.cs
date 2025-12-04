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

namespace QuoraDominatorUI.QDViews.Activity.AnswerScraperConfiguration
{
    /// <summary>
    ///     Interaction logic for AnswerScraperConfiguration.xaml
    /// </summary>
    public class AnswerScraperConfigurationBase : ModuleSettingsUserControl<AnswerScraperViewModel, AnswersScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AnswersScraperModel =
                        JsonConvert.DeserializeObject<AnswersScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new AnswerScraperViewModel();


                ObjViewModel.AnswersScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class AnswerScraperConfiguration
    {
        public AnswerScraperConfiguration()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.AnswersScraperVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AnswersScraper,
                QdMainModule.Scrape.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: AnswersScraperSearchControl
            );
        }
    }
}