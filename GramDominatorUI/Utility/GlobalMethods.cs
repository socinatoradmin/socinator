using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorUI.CustomControl;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility
{
    public class GlobalMethods
    {
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