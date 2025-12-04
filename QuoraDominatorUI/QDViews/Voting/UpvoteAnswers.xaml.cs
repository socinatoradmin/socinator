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
    public class UpvoteAnswersBase : ModuleSettingsUserControl<UpvoteAnswersViewModel, UpvoteAnswersModel>
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
    ///     Interaction logic for Upvote.xaml
    /// </summary>
    public partial class UpvoteAnswers
    {
        public UpvoteAnswers()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UpvoteAnswersHeader,
                footer: UpvoteAnswersFooter,
                queryControl: UpvoteAnswersSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UpvoteAnswers,
                moduleName: QdMainModule.Voting.ToString()
            );


            VideoTutorialLink = ConstantHelpDetails.UpvoteAnswersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UpvoteAnswersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UpvoteAnswers CurrentUpvoteAnswers { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUpvoteAnswers is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UpvoteAnswers GetSingeltonObjectUpvoteAnswers()
        {
            return CurrentUpvoteAnswers ?? (CurrentUpvoteAnswers = new UpvoteAnswers());
        }


        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (UpvoteAnswersSearchControl.CurrentQuery.QueryType == "Custom User")
        //    {
        //        try
        //        {
        //            List<string> Blacklistuser = null;
        //            IGlobalDatabaseConnection globalDbConnection = SocinatorInitialize.GetGlobalDatabase();
        //            var dbContext = globalDbConnection.GetDbContext();
        //            var dbOperation = new DbOperations(dbContext);
        //            try
        //            {
        //                Blacklistuser = dbOperation.Get<BlackListUser>().Select(x => x.UserName).ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //            if (!string.IsNullOrEmpty(UpvoteAnswersSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (UpvoteAnswersSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    Blacklistuser.Contains(UpvoteAnswersSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", "")))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(UpvoteAnswersSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < UpvoteAnswersSearchControl.QueryCollection.Count)
        //                {
        //                    if (UpvoteAnswersSearchControl.QueryCollection[i].Contains("http") &&
        //                        Blacklistuser.Contains(UpvoteAnswersSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", "")))
        //                        UpvoteAnswersSearchControl.QueryCollection.Remove(UpvoteAnswersSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(UpvoteAnswersSearchControl.QueryCollection[i]))
        //                        UpvoteAnswersSearchControl.QueryCollection.Remove(UpvoteAnswersSearchControl.QueryCollection[i]);
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