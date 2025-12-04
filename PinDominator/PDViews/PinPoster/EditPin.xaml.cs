using System.Linq;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinPoster;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinPoster
{
    public class EditPinBase : ModuleSettingsUserControl<EditPinViewModel, EditPinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.PinDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one pin.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for EditPin.xaml
    /// </summary>
    public partial class EditPin
    {
        private EditPin()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: EdiPinHeaderControl,
                footer: EditPinFooterControl,
                MainGrid: MainGrid,
                activityType: ActivityType.EditPin,
                moduleName: PdMainModule.Poster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.EditPinsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.EditPinsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static EditPin CurrentEditPin { get; set; }

        /// <summary>
        ///     GetSingletonObjectRePin﻿ is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static EditPin GetSingletonObjectEditPin()
        {
            return CurrentEditPin ?? (CurrentEditPin = new EditPin());
        }

        private void EditPin_OnLoaded(object sender, RoutedEventArgs e)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll(SocialNetworks.Pinterest)
                .Where(account => account.AccountBaseModel.Status == AccountStatus.Success);

            ObjViewModel.EditPinModel.ListAccounts =
                accounts.Select(account => account.AccountBaseModel.UserName).ToList();
        }
    }
}