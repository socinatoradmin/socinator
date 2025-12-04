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
    public class RemoveConnectionBase : ModuleSettingsUserControl<RemoveConnectionViewModel, RemoveConnectionModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!ObjViewModel.RemoveConnectionModel.IsCheckedBySoftware
                && !ObjViewModel.RemoveConnectionModel.IsCheckedOutSideSoftware
                && !ObjViewModel.RemoveConnectionModel.IsCheckedLangKeyCustomUserList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for RemoveOrWithdrawConnections.xaml
    /// </summary>
    public partial class RemoveConnection : RemoveConnectionBase
    {
        private RemoveConnection()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: RemoveConnectionsHeader,
                footer: RemoveConnectionsFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.RemoveConnections,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RemoveConnectionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RemoveConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static RemoveConnection CurrentRemoveConnection { get; set; }

        /// <summary>
        ///     GetSingeltonObjectRemoveConnections is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static RemoveConnection GetSingeltonObjectRemoveConnection()
        {
            return CurrentRemoveConnection ?? (CurrentRemoveConnection = new RemoveConnection());
        }
    }
}