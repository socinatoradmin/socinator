using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.GrowFollower;
using GramDominatorCore.GDUtility;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.Enums;
using static GramDominatorCore.GDEnums.Enums;
namespace GramDominatorUI.GDViews.GrowFollowers
{
    public partial class CloseFriendTabBase : ModuleSettingsUserControl<CloseFriendViewModel, CloseFriendModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if(!Model.IsCheckAllFollowers && !Model.IsCheckedCustomFollowerList)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please Select At Least One Follower Source To Make Close Friends");
                return false;
            }else if(Model.IsCheckedCustomFollowerList && (string.IsNullOrEmpty(Model.CustomFollowerList) || string.IsNullOrWhiteSpace(Model.CustomFollowerList)))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please Provide At Least One Custom Follower To Make Close Friends");
                return false;
            }
            return base.ValidateExtraProperty();
        }
    }
    /// <summary>
    /// Interaction logic for CloseFriendTab.xaml
    /// </summary>
    public partial class CloseFriendTab : CloseFriendTabBase
    {
        public CloseFriendTab()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: CloseFriendHeader,
                footer: CloseFriendFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.CloseFriend,
                moduleName: GdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AddToCloseFriendVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.AddToCloseFriendKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
        private static CloseFriendTab instance;
        public static CloseFriendTab GetSingletonInstance()=> instance ?? (instance = new CloseFriendTab());
    }
}
