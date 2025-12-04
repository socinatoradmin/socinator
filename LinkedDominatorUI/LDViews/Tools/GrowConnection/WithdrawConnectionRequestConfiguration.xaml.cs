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
    public class WithdrawConnectionRequestConfigurationBase : ModuleSettingsUserControl<
        WithdrawConnectionRequestViewModel, WithdrawConnectionRequestModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.WithdrawConnectionRequestModel =
                        templateModel.ActivitySettings.GetActivityModel<WithdrawConnectionRequestModel>(
                            ObjViewModel.Model, true);
                else
                    ObjViewModel = new WithdrawConnectionRequestViewModel();
                ObjViewModel.WithdrawConnectionRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.WithdrawConnectionRequestModel.IsCheckedBySoftware
                && !ObjViewModel.WithdrawConnectionRequestModel.IsCheckedOutSideSoftware
                && !ObjViewModel.WithdrawConnectionRequestModel.IsCheckedLangKeyCustomUserList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.WithdrawConnectionRequestModel.IsCheckedLangKeyCustomUserList &&
                (ObjViewModel.WithdrawConnectionRequestModel.UrlList == null ||
                 ObjViewModel.WithdrawConnectionRequestModel.UrlList.Count == 0))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSaveYourCustomUsersList".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for WithdrawConnectionRequestConfiguration.xaml
    /// </summary>
    public partial class WithdrawConnectionRequestConfiguration : WithdrawConnectionRequestConfigurationBase
    {
        private WithdrawConnectionRequestConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.WithdrawConnectionRequest,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.WithdrawConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.WithdrawConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static WithdrawConnectionRequestConfiguration CurrentWithdrawConnectionRequestConfiguration
        {
            get;
            set;
        }


        public static WithdrawConnectionRequestConfiguration GetSingeltonObjectWithdrawConnectionRequestConfiguration()
        {
            return CurrentWithdrawConnectionRequestConfiguration ?? (CurrentWithdrawConnectionRequestConfiguration =
                       new WithdrawConnectionRequestConfiguration());
        }
    }
}