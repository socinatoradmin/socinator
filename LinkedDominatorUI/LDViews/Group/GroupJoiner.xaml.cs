using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Group;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Group
{
    public class GroupJoinerBase : ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>
    {
    }


    /// <summary>
    ///     Interaction logic for GroupJoiner.xaml
    /// </summary>
    public partial class GroupJoiner : GroupJoinerBase
    {
        public GroupJoiner()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: GroupJoinerHeader,
                footer: GroupJoinerFooter,
                queryControl: GroupJoinerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupJoiner,
                moduleName: LdMainModules.Group.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.GroupJoinerVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.GroupJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static GroupJoiner CurrentGroupJoiner { get; set; }

        /// <summary>
        ///     GetSingeltonObjectGroupJoiner is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static GroupJoiner GetSingeltonObjectGroupJoiner()
        {
            return CurrentGroupJoiner ?? (CurrentGroupJoiner = new GroupJoiner());
        }
    }
}