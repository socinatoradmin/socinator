using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Accounts;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.CreateAccount
{
    public class AccountCreatorBase : ModuleSettingsUserControl<AccountCreatorViewModel, AccountCreatorModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.ObsCreateAccountInfo.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one account details.");
                return false;
            }
            else if (_footerControl.list_SelectedAccounts.Count != 1)
            {
                Dialog.ShowDialog(this, "Error", "Please select any one account, but not more than one.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for AccountCreator.xaml
    /// </summary>
    public partial class AccountCreator
    {
        public AccountCreator()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AccountCreatorHeaderControl,
                footer: CreateAccountFooterControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CreateAccount,
                moduleName: PdMainModule.Account.ToString()
            );
            VideoTutorialLink = ConstantHelpDetails.CreateAccountVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.CreateAccountKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static AccountCreator CurrentAccountCreator { get; set; }

        public static AccountCreator GetSingletonAccountCreator()
        {
            return CurrentAccountCreator ?? (CurrentAccountCreator = new AccountCreator());
        }
    }
}