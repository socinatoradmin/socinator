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
    public class DownvoteQuestionsBase : ModuleSettingsUserControl<DownvoteQuestionsViewModel, DownvoteQuestionsModel>
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

    public sealed partial class DownvoteQuestions
    {
        public DownvoteQuestions()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownvoteQuestionsHeader,
                footer: DownvoteQuestionsFooter,
                queryControl: DownvoteQuestionsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.DownvoteQuestions,
                moduleName: QdMainModule.Voting.ToString()
            );


            VideoTutorialLink = ConstantHelpDetails.DownvoteQuestionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DownvoteQuestionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DownvoteQuestions CurrentDownvoteQuestions { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static DownvoteQuestions GetSingeltonObjectDownvote()
        {
            return CurrentDownvoteQuestions ?? (CurrentDownvoteQuestions = new DownvoteQuestions());
        }

        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (DownvoteQuestionsSearchControl.CurrentQuery.QueryType == "Custom User")
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
        //            if (!string.IsNullOrEmpty(DownvoteQuestionsSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (DownvoteQuestionsSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    (Blacklistuser.Contains(DownvoteQuestionsSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", ""))
        //                    || Whitelistuser.Contains(DownvoteQuestionsSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", ""))))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(DownvoteQuestionsSearchControl.CurrentQuery.QueryValue)
        //                    || Whitelistuser.Contains(DownvoteQuestionsSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < DownvoteQuestionsSearchControl.QueryCollection.Count)
        //                {
        //                    if (DownvoteQuestionsSearchControl.QueryCollection[i].Contains("http") &&
        //                        (Blacklistuser.Contains(DownvoteQuestionsSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))
        //                        || Whitelistuser.Contains(DownvoteQuestionsSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))))
        //                        DownvoteQuestionsSearchControl.QueryCollection.Remove(DownvoteQuestionsSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(DownvoteQuestionsSearchControl.QueryCollection[i])
        //                        || Whitelistuser.Contains(DownvoteQuestionsSearchControl.QueryCollection[i]))
        //                        DownvoteQuestionsSearchControl.QueryCollection.Remove(DownvoteQuestionsSearchControl.QueryCollection[i]);
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