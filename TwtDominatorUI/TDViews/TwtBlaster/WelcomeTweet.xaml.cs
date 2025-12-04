using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtBlaster
{
    public class WelcomeTweetBase : ModuleSettingsUserControl<WelcomeTweetViewModel, WelcomeTweetModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (string.IsNullOrEmpty(ObjViewModel.WelcomeTweetModel?.WelcomeMessageText.Trim()))
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one message");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for WelcomeTweet.xaml
    /// </summary>
    public partial class WelcomeTweet
    {
        private WelcomeTweet()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: WelcomeTweetHeader,
                footer: WelcomeTweetFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.WelcomeTweet,
                moduleName: TdMainModule.TwtBlaster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.WelcomeTweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.WelcomeTweetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject Creation

        private static WelcomeTweet objWelcomeTweet { get; set; }

        public static WelcomeTweet GetSingletonObjectobjWelcomeTweet()
        {
            return objWelcomeTweet ?? (objWelcomeTweet = new WelcomeTweet());
        }

        #endregion
    }
}