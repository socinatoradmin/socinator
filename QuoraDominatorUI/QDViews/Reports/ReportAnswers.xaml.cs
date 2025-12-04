using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Report;

namespace QuoraDominatorUI.QDViews.Reports
{
    public class ReportAnswersBase : ModuleSettingsUserControl<ReportAnswerViewModel, ReportAnswerModel>
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
    ///     Interaction logic for ReportAnswers.xaml
    /// </summary>
    public partial class ReportAnswers
    {
        private static ReportAnswers _currentReportAnswers;

        public ReportAnswers()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ReportAnswersHeader,
                footer: ReportAnswersFooter,
                queryControl: ReportAnswersSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ReportAnswers,
                moduleName: QdMainModule.Report.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ReportAnswersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ReportAnswersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static ReportAnswers GetSingeltonObjectReportAnswers()
        {
            return _currentReportAnswers ?? (_currentReportAnswers = new ReportAnswers());
        }

        //void AddQuery(object sender, RoutedEventArgs e)
        //{
        //    if (ReportAnswersSearchControl.CurrentQuery.QueryType == "Custom User")
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
        //            if (!string.IsNullOrEmpty(ReportAnswersSearchControl.CurrentQuery.QueryValue))
        //            {
        //                if (ReportAnswersSearchControl.CurrentQuery.QueryValue.Contains("http") &&
        //                    (Blacklistuser.Contains(ReportAnswersSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", ""))
        //                    || Whitelistuser.Contains(ReportAnswersSearchControl.CurrentQuery.QueryValue.Replace("https://www.quora.com/profile/", ""))))
        //                    return;
        //                else
        //                    if (Blacklistuser.Contains(ReportAnswersSearchControl.CurrentQuery.QueryValue)
        //                    || Whitelistuser.Contains(ReportAnswersSearchControl.CurrentQuery.QueryValue))
        //                    return;
        //            }
        //            else
        //            {
        //                int i = 0;
        //                while (i < ReportAnswersSearchControl.QueryCollection.Count)
        //                {
        //                    if (ReportAnswersSearchControl.QueryCollection[i].Contains("http") &&
        //                        (Blacklistuser.Contains(ReportAnswersSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))
        //                        || Whitelistuser.Contains(ReportAnswersSearchControl.QueryCollection[i].Replace("https://www.quora.com/profile/", ""))))
        //                        ReportAnswersSearchControl.QueryCollection.Remove(ReportAnswersSearchControl.QueryCollection[i]);
        //                    else if (Blacklistuser.Contains(ReportAnswersSearchControl.QueryCollection[i])
        //                        || Whitelistuser.Contains(ReportAnswersSearchControl.QueryCollection[i]))
        //                        ReportAnswersSearchControl.QueryCollection.Remove(ReportAnswersSearchControl.QueryCollection[i]);
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