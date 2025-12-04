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
    public class BlockFollowerBase : ModuleSettingsUserControl<BlockFollowerViewModel, BlockFollowerModel>
    {
    }

    /// <summary>
    ///     Interaction logic for BlockFollower.xaml
    /// </summary>
    public partial class BlockFollower : BlockFollowerBase
    {
        private BlockFollower()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: BlockFollowerHeader,
                footer: BlockFollowerFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.BlockFollower,
                moduleName: Enums.GdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BlockFollowerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BlockFollowerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static BlockFollower CurrentBlockFollower { get; set; }

        public static BlockFollower GetSingeltonObjectBlockFollower()
        {
            return CurrentBlockFollower ?? (CurrentBlockFollower = new BlockFollower());
        }
    }
}