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
using QuoraDominatorCore.ViewModel.Voting;

namespace QuoraDominatorUI.QDViews.Activity.DownvoteQuestions
{
    public class
        DownvoteQuestionsConfigurationbase : ModuleSettingsUserControl<DownvoteQuestionsViewModel,
            DownvoteQuestionsModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DownvoteQuestionsModel =
                        JsonConvert.DeserializeObject<DownvoteQuestionsModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new DownvoteQuestionsViewModel();


                ObjViewModel.DownvoteQuestionsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DownvoteQuestionsConfiguration.xaml
    /// </summary>
    public partial class DownvoteQuestionsConfiguration
    {
        public DownvoteQuestionsConfiguration()
        {
            InitializeComponent();

            InitializeBaseClass(
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: DownvoteQuestionsSearchControl,
                activityType: ActivityType.DownvoteQuestions,
                moduleName: QdMainModule.Voting.ToString(),
                MainGrid: MainGrid
            );
            VideoTutorialLink = ConstantHelpDetails.DownvoteQuestionsVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static DownvoteQuestionsConfiguration CurrentDownvoteQuestionsConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF DownvoteQuestionsConfiguration
        /// </summary>
        /// <returns></returns>
        public static DownvoteQuestionsConfiguration GetSingeltonObjectDownvoteQuestionsConfiguration()
        {
            return CurrentDownvoteQuestionsConfiguration ??
                   (CurrentDownvoteQuestionsConfiguration = new DownvoteQuestionsConfiguration());
        }
    }
}