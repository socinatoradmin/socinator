using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.GrowConnection
{
    /// <summary>
    ///     Interaction logic for WithdrawConnectionRequest.xaml
    /// </summary>
    public class WithdrawConnectionRequestBase : ModuleSettingsUserControl<WithdrawConnectionRequestViewModel,
        WithdrawConnectionRequestModel>
    {
        protected override bool ValidateCampaign()
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

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for WithdrawConnectionRequest.xaml
    /// </summary>
    public partial class WithdrawConnectionRequest : WithdrawConnectionRequestBase
    {
        private WithdrawConnectionRequest()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: Header,
                footer: WithdrawConnectionRequestFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.WithdrawConnectionRequest,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.WithdrawConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.WithdrawConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static WithdrawConnectionRequest CurrentWithdrawConnectionRequest { get; set; }

        /// <summary>
        ///     GetSingeltonObjectRemoveConnections is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static WithdrawConnectionRequest GetSingeltonObjectWithdrawConnectionRequest()
        {
            return CurrentWithdrawConnectionRequest ??
                   (CurrentWithdrawConnectionRequest = new WithdrawConnectionRequest());
        }

        public static WithdrawConnectionRequest GetNewSingeltonObjectWithdrawConnectionRequest()
        {
            try
            {
                return new WithdrawConnectionRequest();
                // return demo;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}