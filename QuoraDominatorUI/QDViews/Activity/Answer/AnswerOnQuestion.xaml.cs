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

namespace QuoraDominatorUI.QDViews.Activity.Answer
{
    /// <summary>
    ///     Interaction logic for AnswerScraperConfiguration.xaml
    /// </summary>
    public class AnswerOnQuestionBase : ModuleSettingsUserControl<AnswerQuestionViewModel, AnswerQuestionModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AnswerQuestionModel =
                        JsonConvert.DeserializeObject<AnswerQuestionModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new AnswerQuestionViewModel();


                ObjViewModel.AnswerQuestionModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class AnswerOnQuestion
    {
        public AnswerOnQuestion()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.AnswersQuestionVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AnswerOnQuestions,
                QdMainModule.Answer.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: AnswersQuestionSearchControl
            );
        }
    }
}