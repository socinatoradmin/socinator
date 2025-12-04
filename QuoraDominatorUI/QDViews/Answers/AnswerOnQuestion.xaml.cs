using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Scrape;

namespace QuoraDominatorUI.QDViews.Answers
{
    public class AnswersQuestionBase : ModuleSettingsUserControl<AnswerQuestionViewModel, AnswerQuestionModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog("Input Error", "Please provide answer");
                return false;
            }

            return true;
        }
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for AnswerOnQuestion.xaml
    /// </summary>
    public partial class AnswerOnQuestion
    {
        private static AnswerOnQuestion _answerOnQuestion;

        public AnswerOnQuestion()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AnswersQuestionHeader,
                footer: AnswersQuestionFooter,
                queryControl: AnswersQuestionSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.AnswerOnQuestions,
                moduleName: QdMainModule.Answer.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AnswersQuestionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AnswersQuestionKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static AnswerOnQuestion GetSingeltonAnswerOnQuestion()
        {
            return _answerOnQuestion ?? (_answerOnQuestion = new AnswerOnQuestion());
        }
    }
}