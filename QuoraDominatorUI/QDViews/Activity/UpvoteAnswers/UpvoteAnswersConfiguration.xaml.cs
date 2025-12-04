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

namespace QuoraDominatorUI.QDViews.Activity.UpvoteAnswers
{
    public class UpvoteAnswersConfigurationbase : ModuleSettingsUserControl<UpvoteAnswersViewModel, UpvoteAnswersModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UpvoteAnswersModel =
                        JsonConvert.DeserializeObject<UpvoteAnswersModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UpvoteAnswersViewModel();


                ObjViewModel.UpvoteAnswersModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for UpvoteAnswersConfiguration.xaml
    /// </summary>
    public partial class UpvoteAnswersConfiguration
    {
        public UpvoteAnswersConfiguration()
        {
            InitializeComponent();

            InitializeBaseClass(
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UpvoteAnswersSearchControl,
                activityType: ActivityType.UpvoteAnswers,
                moduleName: QdMainModule.Voting.ToString(),
                MainGrid: MainGrid
            );

            VideoTutorialLink = ConstantHelpDetails.UpvoteAnswersVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static UpvoteAnswersConfiguration CurrentUpvoteAnswersConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF UpvoteAnswersConfiguration
        /// </summary>
        /// <returns></returns>
        public static UpvoteAnswersConfiguration GetSingeltonObjectUpvoteAnswersConfiguration()
        {
            return CurrentUpvoteAnswersConfiguration ??
                   (CurrentUpvoteAnswersConfiguration = new UpvoteAnswersConfiguration());
        }
    }
}