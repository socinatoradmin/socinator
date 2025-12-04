using DominatorHouseCore;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;

namespace FaceDominatorCore.Utility
{
    public class FdUtility
    {
        public string GetClassPropertyValueForTests(object obj, string startingObjStringName = "sut",
            string additionalAdd = "")
        {
            try
            {
                startingObjStringName = startingObjStringName +
                                        (!string.IsNullOrEmpty(additionalAdd) ? $".{additionalAdd}" : "");
                var sb = new StringBuilder();
                GoInsideListMakeString(ref sb, obj, startingObjStringName);
                var gotString = sb.ToString();
                return gotString;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return "";
        }

        public void GoInsideListMakeString(ref StringBuilder sb, object obj, string startingObjName)
        {
            var listProperties = obj.GetType().GetProperties();

            foreach (var element in listProperties)
            {
                var startingObjStringName = startingObjName;
                try
                {
                    var elemName = element.PropertyType.Name;
                    if (elemName == "int" || elemName == "Boolean")
                    {
                        var value = elemName == "Boolean"
                            ? element.GetValue(obj, null).ToString().ToLower()
                            : element.GetValue(obj, null).ToString();
                        if (value != null)
                            sb.AppendLine(GetString(element.Name, value, startingObjStringName, true));
                    }
                    else if (elemName == "String")
                    {
                        var value = element.GetValue(obj, null);
                        if (value != null)
                            sb.AppendLine(GetString(element.Name, value.ToString(), startingObjStringName));
                    }
                    else if (elemName.Contains("List"))
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}";
                        if (value != null)
                            sb.AppendLine(GetStringFromListObj(value, startingObjStringName, 1));
                    }
                    else
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}"; //value.GetType().Name
                        if (value != null)
                            GoInsideListMakeString(ref sb, value, startingObjStringName);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public string GetStringFromListObj(object obj, string startingObjStringName, int numberOfDataFromList = 0)
        {
            try
            {
                var sb = new StringBuilder();
                var value = (IList)obj;
                var iteration = 0;
                if (numberOfDataFromList == 0)
                    numberOfDataFromList = value.Count;

                foreach (var each in value)
                {
                    GoInsideListMakeString(ref sb, each, $"{startingObjStringName}[{iteration++}]");
                    if (iteration >= numberOfDataFromList) break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetString(string name, string value, string startingObjStringName, bool isInt = false)
        {
            value = isInt ? value : $"\"{value}\"";
            return $"\n{startingObjStringName}.{name}.Should().Be({value});";
        }

    }
    public static class StaticUtility
    {
        public static T GetActivityModelNonQueryList<T>(this string activitySettings, dynamic lastModel)
        {
            dynamic getModel = JsonConvert.DeserializeObject<T>(activitySettings);

            if ("LangKeySocinator".FromResourceDictionary() == "Tunto Socianator")
            {
                try
                {
                    Utilities.CopyJobConfigWith(getModel.JobConfiguration, lastModel.JobConfiguration);

                    var listOldQuery = getModel.ListKeywordsNonQuery;
                    getModel.ListKeywordsNonQuery = lastModel.ListKeywordsNonQuery;

                    Utilities.ModifySavedQueries(getModel.SavedQueries, getModel.ListKeywordsNonQuery, listOldQuery);
                }
                catch (Exception ex)
                { ex.DebugLog(); }

            }
            return getModel;
        }

    }
}
