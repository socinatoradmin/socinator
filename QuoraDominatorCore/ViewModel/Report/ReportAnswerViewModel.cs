using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Report
{
    public class ReportAnswerViewModel : BindableBase
    {
        private ReportAnswerModel _reportAnswerModel = new ReportAnswerModel();

        public ReportAnswerViewModel()
        {
            try
            {
                ReportAnswerModel.JobConfiguration = new JobConfiguration
                {
                    ActivitiesPerJobDisplayName = Application.Current
                        .FindResource("LangKeyNumberOfAnswersToReportPerJob")?.ToString(),
                    ActivitiesPerHourDisplayName = Application.Current
                        .FindResource("LangKeyNumberOfAnswersToReportPerHour")?.ToString(),
                    ActivitiesPerDayDisplayName = Application.Current
                        .FindResource("LangKeyNumberOfAnswersToReportPerDay")?.ToString(),
                    ActivitiesPerWeekDisplayName = Application.Current
                        .FindResource("LangKeyNumberOfAnswersToReportPerWeek")?.ToString(),
                    IncreaseActivityDisplayName =
                        Application.Current.FindResource("LangKeyMaxAnswersToReportPerDay")?.ToString(),
                    RunningTime = RunningTimes.DayWiseRunningTimes,
                    Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
                };
                ReportAnswerModel.ListQueryType.Clear();
                ReportAnswerModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Visible;
                Enum.GetValues(typeof(AnswerQueryParameters)).Cast<AnswerQueryParameters>().ToList().ForEach(query =>
                {
                    ReportAnswerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        ?.ToString());
                });
                ReportAnswerModel.ReportOptions.Clear();
                ReportAnswerModel.ReportSubOptions.Clear();
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Spam", Description = "Selling illegal goods, money scams etc.", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Hate Speech", Description = "Serious attack on a group", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Harassment and bullying", Description = "Harassing or threatening an individual", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Harmful activities", Description = "Glorifying violence including self-harm or intent to seriously harm others", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Adult content (Consensual)", Description = "Nudity/Sexual content", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Sexual exploitation and abuse (child safety)", Description = "Sexually explicit or suggestive imagery or writing involving minors", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Sexual exploitation and abuse (adults and animals)", Description = "Sexually explicit or suggestive imagery or writing involving non-consenting adults or non-humans", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Plagiarism", Description = "Reusing content without attribution (link and blockquotes)", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Poorly written", Description = "Not in English or has very bad formatting, grammar, and spelling", HasDescription = true });
                ReportAnswerModel.ReportOptions.Add(new ReportOptionsModel { Title = "Inappropriate credential", Description = "Author's credential is offensive, spam, or impersonation", HasDescription = true });
                //SubOption
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Spam", Description = "Selling illegal goods, money scams etc.", HasDescription = true });
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Hate Speech", Description = "Serious attack on a group", HasDescription = true });
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Harassment and bullying", Description = "Harassing an individual (including doxing)", HasDescription = true });
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Harmful activities", Description = "Threatening or glorifying violence or serious harm, including self-harm", HasDescription = true });
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Sexual exploitation and abuse (child safety)", Description = "Sexually explicit or suggestive imagery or writing involving minors", HasDescription = true });
                ReportAnswerModel.ReportSubOptions.Add(new ReportOptionsModel { Title = "Adult Content", Description = "Sexually explicit, violent, or otherwise inappropriate", HasDescription = true });
                ReportAnswerModel.SelectedOption = ReportAnswerModel.ReportOptions.FirstOrDefault();
                ReportAnswerModel.SelectedSubOption = ReportAnswerModel.ReportSubOptions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            SaveReports = new BaseCommand<object>(sender => true, SaveReportExecute);
        }
        public ReportAnswerModel ReportAnswerModel
        {
            get => _reportAnswerModel;
            set
            {
                if ((_reportAnswerModel == null) & (_reportAnswerModel == value))
                    return;
                SetProperty(ref _reportAnswerModel, value);
            }
        }

        public ReportAnswerModel Model => ReportAnswerModel;


        #region Command

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand SaveReports { get; set; }

        #endregion

        #region Methods
        private void SaveReportExecute(object obj)
        {
            try
            {
                if (Model.SavedQueries.Count > 0)
                {
                    var Description = Model.ReportDescription.ToString();
                    Model.SavedQueries.ForEach(query =>
                    {
                        Model.SelectedOption.ReportDescription = Description;
                        Model.SelectedOption.SubOption = new ReportSubOption { Title = ReportAnswerModel?.SelectedSubOption?.Title,Description = ReportAnswerModel?.SelectedSubOption?.Description,HaveSubOption = ReportAnswerModel.EnableSubOption };
                        query.CustomFilters = JsonConvert.SerializeObject(Model.SelectedOption);
                    });
                    ToasterNotification.ShowInfomation("Reports Saved Successfully");
                    Model.ReportDescription = string.Empty;
                }
                else
                    ToasterNotification.ShowInfomation("Please Add Atleast One Query");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ReportAnswerViewModel, ReportAnswerModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ReportAnswerViewModel, ReportAnswerModel>;

                moduleSettingsUserControl?.AddQuery(typeof(AnswerQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}