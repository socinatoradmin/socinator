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

namespace QuoraDominatorUI.QDViews.Activity.ReportAnswer
{
    public class ReportAnswerConfigurationbase : ModuleSettingsUserControl<ReportAnswerViewModel, ReportAnswerModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ReportAnswerModel =
                        JsonConvert.DeserializeObject<ReportAnswerModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ReportAnswerViewModel();


                ObjViewModel.ReportAnswerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReportAnswerConfiguration.xaml
    /// </summary>
    public partial class ReportAnswerConfiguration
    {
        public ReportAnswerConfiguration()
        {
            InitializeComponent();

            InitializeBaseClass(
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReportAnswersSearchControl,
                activityType: ActivityType.ReportAnswers,
                moduleName: QdMainModule.Report.ToString(),
                MainGrid: MainGrid
            );

            VideoTutorialLink = ConstantHelpDetails.ReportAnswersVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static ReportAnswerConfiguration CurrentReportAnswerConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF ReportAnswerConfiguration
        /// </summary>
        /// <returns></returns>
        public static ReportAnswerConfiguration GetSingeltonObjectReportAnswerConfiguration()
        {
            return CurrentReportAnswerConfiguration ??
                   (CurrentReportAnswerConfiguration = new ReportAnswerConfiguration());
        }
    }
}