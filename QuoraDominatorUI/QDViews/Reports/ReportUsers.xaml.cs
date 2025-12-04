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
    public class ReportUsersbase : ModuleSettingsUserControl<ReportUserViewModel, ReportUserModel>
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
    ///     Interaction logic for ReportUsers.xaml
    /// </summary>
    public sealed partial class ReportUsers
    {
        private static ReportUsers _currentReportUsers;

        public ReportUsers()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ReportUsersHeader,
                footer: ReportUsersFooter,
                queryControl: ReportUsersSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ReportUsers,
                moduleName: QdMainModule.Report.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.ReportUsersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ReportUsersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static ReportUsers GetSingeltonObjectReportUsers()
        {
            return _currentReportUsers ?? (_currentReportUsers = new ReportUsers());
        }
    }
}