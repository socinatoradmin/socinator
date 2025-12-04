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
    public class AnswersScraperBase : ModuleSettingsUserControl<AnswerScraperViewModel, AnswersScraperModel>
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
    ///     Interaction logic for AnswersScraper.xaml
    /// </summary>
    public partial class AnswersScraper
    {
        private static AnswersScraper _currentAnswersScraper;

        public AnswersScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AnswersScraperHeader,
                footer: AnswersScraperFooter,
                queryControl: AnswersScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.AnswersScraper,
                moduleName: QdMainModule.Scrape.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AnswersScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AnswersScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static AnswersScraper GetSingeltonObjectAnswersScraper()
        {
            return _currentAnswersScraper ?? (_currentAnswersScraper = new AnswersScraper());
        }

        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (AnswersScraperSearchControl.CurrentQuery.QueryType == "Custom User")
        //    {
        //        try
        //        {
        //            List<string> Blacklistuser = null;
        //            //IGlobalDatabaseConnection globalDbConnection = SocinatorInitialize.GetGlobalDatabase();
        //            //var dbContext = globalDbConnection.GetDbContext();
        //            //var dbOperation = new DbOperations(dbContext);


        //            var _dbGlobalService = new DbGlobalService();
        //            try
        //            {
        //                //Blacklistuser = dbOperation.Get<BlackListUser>().Select(x => x.UserName).ToList();
        //                Blacklistuser = _dbGlobalService.GetBlackListUser();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //            if (!string.IsNullOrEmpty(AnswersScraperSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (AnswersScraperSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    Blacklistuser.Contains(AnswersScraperSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", "")))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(AnswersScraperSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < AnswersScraperSearchControl.QueryCollection.Count)
        //                {
        //                    if (AnswersScraperSearchControl.QueryCollection[i].Contains("http") &&
        //                        Blacklistuser.Contains(AnswersScraperSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", "")))
        //                        AnswersScraperSearchControl.QueryCollection.Remove(AnswersScraperSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(AnswersScraperSearchControl.QueryCollection[i]))
        //                        AnswersScraperSearchControl.QueryCollection.Remove(AnswersScraperSearchControl.QueryCollection[i]);
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