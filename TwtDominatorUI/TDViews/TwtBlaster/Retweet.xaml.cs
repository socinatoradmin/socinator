using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtBlaster
{
    public class RetweetBase : ModuleSettingsUserControl<RetweetViewModel, RetweetModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for Retweet.xaml
    /// </summary>
    public partial class Retweet : RetweetBase
    {
        private Retweet()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: RetweetHeader,
                footer: RetweetFooter,
                queryControl: RetweetSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Retweet,
                moduleName: TdMainModule.TwtBlaster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.RetweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.RetweetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject Creation

        private static Retweet objRetweet;

        public static Retweet GetSingletonObjectRetweet()
        {
            return objRetweet ?? (objRetweet = new Retweet());
        }

        #endregion
    }
}