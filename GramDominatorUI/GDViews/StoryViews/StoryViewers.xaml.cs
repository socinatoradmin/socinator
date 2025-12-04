using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.StoryViewer;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.StoryViews
{
    public class StoryViewersBase : ModuleSettingsUserControl<StoryViewModel, StoryModel>
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
    ///     Interaction logic for StoryViewers.xaml
    /// </summary>
    public partial class StoryViewers : StoryViewersBase
    {
        private StoryViewers()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: StoryHeader,
                footer: storyFooter,
                queryControl: StoriesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.StoryViewer,
                moduleName: GdMainModule.StoryViewer.ToString()
            );

            // Help control links. 
            //VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            //KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            //ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static StoryViewers objStoryViewers { get; set; }

        public static StoryViewers GetSingletonViewers()
        {
            return objStoryViewers ?? (objStoryViewers = new StoryViewers());
        }
    }
}