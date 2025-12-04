using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaPoster;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaPoster
{
    public class RePosterBase : ModuleSettingsUserControl<RePosterViewModel, RePosterModel>
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
    ///     Interaction logic for RePoster.xaml
    /// </summary>
    public partial class RePoster : RePosterBase
    {
        private RePoster()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: RePosterHeader,
                footer: RePosterFooter,
                queryControl: RePosterSearchQueryControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Reposter,
                moduleName: GdMainModule.Poster.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.RePosterVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RePosterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        /// <summary>
        ///     This method set DataContext of RePoster model
        /// </summary>


        private static RePoster CurrentRePoster { get; set; }

        /// <summary>
        ///     GetSingeltonObjectRePoster is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static RePoster GetSingeltonObjectRePoster()
        {
            return CurrentRePoster ?? (CurrentRePoster = new RePoster());
        }
    }
}