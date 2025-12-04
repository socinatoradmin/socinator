using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.StoryViewer;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.StoryViews
{
    public partial class AddInstaStoryBase : ModuleSettingsUserControl<AddInstaStoryViewModel, AddInstaStoryModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }
    /// <summary>
    /// Interaction logic for AddInstaStory.xaml
    /// </summary>
    public partial class AddInstaStory : AddInstaStoryBase
    {
        private static AddInstaStory _instance;
        public AddInstaStory()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AddStoryHeader,
                footer: AddstoryFooter,
                queryControl: AddStoriesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.AddStory,
                moduleName: GdMainModule.StoryViewer.ToString()
            );

            // Help control links. 
            //VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            //KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            //ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
        public static AddInstaStory Instance => _instance ?? (_instance = new AddInstaStory());
    }
}
