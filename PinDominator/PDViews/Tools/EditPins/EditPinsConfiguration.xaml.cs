using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinPoster;
using System;
using System.Linq;

namespace PinDominator.PDViews.Tools.EditPins
{
    public class EditPinsConfigurationBase : ModuleSettingsUserControl<EditPinViewModel, EditPinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.PinDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one pin.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.EditPinModel =
                        templateModel.ActivitySettings.GetActivityModel<EditPinModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new EditPinViewModel();

                ObjViewModel.EditPinModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for EditPinsConfiguration.xaml
    /// </summary>
    public partial class EditPinsConfiguration
    {
        public EditPinsConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.EditPin,
                Enums.PdMainModule.Poster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.DeletePinsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DeletePinsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;


            LoadAccount();
        }

        private static EditPinsConfiguration CurrentEditPinsConfiguration { get; set; }

        public static EditPinsConfiguration GetSingletonObjectEditPinsConfiguration()
        {
            return CurrentEditPinsConfiguration ?? (CurrentEditPinsConfiguration = new EditPinsConfiguration());
        }


        public void LoadAccount()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll(SocialNetworks.Pinterest)
                .Where(account => account.AccountBaseModel.Status == AccountStatus.Success);

            ObjViewModel.EditPinModel.ListAccounts =
                accounts.Select(account => account.AccountBaseModel.UserName).ToList();
        }
    }
}