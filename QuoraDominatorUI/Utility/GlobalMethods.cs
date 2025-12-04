using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using QuoraDominatorUI.CustomControl;

namespace QuoraDominatorUI.Utility
{
    public class GlobalMethods
    {
        internal static void ShowAnswerFilterControl(SearchQueryControl queryControl)
        {
            var answerFiltersControl = new AnswerFiltersControl();
            var objDialog = new Dialog();
            answerFiltersControl.AnswerFilter.SaveCloseButtonVisible = true;
            var filterWindow = objDialog.GetMetroWindow(answerFiltersControl, "Filter");

            answerFiltersControl.SaveButton.Click += (senders, Events) =>
            {
                queryControl.CurrentQuery.CustomFilters =
                    JsonConvert.SerializeObject(answerFiltersControl.AnswerFilter);
                filterWindow.Close();
            };

            filterWindow.ShowDialog();
        }

        internal static void ShowQuestionFilterControl(SearchQueryControl queryControl)
        {
            var questionFiltersControl = new QuestionFiltersControl();
            var objDialog = new Dialog();
            questionFiltersControl.QuestionFilter.SaveCloseButtonVisible = true;
            var filterWindow = objDialog.GetMetroWindow(questionFiltersControl, "Filter");

            questionFiltersControl.SaveButton.Click += (senders, events) =>
            {
                queryControl.CurrentQuery.CustomFilters =
                    JsonConvert.SerializeObject(questionFiltersControl.QuestionFilter);
                filterWindow.Close();
            };

            filterWindow.ShowDialog();
        }

        internal static void ShowUserFilterControl(SearchQueryControl queryControl)
        {
            var objUserFiltersControl = new UserFiltersControl();
            var objDialog = new Dialog();
            objUserFiltersControl.UserFilter.SaveCloseButtonVisible = true;
            var filterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

            objUserFiltersControl.SaveButton.Click += (senders, events) =>
            {
                queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(objUserFiltersControl.UserFilter);
                filterWindow.Close();
            };

            filterWindow.ShowDialog();
        }
    }
}