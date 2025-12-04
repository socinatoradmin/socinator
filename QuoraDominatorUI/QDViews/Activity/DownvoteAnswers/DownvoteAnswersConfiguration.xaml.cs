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

namespace QuoraDominatorUI.QDViews.Activity.DownvoteAnswers
{
    public class
        DownvoteAnswersConfigurationbase : ModuleSettingsUserControl<DownvoteAnswersViewModel, DownvoteAnswersModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DownvoteAnswersModel =
                        JsonConvert.DeserializeObject<DownvoteAnswersModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new DownvoteAnswersViewModel();


                ObjViewModel.DownvoteAnswersModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DownvoteAnswersConfiguration.xaml
    /// </summary>
    public partial class DownvoteAnswersConfiguration
    {
        public DownvoteAnswersConfiguration()
        {
            InitializeComponent();

            InitializeBaseClass(
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: DownvoteAnswersSearchControl,
                activityType: ActivityType.DownvoteAnswers,
                moduleName: QdMainModule.Voting.ToString(),
                MainGrid: MainGrid
            );

            VideoTutorialLink = ConstantHelpDetails.DownvoteAnswersVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static DownvoteAnswersConfiguration CurrentDownvoteAnswersConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF DownvoteAnswersConfiguration
        /// </summary>
        /// <returns></returns>
        public static DownvoteAnswersConfiguration GetSingeltonObjectDownvoteAnswersConfiguration()
        {
            return CurrentDownvoteAnswersConfiguration ??
                   (CurrentDownvoteAnswersConfiguration = new DownvoteAnswersConfiguration());
        }
    }
}