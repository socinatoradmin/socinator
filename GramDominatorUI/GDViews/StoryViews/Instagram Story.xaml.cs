using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.StoryViewer;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.StoryViews
{
    public class InstagramStoryViewersBase : ModuleSettingsUserControl<InstagramStoryViewModel, InstagramStoryModel>
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
    /// Interaction logic for Instagram_Story.xaml
    /// </summary>
    public partial class Instagram_Story : InstagramStoryViewersBase
    {
        public Instagram_Story()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: InstaStoryHeader,
                footer: InstaStoryFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.InstaStory,
                moduleName: GdMainModule.StoryViewer.ToString()
            );
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
        private static Instagram_Story objStoryViewers { get; set; }

        public static Instagram_Story GetSingletonViewers()
        {
            return objStoryViewers ?? (objStoryViewers = new Instagram_Story());
        }
    }
}
