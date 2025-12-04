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
    public class AcceptConnectionRequestBase : ModuleSettingsUserControl<AcceptConnectionRequestViewModel,
        AcceptConnectionRequestModel>
    {
    }

    /// <summary>
    ///     Interaction logic for AcceptConnectionRequest.xaml
    /// </summary>
    public partial class AcceptConnectionRequest : AcceptConnectionRequestBase
    {
        private AcceptConnectionRequest()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AcceptConnectionHeader,
                footer: AcceptConnectionFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AcceptConnectionRequest,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AcceptConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AcceptConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static AcceptConnectionRequest CurrentAcceptConnectionRequest { get; set; }

        /// <summary>
        ///     GetSingeltonObjectAcceptConnectionRequest is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static AcceptConnectionRequest GetSingeltonObjectAcceptConnectionRequest()
        {
            return CurrentAcceptConnectionRequest ?? (CurrentAcceptConnectionRequest = new AcceptConnectionRequest());
        }
    }
}