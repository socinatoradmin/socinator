using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Scrape;

namespace QuoraDominatorUI.QDViews.Scrape
{
    public class QuestionsScraperBase : ModuleSettingsUserControl<QuestionScraperViewModel, QuestionsScraperModel>
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

    public partial class QuestionsScraper
    {
        public QuestionsScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: QuestionsScraperHeader,
                footer: QuestionsScraperFooter,
                queryControl: QuestionsScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.QuestionsScraper,
                moduleName: QdMainModule.Voting.ToString()
            );


            VideoTutorialLink = ConstantHelpDetails.QuestionsScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.QuestionsScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static QuestionsScraper CurrentQuestionsScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static QuestionsScraper GetSingeltonObjectQuestionsScraper()
        {
            return CurrentQuestionsScraper ?? (CurrentQuestionsScraper = new QuestionsScraper());
        }

        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (QuestionsScraperSearchControl.CurrentQuery.QueryType == "Custom User")
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
        //            if (!string.IsNullOrEmpty(QuestionsScraperSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (QuestionsScraperSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    Blacklistuser.Contains(QuestionsScraperSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", "")))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(QuestionsScraperSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < QuestionsScraperSearchControl.QueryCollection.Count)
        //                {
        //                    if (QuestionsScraperSearchControl.QueryCollection[i].Contains("http") &&
        //                        Blacklistuser.Contains(QuestionsScraperSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", "")))
        //                        QuestionsScraperSearchControl.QueryCollection.Remove(QuestionsScraperSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(QuestionsScraperSearchControl.QueryCollection[i]))
        //                        QuestionsScraperSearchControl.QueryCollection.Remove(QuestionsScraperSearchControl.QueryCollection[i]);
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