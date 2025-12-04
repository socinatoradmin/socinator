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
    public class FollowBackBase : ModuleSettingsUserControl<FollowBackViewModel, FollowBackModel>
    {
    }

    /// <summary>
    ///     Interaction logic for FollowBack.xaml
    /// </summary>
    public sealed partial class FollowBack
    {
        private FollowBackViewModel _objFollowBackViewModel;

        public FollowBack()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: FollowBackHeader,
                footer: FollowBackFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.FollowBack,
                moduleName: PdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowBackVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowBackKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static FollowBack CurrentFollowBack { get; set; }

        public FollowBackViewModel ObjFollowBackViewModel
        {
            get => _objFollowBackViewModel;
            set
            {
                _objFollowBackViewModel = value;
                OnPropertyChanged(nameof(_objFollowBackViewModel));
            }
        }

        /// <summary>
        ///     GetSingeltonObjectFollowBack is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static FollowBack GetSingeltonObjectFollowBack()
        {
            return CurrentFollowBack ?? (CurrentFollowBack = new FollowBack());
        }
    }
}