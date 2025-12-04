using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.GrowFollower;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PinDominator.PDViews.Tools.Follow
{
    public class FollowerConfigurationBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.ChkCommentOnUserLatestPostsChecked && Model.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after follow action.");
                return false;
            }

            if (Model.ChkTryUserLatestPostsChecked && Model.LstNotes.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after follow action.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowerModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new FollowerViewModel();

                ObjViewModel.FollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    public partial class FollowConfiguration
    {
        private FollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                Enums.PdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static FollowConfiguration CurrentFollowConfiguration { get; set; }

        public static FollowConfiguration GetSingletonObjectFollowConfiguration()
        {
            return CurrentFollowConfiguration ?? (CurrentFollowConfiguration = new FollowConfiguration());
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = string.Empty;
        }
    }
}