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
    public class GroupUnJoinerBase : ModuleSettingsUserControl<GroupUnJoinerViewModel, GroupUnJoinerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.GroupUnJoinerModel.IsCheckedBySoftware
                && !ObjViewModel.GroupUnJoinerModel.IsCheckedOutSideSoftware
                && !ObjViewModel.GroupUnJoinerModel.IsCheckedCustomGroupList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfGroupSources".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for GroupUnJoiner.xaml
    /// </summary>
    public partial class GroupUnJoiner : GroupUnJoinerBase
    {
        public GroupUnJoiner()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: GroupUnJoinerHeader,
                footer: GroupUnJoinerFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupUnJoiner,
                moduleName: LdMainModules.Group.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.GroupUnJoinerVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.GroupUnJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static GroupUnJoiner CurrentGroupUnJoiner { get; set; }

        /// <summary>
        ///     GetSingeltonObjectGroupUnJoiner is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static GroupUnJoiner GetSingeltonObjectGroupUnJoiner()
        {
            return CurrentGroupUnJoiner ?? (CurrentGroupUnJoiner = new GroupUnJoiner());
        }
    }
}