using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDViewModel.Engage;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Engage
{
    /// <summary>
    ///     Interaction logic for Share.xaml
    /// </summary>
    public class ShareBase : ModuleSettingsUserControl<ShareViewModel, ShareModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class Share : ShareBase
    {
        private Share()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: ShareHeader,
                footer: ShareFooter,
                queryControl: ShareSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Share,
                moduleName: LdMainModules.Engage.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ShareVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ShareKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region Object creation 

        private static Share CurrentShare { get; set; }

        /// <summary>
        ///     GetSingeltonObjectShare is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Share GetSingeltonObjectShare()
        {
            return CurrentShare ?? (CurrentShare = new Share());
        }

        #endregion
    }
}