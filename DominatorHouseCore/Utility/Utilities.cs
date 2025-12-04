#region

using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class Utilities
    {
        #if DEBUG
        public static bool DisableLicenseInDebug { get; set; } = true;
        #else
        public static bool DisableLicenseInDebug { get; set; } = false;
        #endif
        /// <summary>
        ///     GetMobileDeviceId is used to get the mobile device id with 16 Digits
        /// </summary>
        /// <returns>return the 16 digit unique mobile device ID</returns>
        public static string GetMobileDeviceId(string Guid = "")
        {
            // Collect the random inputString with five character, convert those character to byte array with help of the MD5
            return "android-" + (string.IsNullOrEmpty(Guid)
                       ? RandomUtilties.GetRandomString(5).GetHexFromString().Substring(0, 16)
                       : Guid);
        }

        public static List<SocialNetworks> VisibleNetworks = new List<SocialNetworks>() { SocialNetworks.Instagram, SocialNetworks.TikTok };
        public static bool GetHeadLessStatus(LoginType loginType = LoginType.AutomationLogin, SocialNetworks network = SocialNetworks.Social)
        {
            var HeadLess = true;
            if (loginType == LoginType.BrowserLogin || VisibleNetworks.Contains(network))
            {
                HeadLess = false;
                return HeadLess;
            }
#if DEBUG
            HeadLess = false;
#endif
            return HeadLess;
        }

        public static string ExtractBetweenRegex(string input, string startPattern, string endPattern)
        {
            string pattern = $"{startPattern}(.*?){endPattern}";
            var match = Regex.Match(input, pattern, RegexOptions.Singleline);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }
            return "";
        }
        /// <summary>
        ///     GetHexFromString is used to get the hexadecimal value of the given input inputString
        /// </summary>
        /// <param name="inputString">inputString which is convert into hexadecimal</param>
        /// <returns>Required Hexa decimal value from the inputString</returns>
        public static string GetHexFromString(this string inputString)
        {
            //Validate the input whether is null or not 
            if (inputString == null)
                throw new ArgumentNullException(nameof(inputString));

            // Convert the input values to hexa
            using (var md5 = MD5.Create())
            {
                // Read the bytes values form the input string
                var bytes = Encoding.UTF8.GetBytes(inputString);

                //Compute the hash values of bytes with the help of MD5 then convert those to base datatype(here string),
                //finally convert the string to lower
                return BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
            }
        }


        /// <summary>
        ///     GetGuid is used to get the GUID values
        /// </summary>
        /// <param name="isDashesNeed">This parameter is used to decide whether keep the dashes in the GUID or not</param>
        /// <returns>Return the GUID</returns>
        public static string GetGuid(bool isDashesNeed = true)
        {
            // Generate the GUID 
            var getGuid = Guid.NewGuid().ToString();
            // return the GUID without dashes if isDashesNeed is true 
            return !isDashesNeed ? getGuid.Replace('-', char.MinValue) : getGuid;
        }

        /// <summary>
        ///     Get the text from source string Between two pattern of characters
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            try
            {
                if  (string.IsNullOrEmpty(strSource)||!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return string.Empty;
                var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);

                if (end < 0 || start < 0)
                    return string.Empty;

                return strSource.Substring(start, end - start);

            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        ///     Calculates percentage
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static int PercentageCalculator(int value, int percentage)
        {
            return value * percentage / 100;
        }

        // Returns string from resource dictionary
        public static string FromResourceDictionary(this string resourceDictionaryKey)
        {
            try
            {
                var lang = Application.Current?.FindResource(resourceDictionaryKey)?.ToString() ??
                           resourceDictionaryKey;
                return lang;
            }
            catch (Exception)
            {
                var keySubstring = resourceDictionaryKey.Substring("LangKey".Length);
                return Regex.Replace(keySubstring, "(\\B[A-Z])", " $1");
            }
        }

        /// <summary>
        ///     Get Report Header as string
        /// </summary>
        /// <param name="resourceDictionaryKeys">List of resourceDictionary key. It should be sequential.</param>
        /// <returns></returns>
        public static string ReportHeaderFromResourceDict(this List<string> resourceDictionaryKeys)
        {
            var header = "";
            foreach (var each in resourceDictionaryKeys)
            {
                string lang;
                try
                {
                    lang = Application.Current?.FindResource(each)?.ToString() ?? each;
                }
                catch (Exception)
                {
                    lang = each.Substring("LangKey".Length);
                    lang = Regex.Replace(lang, "(\\B[A-Z])", " $1");
                }

                header += $"{lang},";
            }

            return header.TrimEnd(',');
        }


        public static void ExportReports(string fileName, string csvHeader, List<string> csvData)
        {
            using (var streamWriter = new StreamWriter(fileName, true, Encoding.UTF8))
            {
                streamWriter.WriteLine(csvHeader);
            }

            try
            {
                Task.Factory.StartNew(async () =>
                {
                    ToasterNotification.ShowWarning("LangKeyExportingStarted".FromResourceDictionary());
                    foreach (var item in csvData)
                        using (var streamWriter = new StreamWriter(fileName, true, Encoding.UTF8))
                        {
                            await streamWriter.WriteLineAsync(item);
                        }

                    ToasterNotification.ShowSuccess(
                        $"{"LangKeySucessfullyExportedTo".FromResourceDictionary()} {fileName} ");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            //GlobusLogHelper.log.Info("Data has been exported successfully");
        }


        public static string GetUrlFormPostData(object obj)
        {
            var urlFormData = string.Empty;
            var serializedPostData = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedPostData);

            foreach (var dictKey in dict.Keys)
                urlFormData += (urlFormData == string.Empty ? string.Empty : "&") + dictKey + "=" + dict[dictKey];
            return urlFormData;
        }

        /// <summary>
        ///     Remove the url from given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveUrls(string text)
        {
            return Regex.Replace(text, @"\b(?:https?://|www\.)\S+\b", string.Empty).Trim();
        }

        /// <summary>
        ///     Replace the url with their shorten url
        /// </summary>
        /// <param name="text">text</param>
        /// <returns></returns>
        public static string ReplaceWithShortenUrl(string text)
        {
            // pattern
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // call to replace evaluator
            return linkParser.Replace(text, ReplaceMatchEvaluator);
        }

        /// <summary>
        ///     Apply the shorten url for all matches
        /// </summary>
        /// <param name="match"><see cref="Match" /> match objects</param>
        /// <returns></returns>
        public static string ReplaceMatchEvaluator(Match match)
        {
            return Shorten(match.Value);
        }

        /// <summary>
        ///     Get a shorten url for give url
        /// </summary>
        /// <param name="longUrl"></param>
        /// <returns></returns>
        public static string Shorten(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
                return longUrl;

            // get the Bitly login and apikey value
            var login = ConstantVariable.BitlyLogin;
            var apikey = ConstantVariable.BitlyApiKey;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(apikey))
                return longUrl;

            // base url
            var url =
                $"http://api.bit.ly/shorten?format=json&version=2.0.1&longUrl={HttpUtility.UrlEncode(longUrl)}&login={login}&apiKey={apikey}";

            // make a get requests
            var request = (HttpWebRequest) WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        // Hitting the bitly url and fetching the shorten url for given long url
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        dynamic jsonResponse = JsonConvert.DeserializeObject(reader.ReadToEnd());
                        // Fetch the short url from api
                        string shortUrl = jsonResponse["results"][longUrl]["shortUrl"];
                        return shortUrl;
                    }
                }
            }
            catch (WebException)
            {
                return longUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return longUrl;
        }


        /// <summary>
        ///     Extract integer only value from string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetIntegerOnlyString(string data)
        {
            return data.Contains("null") ? "0" : Regex.Replace(data, "[^0-9]+", string.Empty);
        }


        public static string FirstMatchExtractor(string decodedResponse, string pattern)
        {
            var match = Regex.Matches(decodedResponse, pattern, RegexOptions.Singleline);
            return match.Count > 0 ? match[0].Groups[1].ToString() : string.Empty;
        }

        public static bool DownloadNotFound()
        {

            try
            {
                var webclient = new WebClient();
                webclient.DownloadFile("https://cdn.browshot.com/static/images/not-found.png",
                     $"{ConstantVariable.GetNotFoundImage()}");
            }
            catch (Exception)
            {
                DownloadImageWithBypassedSSL("https://cdn.browshot.com/static/images/not-found.png",
                     $"{ConstantVariable.GetNotFoundImage()}");
            }
            return true;
        }

        public static void DownloadImageWithBypassedSSL(string url, string destinationPath)
        {
            try
            {
                // Create an HttpWebRequest for the specific URL
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Bypass SSL validation for this request
                request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                // Execute the request and get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        using (FileStream fileStream = File.Create(destinationPath))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static bool DownloadSocinatorIcon()
        {
            var webclient = new WebClient();
            webclient.DownloadFile("https://socinator.com/wp-content/uploads/2018/07/fav_64.png",
                $"{ConstantVariable.GetSocinatorIcon()}");
            return true;
        }

        public static string ReplaceUniCode(string messeges)
        {
            messeges = HttpUtility.HtmlDecode(messeges);
            var _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
            messeges = _regex.Replace(messeges,
                m => ((char) int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
            return messeges;
        }

        public static bool AppClosing;
        public static readonly object LockOpeningBrowser = new object();

        public static List<Tuple<int, DateTime, DateTime>> RunningWebDrivers =
            new List<Tuple<int, DateTime, DateTime>>();

        public static void KillGecko()
        {
            try
            {
                AppClosing = true;

                lock (LockOpeningBrowser)
                {
                    if (RunningWebDrivers.Count == 0) return;
                    var listOfFirefox = System.Diagnostics.Process.GetProcessesByName("firefox").ToList();
                    var localListDrivers = new List<Tuple<int, DateTime, DateTime>>(RunningWebDrivers);
                    foreach (var geckoProcessId in localListDrivers)
                    {
                        try
                        {
                            System.Diagnostics.Process.GetProcessById(geckoProcessId.Item1).Kill();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (listOfFirefox.Count == 0) return;
                        var processFirefox = listOfFirefox.Where(x => !x.HasExited &&
                                                                      x.StartTime >= geckoProcessId.Item2 &&
                                                                      x.StartTime < geckoProcessId.Item3);
                        try
                        {
                            foreach (var each in processFirefox)
                                if (!each.HasExited)
                                    each.Kill();
                        }
                        catch (Exception)
                        {
                            /*Ignore*/
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static T DeepCloneObject<T>(this T instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance));
        }

        public static void CopyJobConfigWith(this JobConfiguration jobConfig, JobConfiguration oldJobConfig)
        {
            jobConfig.ActivitiesPerJobDisplayName = oldJobConfig.ActivitiesPerJobDisplayName;
            jobConfig.ActivitiesPerHourDisplayName = oldJobConfig.ActivitiesPerHourDisplayName;
            jobConfig.ActivitiesPerDayDisplayName = oldJobConfig.ActivitiesPerDayDisplayName;
            jobConfig.ActivitiesPerWeekDisplayName = oldJobConfig.ActivitiesPerWeekDisplayName;
            jobConfig.IncreaseActivityDisplayName = oldJobConfig.IncreaseActivityDisplayName;
        }

        public static void ModifySavedQueries(this ObservableCollection<QueryInfo> savedQuery,
            List<string> listQueryTypes, List<string> oldlistQueryTypes)
        {
            savedQuery.ForEach(x =>
            {
                var queryNameIndex = oldlistQueryTypes.IndexOf(x.QueryType);
                x.QueryType = listQueryTypes[queryNameIndex];
                x.QueryTypeDisplayName = x.QueryType;
            });
        }

        public static T GetActivityModel<T>(this string activitySettings, dynamic lastModel, bool isNonQuery = false)
        {
            dynamic getModel = JsonConvert.DeserializeObject<T>(activitySettings);

            if ("LangKeySocinator".FromResourceDictionary() != "Tunto Socianator") return getModel;
            try
            {
                CopyJobConfigWith(getModel.JobConfiguration, lastModel.JobConfiguration);

                if (!isNonQuery)
                {
                    var listOldQuery = getModel.ListQueryType;
                    getModel.ListQueryType = lastModel.ListQueryType;

                    ModifySavedQueries(getModel.SavedQueries, getModel.ListQueryType, listOldQuery);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return getModel;
        }

        public static string AsCsvData(this string data /*, bool isLast = false*/)
        {
            return $"\"{data?.Replace("\"", "\"\"")}\"" /* + (!isLast ? "," : "")*/;
        }

        public static void UpdateTestResponseDataFile(string hitResponse, string respDataFileLoc,
            SoftwareSettingsModel softwareSettings = null)
        {
            try
            {
                if (softwareSettings == null)
                {
                    var softwareSettingsFileManager =
                        InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
                    softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
                }

                if (softwareSettings.IsTestMode)
                    FileUtilities.ReWriteDataIntoFile(hitResponse, respDataFileLoc);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void UpdateResponseHandlerTest(string hitResponse, object hitResponseHandler,
            string respDataFileLoc, string respHandTestFileLoc, SoftwareSettingsModel softwareSettings = null)
        {
            try
            {
                if (softwareSettings == null)
                {
                    var softwareSettingsFileManager =
                        InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
                    softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
                }

                if (!softwareSettings.IsTestMode) return;
                // Update data in TestResponseFile(json, html etc.)
                FileUtilities.ReWriteDataIntoFile(hitResponse, respDataFileLoc);

                // get text content from the ResponseHandler class file to update the content in it 
                var classData = FileUtilities.ReadFile(respHandTestFileLoc).Result;

                // New checking data
                var dateToReplace = GetClassPropertyValueForTests(hitResponseHandler);

                // old checking data
                var oldData = GetBetween(classData, "#region DataChecking", "#endregion");

                // renew the class content
                classData = classData.Replace(oldData, $"\n{dateToReplace}\n");

                // updating the class content
                FileUtilities.ReWriteDataIntoFile(classData, respHandTestFileLoc);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static string GetClassPropertyValueForTests(object obj, string startingObjStringName = "sut",
            string additionalAdd = "")
        {
            try
            {
                startingObjStringName += (!string.IsNullOrEmpty(additionalAdd) ? $".{additionalAdd}" : "");
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

        public static void GoInsideListMakeString(ref StringBuilder sb, object obj, string startingObjName)
        {
            var listProperties = obj.GetType().GetProperties();

            foreach (var element in listProperties)
            {
                var startingObjStringName = startingObjName;
                try
                {
                    var elemName = element.PropertyType.Name;
                    switch (elemName)
                    {
                        case "int":
                        case "Boolean":
                        {
                            var value = elemName == "Boolean"
                                ? element.GetValue(obj, null).ToString().ToLower()
                                : element.GetValue(obj, null).ToString();
                            if (value != null)
                                sb.AppendLine(GetString(element.Name, value, startingObjStringName, true));
                            break;
                        }

                        case "String":
                        {
                            var value = element.GetValue(obj, null);
                            if (value != null)
                            {
                                var stringValue = value.ToString();
                                if (stringValue.Contains("\""))
                                    stringValue = stringValue.Replace("\"", "\\\"");
                                if (stringValue.Contains("\n"))
                                    stringValue = stringValue.Replace("\n", "\\n");
                                if (stringValue.Contains("\r\n"))
                                    stringValue = stringValue.Replace("\r\n", "\\r\\n");

                                sb.AppendLine(GetString(element.Name, stringValue, startingObjStringName));
                            }

                            break;
                        }

                        default:
                        {
                            if (elemName.Contains("List"))
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

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public static string GetStringFromListObj(object obj, string startingObjStringName,
            int numberOfDataFromList = 0)
        {
            try
            {
                var sb = new StringBuilder();
                var value = (IList) obj;
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

        public static string GetString(string name, string value, string startingObjStringName, bool isInt = false)
        {
            value = isInt ? value : $"\"{value}\"";
            return $"\n{startingObjStringName}.{name}.Should().Be({value});";
        }

        public static string GetFileExtensonFromRemoteUrl(string RemoteUrl, CookieContainer cookieContainer = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(RemoteUrl);
                request.Method = "GET";
                request.CookieContainer = cookieContainer;
                var response = (HttpWebResponse)request.GetResponse();
                return response.ContentType?.Split('/')[1];
            }
            catch (Exception ex)
            {
                return $"Error ==> {ex.GetBaseException()}";
            }

        }
        public static string ValidateJsonString(string jsonData)
        {
            var json = jsonData.Replace("\t", "").Replace("\r\n", "");
            var loop = true;
            var Counter = 0;
            try
            {
                do
                {
                    try
                    {
                        var m = JObject.Parse(json);
                        loop = false;
                    }
                    catch (JsonReaderException ex)
                    {
                        var position = ex.LinePosition;
                        var invalidChar = json.Substring(position - 2, 2);
                        invalidChar = Regex.Replace(invalidChar, "[^\"\']", "");
                        invalidChar = invalidChar.Replace("\"", "\\\"")?.Replace("\'", "\\\'");
                        json = $"{json.Substring(0, position - 1)}{invalidChar}{json.Substring(position)}";
                    }
                } while (loop && Counter++ <= json.Length-1);
            }
            catch (Exception)
            {
            }
            return json;
        }
        public static double GetBrowserMemoryBufferSize()
        {
            try
            {
                var perfCounterService = InstanceProvider.GetInstance<IPerfCounterService>();
                var loadedMemory = perfCounterService?.LoadedMemoryDescrption?.Replace(" MB", "");
                double.TryParse(loadedMemory, out double memorySize);
                if(memorySize >= 16000)
                    return 14000;
                else
                    return 0;
            }catch (Exception)
            {
                return 0;
            }
        }

        public static SQLiteConnection Connection {  get; private set; }

        public static void ClearSession(DominatorAccountModel dominatorAccount)
        {
            try
            {
                var network = dominatorAccount.AccountBaseModel.AccountNetwork;
                var accountId = dominatorAccount.AccountId;
                var SessionFileName = $"{network}AccountSession.db";
                var dbfile = ConstantVariable.GetIndexAccountDir() + $"\\DB\\{SessionFileName}";
                if (!FileUtilities.FileExist(dbfile))
                    return;
                Connection = new SQLiteConnection(dbfile);
                var data = Get<AccountsSessionsTable>(x => x.AccountId == accountId);
                foreach(var d in data)
                {
                    if (d != null)
                        Remove(d);
                }
                
            }catch(Exception) { }
            finally { 
                Connection?.Close();
                Connection?.Dispose();
            }
        }
        public static bool Remove<T>(T t) where T : class
        {
            return Connection.Delete<T>(t) > 0;
        }
        public static List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null
                ? Connection.Table<T>().ToList()
                : Connection.Table<T>().Where(expression).ToList();
        }

        public static string SerializeObject<T>(this T classObject, JsonSerializerSettings settings = null, Formatting formatting = Formatting.None)
            => settings == null ? JsonConvert.SerializeObject(classObject) : JsonConvert.SerializeObject(classObject, formatting, settings);
        public static T DeserializeObject<T>(this string serializedString, JsonSerializerSettings settings = null)
            => settings == null ? JsonConvert.DeserializeObject<T>(serializedString) : JsonConvert.DeserializeObject<T>(serializedString, settings);
    }

    public static class JsonConverterUtil
    {
        public static string SerializeObject(object obj, JsonSerializerSettings settings = null, Formatting formatting = Formatting.None)
            => obj.SerializeObject(settings, formatting);
        public static T DeserializeObject<T>(string str, JsonSerializerSettings settings = null)
            => str.DeserializeObject<T>(settings);
        public static T GetArray<T>(this string Value) => JArray.Parse(Value).ToObject<T>();
    }
}