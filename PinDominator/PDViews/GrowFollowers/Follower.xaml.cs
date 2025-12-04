using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.GrowFollower;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.GrowFollowers
{
    public class FollowerBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.ChkCommentOnUserLatestPostsChecked && Model.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after follow action.");
                return false;
            }

            if (Model.ChkTryUserLatestPostsChecked && Model.LstNotes.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after follow action.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Follower.xaml
    /// </summary>
    public sealed partial class Follower
    {
        /// Constructor
        private Follower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: PdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Follower CurrentFollower { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Follower GetSingeltonObjectFollower()
        {
            return CurrentFollower ?? (CurrentFollower = new Follower());
        }


        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = string.Empty;
        }
    }
}