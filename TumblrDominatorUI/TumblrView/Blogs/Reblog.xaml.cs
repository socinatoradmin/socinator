using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Blog;

namespace TumblrDominatorUI.TumblrView.Blogs
{
    public class ReblogBase : ModuleSettingsUserControl<ReblogViewModel, ReblogModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <inheritdoc cref="ReblogUserControl" />
    /// <summary>
    ///     Interaction logic for Reblog.xaml
    /// </summary>
    public sealed partial class Reblog
    {
        private static Reblog _objReblog;

        private Reblog()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: ReblogFooter,
                queryControl: ReblogSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Reblog,
                moduleName: Enums.TmbMainModule.Blog.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.ReblogVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ReblogKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ReblogContactLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        /// <summary>
        ///     GetSingeltonObjectReblog is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Reblog GetSingeltonObjectReblog()
        {
            return _objReblog ?? (_objReblog = new Reblog());
        }
    }
}