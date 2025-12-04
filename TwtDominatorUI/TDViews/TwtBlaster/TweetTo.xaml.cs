using System.Windows;
using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;
using static TwtDominatorCore.TDEnums.Enums;


namespace TwtDominatorUI.TDViews.TwtBlaster
{
    public class TweetToBase : ModuleSettingsUserControl<TweetToViewModel, TweetToModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for TweetTo.xaml
    /// </summary>
    public partial class TweetTo : TweetToBase
    {
        private TweetTo()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: TweetToHeader,
                footer: TweetToFooter,
                queryControl: TweetToSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.TweetTo,
                moduleName: TdMainModule.TwtBlaster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TweetToVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TweetToKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private void menu_DeleteSingleImage_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.RemoveSelectedMediaExecute(sender);
        }


        #region SingletonObject Creation

        private static TweetTo _TweetTo;

        public static TweetTo GetSingletonObjectTweetTo()
        {
            return _TweetTo ?? (_TweetTo = new TweetTo());
        }

        #endregion
    }
}