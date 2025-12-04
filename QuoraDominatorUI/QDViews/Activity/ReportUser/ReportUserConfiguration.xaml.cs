using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.Report;

namespace QuoraDominatorUI.QDViews.Activity.ReportUser
{
    public class ReportUserConfigurationbase : ModuleSettingsUserControl<ReportUserViewModel, ReportUserModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ReportUserModel =
                        JsonConvert.DeserializeObject<ReportUserModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ReportUserViewModel();


                ObjViewModel.ReportUserModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReportUserConfiguration.xaml
    /// </summary>
    public partial class ReportUserConfiguration
    {
        public ReportUserConfiguration()
        {
            InitializeComponent();

            InitializeBaseClass(
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReportUsersSearchControl,
                activityType: ActivityType.ReportUsers,
                moduleName: QdMainModule.Report.ToString(),
                MainGrid: MainGrid
            );

            VideoTutorialLink = ConstantHelpDetails.ReportUsersVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static ReportUserConfiguration CurrentReportUserConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF ReportUserConfiguration
        /// </summary>
        /// <returns></returns>
        public static ReportUserConfiguration GetSingeltonObjectReportUserConfiguration()
        {
            return CurrentReportUserConfiguration ?? (CurrentReportUserConfiguration = new ReportUserConfiguration());
        }
    }
}