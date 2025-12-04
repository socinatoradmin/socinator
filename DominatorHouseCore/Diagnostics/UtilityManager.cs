#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WindowsInstaller;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.DashboardVms;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using Microsoft.VisualBasic.Devices;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    public class UtilityManager
    {
        public static SubscribtionPopUpModel PopUpModel { get; set; }= new SubscribtionPopUpModel();
        public static HashSet<SocialNetworks> Networks { get; set; } = new HashSet<SocialNetworks>();
        public static async Task<HashSet<SocialNetworks>> ResolveExceptions(string inputString, string exemption,
            string fixtures, string exemptionType)
        {
            string details=string.Empty;
            var message = "LangKeySomethingWentWrong".FromResourceDictionary();
            string finalResponse = null;
            string availbleNetworkResponse = null;
            if(Utilities.DisableLicenseInDebug)
                inputString = ConfigurationManager.AppSettings["Matched"];
            try
            {
                if (inputString == ConfigurationManager.AppSettings["Unavailable"])
                {
                    if (exemptionType != "Fatal")
                    {
                        if (exemptionType == "Debug")
                            using (var streamReader = new StreamReader(await DebugLogExemptions(exemption, fixtures)))
                            {
                                finalResponse = await streamReader.ReadToEndAsync();
                            }
                        else if (exemptionType == "Patal")
                            using (var streamReader =
                                new StreamReader(await DebugPowerLogExemptions(exemption, fixtures)))
                            {
                                finalResponse = await streamReader.ReadToEndAsync();
                            }
                        else
                            using (var streamReader = new StreamReader(await LogExemptions(exemption, fixtures)))
                            {
                                finalResponse = await streamReader.ReadToEndAsync();
                            }
                    }

                    return await ResolveExceptions(JObject.Parse(finalResponse)["code"].ToString(), exemption,
                        fixtures, exemptionType);
                }

                if (inputString == ConfigurationManager.AppSettings["EmptyExemption"])
                {
                    message = "EmptyExemptionErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["ExemptionNotFound"])
                {
                    message = "ExemptionNotFoundErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["ExemptionDisabled"])
                {
                    message = "ExemptionDisabledErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["ExemptionExpired"] ||
                         inputString == ConfigurationManager.AppSettings["FatalExcemptionExpired"] ||
                         inputString == ConfigurationManager.AppSettings["Fluent"])
                {
                    message = "ExemptionExpiredErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["InvalidInput"])
                {
                    message = "InvalidInputErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["MoreLimits"] ||
                         inputString == ConfigurationManager.AppSettings["Invalid"])
                {
                    message = "MoreLimitsErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["NoMoreAllowed"])
                {
                    message = "NoMoreAllowedErrorMessage".FromResourceDictionary();
                }

                else if (inputString == ConfigurationManager.AppSettings["Matched"] && exemptionType == "Other")
                {
                    if (!Utilities.DisableLicenseInDebug)
                    {
                        var availableExemption = await FindExemptions(exemption);
                        using (var streamReader = new StreamReader(availableExemption))
                        {
                            string decryptedString;
                            availbleNetworkResponse = await streamReader.ReadToEndAsync();
                            var exemptionNumber =
                                JObject.Parse(availbleNetworkResponse)[ConfigurationManager.AppSettings["ExemptionId"]]
                                    .ToString();
                            var exemptionErrorDetails = await GetExemptionInnerException(exemptionNumber);
                            using (var stream = new StreamReader(exemptionErrorDetails))
                            {
                                decryptedString = await stream.ReadToEndAsync();
                            }
                            if (!isPaypal)
                            {

                                await CheckNotificationForPopUp(details);
                                if (isPaypal)
                                {
                                    try
                                    {
                                        InvoiceManager = new InvoiceManager();
                                        InvoiceManager.NetworkAccessDetails = JsonConvert.DeserializeObject<NetworkAccessDetails>(availbleNetworkResponse);
                                        var rawInvoiceDetails = JsonConvert.DeserializeObject<InvoiceDetails[]>(details);
                                        InvoiceManager.InvoiceDetails = rawInvoiceDetails[0];
                                        return null;
                                    }
                                    catch (Exception)
                                    {
                                        InvoiceManager = null;
                                        return null;
                                    }
                                }
                                await LogAccess(availbleNetworkResponse, details, exemption);
                                isPaypal = true;
                            }
                            details = AesDecryption.DecryptAes(decryptedString);

                        }
                        if (!isPaypal)
                        {

                            await CheckNotificationForPopUp(details);
                            if (isPaypal)
                            {
                                try
                                {
                                    InvoiceManager = new InvoiceManager();
                                    InvoiceManager.NetworkAccessDetails = JsonConvert.DeserializeObject<NetworkAccessDetails>(availbleNetworkResponse);
                                    var rawInvoiceDetails = JsonConvert.DeserializeObject<InvoiceDetails[]>(details);
                                    InvoiceManager.InvoiceDetails = rawInvoiceDetails[0];
                                    return null;
                                }
                                catch (Exception)
                                {
                                    InvoiceManager = null;
                                    return null;
                                }
                            }
                            await LogAccess(availbleNetworkResponse, details, exemption);
                            isPaypal = true;
                        }
                    }
                    else
                    {
                        exemptionType = "Patal";
                        details = "Excemption";
                    }
                    await UpdatePopModel(details,true);
                    return LogExceptionForEachNetwork(details, exemptionType);
                }
                else if (exemptionType != "Other" && (inputString == ConfigurationManager.AppSettings["Matched"] ||
                                                      inputString == ConfigurationManager.AppSettings["Uniform"]))
                {
                    var  detailsError = exemption.Split('-')[0];

                    await LogAccess(availbleNetworkResponse, details, exemption);
                    await UpdatePopModel(details, true);
                    return LogExceptionForEachNetwork(detailsError, exemptionType);
                }

                else if (inputString == ConfigurationManager.AppSettings["Unknown"])
                {
                    message = "UnknownErrorMessage".FromResourceDictionary();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog($"fixt_{fixtures}");
            }

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //var customDialog = new CustomDialog()
                    //{
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Content = message
                    //};
                    //var objDialog = new Dialog();
                    //var dialogWindow = objDialog.GetMetroWindowWithOutClose(customDialog, ConfigurationManager.AppSettings["Title"]);


                    //var sleep = 1;
                    //while (true)
                    //{
                    //    if (sleep < 10)
                    //    {

                    //    }
                    //}

                    //dialogWindow.ShowDialog();
                    //Thread.Sleep(10 * 1000);
                    //dialogWindow.Close();
                    GlobusLogHelper.log.Debug(
                        $"IS:{inputString}| ET:{exemptionType} | {exemption}:{fixtures} | FR:{finalResponse} | ANR:{availbleNetworkResponse}");
                    Dialog.ShowDialog("LangKeyLicenseError".FromResourceDictionary(), message);
                    return new HashSet<SocialNetworks>();
                });
            }
            else
            {
                //var customDialog = new CustomDialog()
                //{
                //    HorizontalAlignment = HorizontalAlignment.Center,
                //    Content = message
                //};
                //var objDialog = new Dialog();
                //var dialogWindow = objDialog.GetMetroWindowWithOutClose(customDialog, ConfigurationManager.AppSettings["Title"]);
                //dialogWindow.ShowDialog();

                //Thread.Sleep(10 * 1000);
                //dialogWindow.Close();
                //return new HashSet<SocialNetworks>();
                GlobusLogHelper.log.Debug(
                    $"IS:{inputString}| ET:{exemptionType} | {exemption}:{fixtures} | FR:{finalResponse} | ANR:{availbleNetworkResponse}");
                Dialog.ShowDialog("LangKeyLicenseError".FromResourceDictionary(), message);
                return new HashSet<SocialNetworks>();
            }
            return new HashSet<SocialNetworks>();
        }
        public static async Task LogAccess(string userDetails, string InvoiceDetails,string key)
        {
            try
            {
                var userHandle = new JsonHandler(userDetails);

                if (userHandle != null)
                {
                    var model = new LogModel();
                    var userlog = new UserLogs();
                    model.name = userHandle.GetElementValue("name");
                    model.username = userHandle.GetElementValue("username");
                    model.email = userHandle.GetElementValue("email");
                    model.plan = userHandle.GetElementValue("scheme_title");
                    model.license_key = key;
                    model.expire_date = userHandle.GetElementValue("license_expires");
                    userlog.license_key = key;
                    userlog.log_time = DateTime.UtcNow.GetCurrentEpochTimeSeconds();
                    model.user_logs = userlog;
                    var pd = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                    var postData = Encoding.UTF8.GetBytes(pd);
                    var response = await HttpHelper.PostResponseStreamAsync(Utility.Constants.AccessLogUrl, postData);
                }
            }
            catch (Exception)
            {
            }
            
            
        }
        public static async Task CheckNotificationForPopUp(string details)
        {
            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceDetails[]>(details);
                if (data != null)
                {
                    var actualData = data[0];
                    var CurrentDate = DateTime.UtcNow;
                    var dateFoundExpireRaw = actualData.nested.access[actualData.nested.access.Count - 1].expire_date;
                    var dateFoundExpire = DateTime.ParseExact(dateFoundExpireRaw, "yyyy-MM-dd",
                                           System.Globalization.CultureInfo.InvariantCulture);
                    if (actualData.paysys_id.Equals("paypal"))
                    {
                        isPaypal = true;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static async Task<Stream> ProcessInputString(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.ProcessingInput, exemption,
                fixtures));
        }

        public static async Task<Stream> ProcessDebugTypeString(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.ProcessingDebugType,
                exemption, fixtures));
        }

        private static async Task<Stream> FindExemptions(string exemption)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.FindExemptions, exemption));
        }

        public static async Task<Stream> ProcessPowerof7InputString(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.DebugPower, exemption,
                fixtures));
        }


        private static async Task<Stream> GetExemptionInnerException(string innerException)
        {
            var key = ConfigurationManager.AppSettings["ExceptionKey"];
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.ExemptionInnerException,
                innerException, key));
        }

        private static async Task<Stream> LogExemptions(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.LogExemptions, exemption,
                fixtures));
        }

        private static async Task<Stream> DebugLogExemptions(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.DebugLogExemptions, exemption,
                fixtures));
        }

        private static async Task<Stream> DebugPowerLogExemptions(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.DebugPowerLogExemptions,
                exemption, fixtures));
        }


        //private static async Task<Stream> LogDebugExemption(string exemption, string fixtures)
        //{
        //    return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.LogDebugExemption, exemption, fixtures));
        //}

        public static string GetFixtures()
        {
            var uuid = string.Empty;
            try
            {
                var ComputerName = "localhost";
                var scope = new ManagementScope($"\\\\{ComputerName}\\root\\CIMV2", null);
                scope.Connect();
                var query = new ObjectQuery("SELECT UUID FROM Win32_ComputerSystemProduct");
                var searcher = new ManagementObjectSearcher(scope, query);
                foreach (var wmiObject in searcher.Get()) uuid = wmiObject["UUID"].ToString();

                var split = Regex.Split(uuid, "-");
                return split.Last();
            }
            catch (Exception e)
            {
                e.DebugLog($"Exception {e.Message} Trace {e.StackTrace}");
                return Alternate();
            }

            //var fixtures = string.Empty;
            //try
            //{
            //    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            //    {
            //        if (nic.OperationalStatus != OperationalStatus.Up)
            //            continue;

            //        if (!string.IsNullOrEmpty(fixtures))
            //            break;

            //        fixtures += nic.GetPhysicalAddress().ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //return fixtures;
        }

        private static string Alternate()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            var macAddress = string.Empty;
            long maxSpeed = -1;
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //log.Debug(
                    //    "Found MAC Address: " + nic.GetPhysicalAddress() +
                    //    " Type: " + nic.NetworkInterfaceType);

                    var tempMac = nic.GetPhysicalAddress().ToString();
                    if (nic.Speed > maxSpeed &&
                        !string.IsNullOrEmpty(tempMac) &&
                        tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                    {
                        //log.Debug("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
                        maxSpeed = nic.Speed;
                        macAddress = tempMac;
                    }
                }

                return macAddress;
            }
            catch (Exception e)
            {
                e.DebugLog($"Exception {e.Message} Trace {e.StackTrace}");
                return null;
            }
        }
        static bool isPaypal;
        static InvoiceManager InvoiceManager;
        public static async Task<InvoiceManager> CheckPaymentType(string exemption)
        {
            Networks =  await LogIndividualNetworksExceptions(exemption);
            return InvoiceManager;

        }
        public static async Task<HashSet<SocialNetworks>> LogIndividualNetworksExceptions(string exemption)
        {
            var fixture = string.Empty;
            try
            {
                if (Utilities.DisableLicenseInDebug)
                    exemption = "Patal";
                if (string.IsNullOrEmpty(exemption))
                {
                    FeatureFlags.Instance = new FeatureFlags
                    {
                        {"SocinatorInitializer", true}
                    };
                    return SocinatorInitialize.AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }
                if (!Utilities.DisableLicenseInDebug)
                    fixture = await Task.Run(()=> GetFixtures());
                //fixture = "A22BC371EAE6";
                string finalResponse;
                string exemptionType;
                if (exemption.Contains(ConfigurationManager.AppSettings["DebugType"]))
                {
                    var responseStream = await ProcessDebugTypeString(exemption, fixture);
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        finalResponse = await streamReader.ReadToEndAsync();
                        exemptionType = "Debug";
                    }
                }
                else if (exemption.Contains(ConfigurationManager.AppSettings["FatalException"]))
                {
                    finalResponse = await ProcessFatalException(exemption, fixture);
                    exemptionType = "Fatal";
                }
                else if (exemption.Contains(ConfigurationManager.AppSettings["PatalException"]))
                {
                    var stream = await ProcessPowerof7InputString(exemption, fixture);
                    using (var streamReader = new StreamReader(stream))
                    {
                        finalResponse = await streamReader.ReadToEndAsync();
                        exemptionType = "Patal";
                    }
                }
                else
                {
                    if (!Utilities.DisableLicenseInDebug)
                    {
                        var responseStream = await ProcessInputString(exemption, fixture);
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            finalResponse = await streamReader.ReadToEndAsync();
                            exemptionType = "Other";
                        }
                    }
                    else
                    {
                        finalResponse = $"{{\"code\":\"\"}}";
                        exemptionType = "Other";
                    }
                }
                await UpdatePopModel(finalResponse);
                return await ResolveExceptions(JObject.Parse(finalResponse)["code"].ToString(), exemption, fixture,
                    exemptionType);
            }
            catch (Exception ex)
            {
                ex.DebugLog($"fixt_{fixture}");

                if (!ex.Message.Contains("The remote name could not be resolved"))
                    return SocinatorInitialize.AvailableNetworks;

                var dialogResult = Dialog.ShowCustomDialog(
                    "LangKeyNetworkError".FromResourceDictionary(),
                    "LangKeyCheckInternet".FromResourceDictionary(),
                    "LangKeyTryAgain".FromResourceDictionary(),
                    "LangKeyCancel".FromResourceDictionary());
                if (dialogResult == MessageDialogResult.Affirmative) return null;
            }

            return SocinatorInitialize.AvailableNetworks;
        }

        private static async Task UpdatePopModel(string finalResponse,bool IsFinal=false)
        {
            try
            {
                if (IsFinal)
                {
                    var beginDate = Utilities.GetBetween(finalResponse, "\"begin_date\":\"", "\"");
                    DateTime.TryParse(beginDate, out DateTime beginDate1);
                    var days = (PopUpModel.expires - beginDate1).Days;
                    PopUpModel.IsTrial = days > 0 && days <= 31;
                }
                else
                {
                    var parser = new JsonHandler(finalResponse);
                    var expires = parser.GetElementValue("expires");
                    if (!string.IsNullOrEmpty(expires))
                    {
                        DateTime.TryParse(parser.GetElementValue("expires"), out DateTime time);
                        PopUpModel.expires = time;
                    }
                    else
                    {
                        PopUpModel.expires = new DateTime();
                    }
                }
            }
            catch { }
        }

        public static HashSet<SocialNetworks> LogExceptionForEachNetwork(string details, string exceptionType)
        {
            try
            {
                FeatureFlags.Instance = new FeatureFlags { { "SocinatorInitializer", true } };

                SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);

                if (string.IsNullOrEmpty(details))
                    return SocinatorInitialize.AvailableNetworks;

                #region All networks with unlimited Accounts

                try
                {
                    if (exceptionType == "Other")
                    {
                        var jsonArray = JArray.Parse(details);

                        #region Getting Exemption Title

                        var exemptionItems =
                            jsonArray.Children()["nested"].First()[ConfigurationManager.AppSettings["ExemptionItem"]]
                                .ToString();
                        var arrayExemptionItems = JArray.Parse(exemptionItems)[0];
                        var exemptionTitle = arrayExemptionItems[ConfigurationManager.AppSettings["ExemptionTitle"]]
                            .ToString();

                        #endregion

                        #region Full Exemption

                        if (exemptionTitle == ConfigurationManager.AppSettings["FullExemption"])
                            try
                            {
                                var options =
                                    jsonArray.Children()["nested"].First()[
                                            ConfigurationManager.AppSettings["ExemptionItem"]].First()["options"]
                                        .ToString();
                                var packageCount =
                                    JObject.Parse(options)[ConfigurationManager.AppSettings["SelectPackage"]]["value"]
                                        .ToString();

                                SocinatorInitialize.MaximumAccountCount =
                                    int.Parse(Utilities.GetIntegerOnlyString(packageCount));
                                AddAllNetwork();
                            }
                            catch (Exception ex)
                            {
                                SocinatorInitialize.MaximumAccountCount = 0;
                                ex.DebugLog();
                            }

                        #endregion

                        #region Custom Exemption

                        else if (exemptionTitle == ConfigurationManager.AppSettings["CustomExemption"])
                            try
                            {
                                var arrInvoiceItems = JArray.Parse(exemptionItems);

                                SocinatorInitialize.AvailableNetworks.Clear();

                                FeatureFlags.Instance = new FeatureFlags
                                    {{"SocinatorInitializer", true}, {"Social", true}};

                                SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);

                                foreach (var token in arrInvoiceItems)
                                    try
                                    {
                                        var tokenString = token["options"].ToString();

                                        var networkSplit = Regex.Split(tokenString, "},");

                                        SocinatorInitialize.MaximumAccountCount = 10000;

                                        foreach (var networkValues in networkSplit)
                                            try
                                            {
                                                var isSelected = Utilities.GetBetween(networkValues, "value\":[\"",
                                                    "\"],");
                                                if (isSelected != "1")
                                                    continue;
                                                var network = Utilities.FirstMatchExtractor(networkValues,
                                                    "optionLabel\":\"(.*?)\"");
                                                //var networks = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                                                var networks = network == "Youtube" ? SocialNetworks.YouTube : (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                                                SocinatorInitialize.AvailableNetworks.Add(networks);
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.DebugLog();
                                            }
                                    }
                                    catch (Exception ex)
                                    {
                                        SocinatorInitialize.MaximumAccountCount = 0;
                                        ex.DebugLog();
                                    }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                SocinatorInitialize.MaximumAccountCount = 0;
                            }

                        #endregion

                        #region Pro Exemption

                        else if(exemptionTitle == ConfigurationManager.AppSettings["ProExemption"]
                            || (!string.IsNullOrEmpty(exemptionTitle) 
                            && exemptionTitle.Contains(ConfigurationManager.AppSettings["ProExemption"])))
                        {
                            AddAllNetwork();
                            var MaxAccount = Utilities.GetBetween(exemptionTitle, "Professional (", " Social Accounts)");
                            if (int.TryParse(MaxAccount, out int maximumAccount))
                                SocinatorInitialize.MaximumAccountCount = maximumAccount;
                            else
                                SocinatorInitialize.MaximumAccountCount = 10;
                        }

                        #endregion

                        #region Team Exemption

                        else if ( (!string.IsNullOrEmpty(exemptionTitle) 
                            && exemptionTitle.Contains(ConfigurationManager.AppSettings["TeamExemption"])))
                        {
                            AddAllNetwork();
                            var MaxAccount=Utilities.GetBetween(exemptionTitle, "Team (", " Social Accounts)");
                            if(int.TryParse( MaxAccount, out int maximumAccount))
                                SocinatorInitialize.MaximumAccountCount = maximumAccount;
                            else
                            SocinatorInitialize.MaximumAccountCount = 70;
                        }

                        #endregion

                        #region Business Exemption

                        else if ( (!string.IsNullOrEmpty(exemptionTitle)
                            && exemptionTitle.Contains(ConfigurationManager.AppSettings["BusinessExemption"])))
                        {
                            AddAllNetwork();
                            var MaxAccount = Utilities.GetBetween(exemptionTitle, "Business (", " Social Accounts)");
                            if (int.TryParse(MaxAccount, out int maximumAccount))
                                SocinatorInitialize.MaximumAccountCount = maximumAccount;
                            else
                                SocinatorInitialize.MaximumAccountCount = 150;
                        }

                        #endregion

                        #region EnterPrise Exemption

                        else if (exemptionTitle == ConfigurationManager.AppSettings["EnterPriseExemption"]
                            || (!string.IsNullOrEmpty(exemptionTitle)
                            && exemptionTitle.Contains(ConfigurationManager.AppSettings["EnterPriseExemption"])))
                        {
                            AddAllNetwork();
                        }

                        #endregion

                        #region Single Exemption

                        else
                            try
                            {
                                SocinatorInitialize.AvailableNetworks.Clear();
                                FeatureFlags.Instance = new FeatureFlags
                                    {{"SocinatorInitializer", true}, {"Social", true}};
                                SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);
                                var exemptionDescription =
                                    arrayExemptionItems[ConfigurationManager.AppSettings["ExemptionDescription"]]
                                        .ToString();
                                exemptionDescription = exemptionDescription
                                    .Replace(ConstantVariable.MarketingSoftware, string.Empty).Trim();
                                var networks =
                                    (SocialNetworks)Enum.Parse(typeof(SocialNetworks), exemptionDescription);
                                SocinatorInitialize.AvailableNetworks.Add(networks);
                                SocinatorInitialize.MaximumAccountCount = 10000;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                SocinatorInitialize.MaximumAccountCount = 0;
                            }

                        #endregion
                    }
                    else if (exceptionType == "Patal")
                    {
                        AddAllNetwork();
                        if (!Utilities.DisableLicenseInDebug)
                            SocinatorInitialize.MaximumAccountCount = 10;
                    }
                    else
                    {
                        try
                        {
                            SocinatorInitialize.AvailableNetworks.Clear();
                            FeatureFlags.Instance = new FeatureFlags { { "SocinatorInitializer", true }, { "Social", true } };
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);
                            var exemptionDescription = ConfigurationManager.AppSettings[details];
                            var networks = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), exemptionDescription);
                            SocinatorInitialize.AvailableNetworks.Add(networks);
                            SocinatorInitialize.MaximumAccountCount = 10000;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            SocinatorInitialize.MaximumAccountCount = 0;
                        }
                    }

                    FeatureFlags.UpdateFeatures();
                    return SocinatorInitialize.AvailableNetworks;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

#endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            FeatureFlags.UpdateFeatures();
            return SocinatorInitialize.AvailableNetworks;
        }

        private static void AddAllNetwork()
        {
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Twitter);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Facebook);
            //SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Gplus);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Instagram);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.LinkedIn);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Quora);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Pinterest);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Tumblr);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.YouTube);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Reddit);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.TikTok);
        }

        public static async Task<string> ProcessFatalException(string exception, string fixture)
        {
            try
            {
                var webClient = new WebClient();
                var form = new NameValueCollection();

                var exceptionLogger = ConfigurationManager.AppSettings["ExceptionLogger"];
                form.Add(ConfigurationManager.AppSettings["ExceptionParameter"], exception);
                form.Add(ConfigurationManager.AppSettings["ExceptionEndPoint"], fixture);
                form.Add(ConfigurationManager.AppSettings["ExceptionPoint"],
                    ConfigurationManager.AppSettings["ExceptionProx"]);
                form.Add(ConfigurationManager.AppSettings["ExceptionPath"], ConfigurationManager.AppSettings["Social"]);
                form.Add(ConfigurationManager.AppSettings["ExceptionCall"], "");


                //Post the data and read the response
                var responseData = webClient.UploadValues(exceptionLogger, form);
                var xml = "<tag>" + Encoding.UTF8.GetString(responseData).Replace("\n", "") + "</tag>";
                try
                {
                    var xdoc = XDocument.Parse(xml);
                    foreach (var elem in xdoc.Descendants("tag"))
                    {
                        var row = elem.Descendants();

                        //string str = elem.ToString();
                        foreach (var element in row)
                            try
                            {
                                var keyName = element.Name.LocalName;
                                if (keyName == "status") return "{\"code\":\"" + element.Value + "\"}";
                            }
                            catch (Exception)
                            {
                                return "{\"code\":\"" + "error" + "\"}";
                            }
                    }
                }
                catch (Exception)
                {
                    return "{\"code\":\"" + "error" + "\"}";
                }
            }
            catch (Exception)
            {
                return "{\"code\":\"" + "error" + "\"}";
            }

            return string.Empty;
        }

        public static async Task<bool> CheckForNewUpdates()
        {
            try
            {
                string finalResponse;

                var revisionHistoryViewModel =
                    InstanceProvider.GetInstance<IDashboardViewModel>("RevisionHistory");
                var currentVersion = Utilities.GetBetween(revisionHistoryViewModel.CurrentVersion, "[", "]");

                using (var streamReader = new StreamReader(await ProcessUpdatedVersionString()))
                {
                    finalResponse = streamReader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(finalResponse.Trim()))
                    return false;

                if (currentVersion.Trim() != finalResponse.Trim())
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public static async Task<bool> InstallNewUpdates()
        {
            try
            {
                var installedApplication = false;

                var type = Type.GetTypeFromProgID("WindowsInstaller.Installer");
                var installer = (Installer)Activator.CreateInstance(type);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    installer.InstallProduct(string.Format(ConstantVariable.UpdateVersionLink
                            , ConstantVariable.UpdatedVersionIP, ConstantVariable.UpdateInstallerFilePath),
                        "PROPERTY=VALUE");

                    installedApplication = true;
                });

                while (!installedApplication)
                    await Task.Delay(1000);

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public static async Task<Stream> ProcessUpdatedVersionString()
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.UpdateVersionLink
                , ConstantVariable.UpdatedVersionIP, ConstantVariable.UpdateVersionFilePath));
        }

        public static async Task<bool> ProcessAccessLog(List<Models.DominatorAccountModel> accountList, FileManagers.ICampaignsFileManager campaignsFileManager)
        {
            try
            {
                if (accountList.Count > 0)
                {
                    AccessLogModel accessLogModel = new AccessLogModel();
                    accessLogModel.licKey = FileManagers.SocinatorKeyHelper.Key.FatalErrorMessage;
                    if (string.IsNullOrEmpty(accessLogModel.licKey))
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        return false;
                    }
                    var accountgroup = accountList.GroupBy(y => y.AccountBaseModel.AccountNetwork);
                    List<AccDetail> listAcc = new List<AccDetail>();
                    List<ActivityDetail> listActivity = new List<ActivityDetail>();
                    var tempCamps = campaignsFileManager.GetTemp();
                    accountgroup.ForEach(y =>
                    {
                        AccDetail accDetail = new AccDetail();
                        accDetail.acc_count = y?.Count() ?? 0;
                        accDetail.success_count = y.Where(x => x.AccountBaseModel.Status == AccountStatus.Success)?.Count() ?? 0;
                        accDetail.network = y.FirstOrDefault().AccountBaseModel.AccountNetwork.ToString();
                        listAcc.Add(accDetail);
                    });
                    accessLogModel.acc_details = listAcc.ToArray();
                    campaignsFileManager.ForEach(camp =>
                    {
                        ActivityDetail activity = new ActivityDetail();
                        activity.network = camp.SocialNetworks.ToString();
                        activity.activity = camp.MainModule;
                        activity.sub_activity = camp.SubModule;
                        activity.status = camp.Status;
                        activity.creation_date = camp.CreationDate;
                        listActivity.Add(activity);
                    });
                    tempCamps.ForEach(camp =>
                    {
                        ActivityDetail activity = new ActivityDetail();
                        activity.network = camp.CampignDeletedTemps.SocialNetworks.ToString();
                        activity.activity = camp.CampignDeletedTemps.MainModule;
                        activity.sub_activity = camp.CampignDeletedTemps.SubModule;
                        activity.status = "Removed";
                        activity.creation_date = camp.CampignDeletedTemps.CreationDate;
                        listActivity.Add(activity);
                    });
                    campaignsFileManager.RemoveTemp();
                    accessLogModel.activity_details = listActivity.ToArray();
                    RunningTime runningTime = new RunningTime();
                    runningTime.start = DateTimeUtilities.ConvertToEpoch(Utility.Constants.StartTime);
                    runningTime.end = DateTimeUtilities.ConvertToEpoch(DateTime.Now);
                    accessLogModel.running_time = runningTime;
                    var pd = Newtonsoft.Json.JsonConvert.SerializeObject(accessLogModel);
                    var postData = Encoding.UTF8.GetBytes(pd);
                    var response = await HttpHelper.PostResponseStreamAsync(Utility.Constants.AccessLogUrl, postData);
                    if (response.Response.Contains("{\"status\":1}"))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class Access
    {
        public int access_id { get; set; }
        public string invoice_id { get; set; }
        public string invoice_public_id { get; set; }
        public string invoice_payment_id { get; set; }
        public string invoice_item_id { get; set; }
        public string user_id { get; set; }
        public string product_id { get; set; }
        public string transaction_id { get; set; }
        public string begin_date { get; set; }
        public string expire_date { get; set; }
        public string qty { get; set; }
        public object comment { get; set; }
    }

    public class InvoiceItem
    {
        public string orig_first_price { get; set; }
        public string orig_second_price { get; set; }
        public int invoice_item_id { get; set; }
        public string invoice_id { get; set; }
        public string invoice_public_id { get; set; }
        public string item_id { get; set; }
        public string item_type { get; set; }
        public string item_title { get; set; }
        public string item_description { get; set; }
        public string qty { get; set; }
        public string first_price { get; set; }
        public string first_discount { get; set; }
        public string first_tax { get; set; }
        public string first_total { get; set; }
        public string first_shipping { get; set; }
        public string first_period { get; set; }
        public string rebill_times { get; set; }
        public string second_price { get; set; }
        public string second_discount { get; set; }
        public string second_tax { get; set; }
        public string second_total { get; set; }
        public string second_shipping { get; set; }
        public string second_period { get; set; }
        public string currency { get; set; }
        public string tax_group { get; set; }
        public string is_countable { get; set; }
        public string variable_qty { get; set; }
        public object is_tangible { get; set; }
        public string billing_plan_id { get; set; }
        public string billing_plan_data { get; set; }
        public string options { get; set; }
        public object option1 { get; set; }
        public object option2 { get; set; }
        public object option3 { get; set; }
        public object tax_rate { get; set; }
    }

    public class InvoicePayment
    {
        public int invoice_payment_id { get; set; }
        public string invoice_id { get; set; }
        public string invoice_public_id { get; set; }
        public string user_id { get; set; }
        public string paysys_id { get; set; }
        public string receipt_id { get; set; }
        public string transaction_id { get; set; }
        public string dattm { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string discount { get; set; }
        public string tax { get; set; }
        public string shipping { get; set; }
        public object refund_dattm { get; set; }
        public object refund_amount { get; set; }
        public string base_currency_multi { get; set; }
        public string display_invoice_id { get; set; }

        [JsonProperty("conversion-track-done")]
        public string conversiontrackdone { get; set; }

        [JsonProperty("google-analytics-done")]
        public string googleanalyticsdone { get; set; }
    }

    public class Nested
    {
        [JsonProperty("invoice-items")]
        public List<InvoiceItem> invoiceitems { get; set; }

        [JsonProperty("invoice-payments")]
        public List<InvoicePayment> invoicepayments { get; set; }
        public List<Access> access { get; set; }
    }

    public class InvoiceDetails
    {
        [JsonProperty("paypal-profile-id")]
        public string paypalprofileid { get; set; }
        public int invoice_id { get; set; }
        public string user_id { get; set; }
        public string paysys_id { get; set; }
        public string currency { get; set; }
        public string first_subtotal { get; set; }
        public string first_discount { get; set; }
        public string first_tax { get; set; }
        public string first_shipping { get; set; }
        public string first_total { get; set; }
        public string first_period { get; set; }
        public string rebill_times { get; set; }
        public string second_subtotal { get; set; }
        public string second_discount { get; set; }
        public string second_tax { get; set; }
        public string second_shipping { get; set; }
        public string second_total { get; set; }
        public string second_period { get; set; }
        public object tax_rate { get; set; }
        public object tax_type { get; set; }
        public object tax_title { get; set; }
        public string status { get; set; }
        public object coupon_id { get; set; }
        public object coupon_code { get; set; }
        public string discount_first { get; set; }
        public string discount_second { get; set; }
        public string is_confirmed { get; set; }
        public string public_id { get; set; }
        public string invoice_key { get; set; }
        public string tm_added { get; set; }
        public string tm_started { get; set; }
        public object tm_cancelled { get; set; }
        public string rebill_date { get; set; }
        public object due_date { get; set; }
        public object terms { get; set; }
        public object comment { get; set; }
        public string base_currency_multi { get; set; }
        public string saved_form_id { get; set; }
        public string remote_addr { get; set; }
        public object aff_id { get; set; }
        public object keyword_id { get; set; }
        public Nested nested { get; set; }
    }

    public class NetworkAccessDetails
    {
        public string code { get; set; }
        public string message { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string invoice_id { get; set; }
        public string invoice_item_id { get; set; }
        public string scheme_id { get; set; }
        public string scheme_title { get; set; }
        public DateTime license_expires { get; set; }
        public int next_check { get; set; }
    }

    public class InvoiceManager
    {
        public InvoiceDetails InvoiceDetails { get; set; }
        public NetworkAccessDetails NetworkAccessDetails { get; set; }
    }

}