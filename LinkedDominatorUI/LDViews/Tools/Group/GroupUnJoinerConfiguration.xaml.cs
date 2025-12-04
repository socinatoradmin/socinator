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
    public class GroupUnJoinerConfigurationBase : ModuleSettingsUserControl<GroupUnJoinerViewModel, GroupUnJoinerModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.GroupUnJoinerModel =
                        templateModel.ActivitySettings.GetActivityModel<GroupUnJoinerModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new GroupUnJoinerViewModel();
                ObjViewModel.GroupUnJoinerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.GroupUnJoinerModel.IsCheckedBySoftware
                && !ObjViewModel.GroupUnJoinerModel.IsCheckedOutSideSoftware
                && !ObjViewModel.GroupUnJoinerModel.IsCheckedCustomGroupList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfGroupSources".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for GroupJoinerConfiguration.xaml
    /// </summary>
    public partial class GroupUnJoinerConfiguration : GroupUnJoinerConfigurationBase
    {
        public GroupUnJoinerConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.GroupUnJoiner,
                LdMainModules.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.GroupUnJoinerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.GroupUnJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static GroupUnJoinerConfiguration CurrentGroupUnJoinerConfiguration { get; set; }

        public static GroupUnJoinerConfiguration GetSingeltonObjectGroupUnJoinerConfiguration()
        {
            return CurrentGroupUnJoinerConfiguration ??
                   (CurrentGroupUnJoinerConfiguration = new GroupUnJoinerConfiguration());
        }
    }
}