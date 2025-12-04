using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Voting;

namespace QuoraDominatorUI.QDViews.Voting
{
    public class DownvoteAnswersBase : ModuleSettingsUserControl<DownvoteAnswersViewModel, DownvoteAnswersModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for DownvoteAnswers.xaml
    /// </summary>
    public partial class DownvoteAnswers
    {
        public DownvoteAnswers()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownvoteAnswersHeader,
                footer: Footer,
                queryControl: DownvoteAnswersSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.DownvoteAnswers,
                moduleName: QdMainModule.Voting.ToString()
            );


            VideoTutorialLink = ConstantHelpDetails.DownvoteAnswersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DownvoteAnswersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DownvoteAnswers CurrentDownvoteAnswers { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static DownvoteAnswers GetSingeltonObjectDownvoteAnswers()
        {
            return CurrentDownvoteAnswers ?? (CurrentDownvoteAnswers = new DownvoteAnswers());
        }

        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (DownvoteAnswersSearchControl.CurrentQuery.QueryType == "Custom User")
        //    {
        //        try
        //        {
        //            List<string> Blacklistuser = null;
        //            List<string> Whitelistuser = null;
        //            IGlobalDatabaseConnection globalDbConnection = SocinatorInitialize.GetGlobalDatabase();
        //            var dbContext = globalDbConnection.GetDbContext();
        //            var dbOperation = new DbOperations(dbContext);
        //            try
        //            {
        //                Blacklistuser = dbOperation.Get<BlackListUser>().Select(x => x.UserName).ToList();
        //                Whitelistuser = dbOperation.Get<WhiteListUser>().Select(x => x.UserName).ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //            if (!string.IsNullOrEmpty(DownvoteAnswersSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (DownvoteAnswersSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    (Blacklistuser.Contains(DownvoteAnswersSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", "")) 
        //                    || Whitelistuser.Contains(DownvoteAnswersSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", ""))))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(DownvoteAnswersSearchControl.CurrentQuery.QueryValue)
        //                    || Whitelistuser.Contains(DownvoteAnswersSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < DownvoteAnswersSearchControl.QueryCollection.Count)
        //                {
        //                    if (DownvoteAnswersSearchControl.QueryCollection[i].Contains("http") &&
        //                        (Blacklistuser.Contains(DownvoteAnswersSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))
        //                        || Whitelistuser.Contains(DownvoteAnswersSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))))
        //                        DownvoteAnswersSearchControl.QueryCollection.Remove(DownvoteAnswersSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(DownvoteAnswersSearchControl.QueryCollection[i])
        //                        || Whitelistuser.Contains(DownvoteAnswersSearchControl.QueryCollection[i]))
        //                        DownvoteAnswersSearchControl.QueryCollection.Remove(DownvoteAnswersSearchControl.QueryCollection[i]);
        //                    else
        //                        i++;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }
        //    base.SearchQueryControl_OnAddQuery(sender, e, typeof(FollowerQuery));
        //}
    }
}