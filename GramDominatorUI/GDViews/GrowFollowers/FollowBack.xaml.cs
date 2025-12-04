using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.GrowFollowers
{
    public class FollowBackBase : ModuleSettingsUserControl<FollowBackViewModel, FollowBackModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check AutoFollow.Unfollow
            if (!Model.IsFollowBack && !Model.IsAcceptFollowRequest)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please select atleast one checkbox option either follow back or Accept follow request");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for FollowBack.xaml
    /// </summary>
    public partial class FollowBack : FollowBackBase
    {
        private FollowBack()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: FollowBackHeader,
                footer: FollowBackFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.FollowBack,
                moduleName: Enums.GdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowBackVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowBackKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static FollowBack CurrentFollowBack { get; set; }

        public static FollowBack GetSingeltonObjectFollowBack()
        {
            return CurrentFollowBack ?? (CurrentFollowBack = new FollowBack());
        }
    }
}