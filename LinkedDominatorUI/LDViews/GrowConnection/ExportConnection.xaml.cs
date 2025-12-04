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
    public class ExportConnectionBase : ModuleSettingsUserControl<ExportConnectionViewModel, ExportConnectionModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!ObjViewModel.ExportConnectionModel.IsCheckedBySoftware
                && !ObjViewModel.ExportConnectionModel.IsCheckedOutSideSoftware
                && !ObjViewModel.ExportConnectionModel.IsCheckedLangKeyCustomUserList)

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for ExportConnection.xaml
    /// </summary>
    public partial class ExportConnection : ExportConnectionBase
    {
        public ExportConnection()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ExportConnectionHeader,
                footer: ExportConnectionFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.ExportConnection,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ExportConnectionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ExportConnectionKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static ExportConnection CurrentExportConnection { get; set; }

        /// <summary>
        ///     GetSingeltonObjectExportConnection is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static ExportConnection GetSingeltonObjectExportConnection()
        {
            return CurrentExportConnection ?? (CurrentExportConnection = new ExportConnection());
        }
    }
}