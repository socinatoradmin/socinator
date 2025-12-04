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
    public class ConnectionRequestBase : ModuleSettingsUserControl<ConnectionRequestViewModel, ConnectionRequestModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!ValidateQuery())
                return false;

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for ConnectionRequest.xaml
    /// </summary>
    public partial class ConnectionRequest : ConnectionRequestBase
    {
        private ConnectionRequest()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ConnectionHeader,
                footer: ConnectionFooter,
                queryControl: ConnectionSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ConnectionRequest,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static ConnectionRequest CurrentConnectionRequest { get; set; }

        /// <summary>
        ///     GetSingeltonObjectConnectionRequest is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static ConnectionRequest GetSingeltonObjectConnectionRequest()
        {
            return CurrentConnectionRequest ?? (CurrentConnectionRequest = new ConnectionRequest());
        }
    }
}