using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtBlaster
{
    public class ReposterBase : ModuleSettingsUserControl<ReposterViewModel, ReposterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for Reposter.xaml
    /// </summary>
    public partial class Reposter : ReposterBase
    {
        private Reposter()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: ReposterHeader,
                footer: ReposterFooter,
                queryControl: ReposterSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Reposter,
                moduleName: TdMainModule.TwtBlaster.ToString()
            );

            // Help control links.
            VideoTutorialLink = TDHelpDetails.ReposterVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ReposterKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static Reposter objReposter;

        public static Reposter GetSingletonObjectReposter()
        {
            return objReposter ?? (objReposter = new Reposter());
        }

        #endregion
    }
}