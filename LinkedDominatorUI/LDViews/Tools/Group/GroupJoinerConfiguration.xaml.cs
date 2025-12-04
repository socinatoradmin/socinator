using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Group;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Group
{
    public class GroupJoinerConfigurationBase : ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.GroupJoinerModel =
                        templateModel.ActivitySettings.GetActivityModel<GroupJoinerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new GroupJoinerViewModel();
                ObjViewModel.GroupJoinerModel.IsAccountGrowthActive = isToggleActive;
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

    /// <summary>
    ///     Interaction logic for GroupJoinerConfiguration.xaml
    /// </summary>
    public partial class GroupJoinerConfiguration : GroupJoinerConfigurationBase
    {
        public GroupJoinerConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.GroupJoiner,
                LdMainModules.Group.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: GroupJoinerSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.GroupJoinerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.GroupJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static GroupJoinerConfiguration CurrenGroupJoinerConfiguration { get; set; }

        public static GroupJoinerConfiguration GetSingeltonObjectGroupJoinerConfiguration()
        {
            return CurrenGroupJoinerConfiguration ?? (CurrenGroupJoinerConfiguration = new GroupJoinerConfiguration());
        }
    }
}