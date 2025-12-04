using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.GrowConnection
{
    /// <summary>
    ///     Interaction logic for FollowPageConfiguration.xaml
    /// </summary>
    public class FollowPageConfigurationBase : ModuleSettingsUserControl<FollowPagesViewModel, FollowPagesModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowPagesModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowPagesModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new FollowPagesViewModel();
                ObjViewModel.FollowPagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    public partial class FollowPageConfiguration
    {
        public FollowPageConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FollowPages,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static FollowPageConfiguration CurrentFollowPageConfiguration { get; set; }

        public static FollowPageConfiguration GetSingeltonObjectFollowPageConfiguration()
        {
            return CurrentFollowPageConfiguration ?? (CurrentFollowPageConfiguration = new FollowPageConfiguration());
        }
    }
}