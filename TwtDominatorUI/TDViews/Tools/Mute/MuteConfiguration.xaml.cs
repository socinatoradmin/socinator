using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.GrowFollower;

namespace TwtDominatorUI.TDViews.Tools.Mute
{
    public class MuteConfigurationBase : ModuleSettingsUserControl<MuteViewModel, MuteModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MuteModel =
                        templateModel.ActivitySettings.GetActivityModel<MuteModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new MuteViewModel();

                ObjViewModel.MuteModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for MuteConfiguration.xaml
    /// </summary>
    public partial class MuteConfiguration : MuteConfigurationBase
    {
        public ObservableCollectionBase<string> lstAccounts = null;

        public MuteConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Mute,
                Enums.TdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: MuteConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.MuteVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.MuteKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}