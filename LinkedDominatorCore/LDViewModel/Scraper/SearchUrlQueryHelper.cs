using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorCore.LDViewModel.Scraper
{
    public class SearchUrlQueryHelper
    {
        public void AddQuery(SearchQueryControl queryControl, string campaignName, ActivityType activityType,
            ObservableCollection<QueryInfo> savedQueries)
        {
            try
            {
                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue.Trim()) &&
                    queryControl.QueryCollection.Count == 0)
                    return;

                // here we using '<>' as a delimiter to split urls, since some url may contains ','
                if (queryControl.CurrentQuery.QueryValue.Contains("<>"))
                {
                    queryControl.QueryCollection.Clear();
                    queryControl.QueryCollection.AddRange(Regex
                        .Split(queryControl.CurrentQuery.QueryValue, "<>").Where(x => !string.IsNullOrEmpty(x.Trim()))
                        .Select(x => x.Trim()).Distinct());
                    queryControl.CurrentQuery.QueryValue = string.Empty;
                }

                var queryValueIndex = new List<int>();
                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue) &&
                    queryControl.QueryCollection.Count != 0)
                {
                    queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null)
                            return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                       
                        currentQuery.QueryPriority = savedQueries.Count + 1;

                        savedQueries.Add(currentQuery);
                        currentQuery.Index = savedQueries.IndexOf(currentQuery) + 1;
                    });

                    if (queryValueIndex.Count > 0)
                    {
                        if (queryValueIndex.Count <= 10)
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork,
                                campaignName, activityType,
                                "{ " + string.Join(" },{ ", queryValueIndex.ToArray()) + " }");
                        else
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount,
                                SocinatorInitialize.ActiveSocialNetwork, campaignName, activityType,
                                queryValueIndex.Count);
                    }
                }
                else
                {
                    queryControl.CurrentQuery.QueryTypeDisplayName = queryControl.CurrentQuery.QueryType;

                    var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = currentQuery.QueryValue.Trim();

                    currentQuery.QueryPriority = savedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, savedQueries)) return;

                    savedQueries.Add(currentQuery);
                    currentQuery.Index = savedQueries.IndexOf(currentQuery) + 1;
                    queryControl.CurrentQuery.QueryValue = string.Empty;
                }

                queryControl.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsQueryExist(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                    x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Alert",
                        "Query already Exist !!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
    }
}