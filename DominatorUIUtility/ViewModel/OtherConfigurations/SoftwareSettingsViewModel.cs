using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class SoftwareSettingsViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly ISoftwareSettings _softwareSettings;
        private readonly IPuppeteerBrowserManager browserManager;
        private bool _progressRing;

        private string _searchText;

        public SoftwareSettingsViewModel(ISoftwareSettings softwareSettings, IPuppeteerBrowserManager puppeteerBrowser) : base("LangKeySoftwareSettings",
            "SoftwareSettingsControlTemplate")
        {
            _softwareSettings = softwareSettings;
            browserManager = puppeteerBrowser;
            SaveCmd = new DelegateCommand(Save);
            SoftwareSettingsModel = softwareSettings.Settings??new SoftwareSettingsModel() { IsDoNotAutoLoginAccountsWhileAddingToSoftware = true, IsThreadLimitChecked = true, MaxThreadCount = 5, RunQueriesRandomly = true, SortByUsername = true };
            ExportCommand = new DelegateCommand(Export);

            //Assign LocationDetails
            SoftwareSettingsModel.ListLocationModelTemp =
                SoftwareSettingsModel.ListLocationModel = softwareSettings.AssignLocationList();

            SoftwareSettingsModel.DebugVisibility = Visibility.Collapsed;

#if DEBUG
            SoftwareSettingsModel.DebugVisibility = Visibility.Visible;
#endif
        }

        public SoftwareSettingsModel SoftwareSettingsModel { get; }

        public DelegateCommand SaveCmd { get; }

        public DelegateCommand ExportCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText)
                    return;
                SetProperty(ref _searchText, value);
                SetListLocationModel(value);
            }
        }

        public bool ProgressRing
        {
            get => _progressRing;
            set
            {
                if (_progressRing == value) return;
                SetProperty(ref _progressRing, value);
            }
        }

        public void SetListLocationModel(string input)
        {
            SoftwareSettingsModel.ListLocationModel.ForEach
            (x => x = SoftwareSettingsModel.ListLocationModelTemp.FirstOrDefault(
                y => y.CountryName == x.CountryName));

            SoftwareSettingsModel.ListLocationModelTemp = new ObservableCollection<LocationModel>
            (SoftwareSettingsModel.ListLocationModel.ToList().Where(x =>
                x.CountryName.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)));
        }

        private void Export()
        {
            SoftwareSettingsModel.ExportPath = FileUtilities.GetExportPath(true);
        }

        private void Save()
        {
            if (SoftwareSettingsModel.IsSelectCountriesFilter)
            {
                ProgressRing = true;
                ThreadFactory.Instance.Start(() => { DownloadLocations(); });
            }

            if (SoftwareSettingsModel.IsDefaultExportPathSelected)
            {
                if (!string.IsNullOrEmpty(SoftwareSettingsModel.ExportPath) &&
                    Directory.Exists(SoftwareSettingsModel.ExportPath))
                    SaveSetting();
                else
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                        "LangKeyEnterValidFolderPath".FromResourceDictionary());
            }
            else
            {
                SoftwareSettingsModel.ExportPath = string.Empty;
                SaveSetting();
            }
        }

        private void SaveSetting()
        {
            ThreadFactory.Instance.Start(() =>
            {
                while (ProgressRing)
                    Thread.Sleep(500);

                if (_softwareSettings.Save())
                    Application.Current.Dispatcher.Invoke(async() =>
                    {
                        var result = Dialog.ShowCustomDialog("LangKeySuccess".FromResourceDictionary(),
                            "LangKeyConfirmToRestartAfterSoftwareSettingSaved".FromResourceDictionary(),
                            "LangKeyRestartNow".FromResourceDictionary(),
                            "LangKeyRestartLater".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                        {
                            await browserManager.RemoveAllBrowser();
                            Application.Current.Shutdown();
                            Process.Start(Application.ResourceAssembly.Location);
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);
                        }
                    });
            });
        }

        public (List<LocationList>, DbOperations) GetLocationList()
        {
            try
            {
                var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                var dbGlobalContext = dataBaseConnectionGlb.GetSqlConnection();
                var _dbGlobalListOperations = new DbOperations(dbGlobalContext);
                return (_dbGlobalListOperations.Get<LocationList>(),_dbGlobalListOperations);
            }
            catch { return (new List<LocationList>(),null); }
        }

        private void DownloadLocations()
        {
            try
            {
                var LocationData = GetLocationList();
                var ListCountry = LocationData.Item1;
                var DbContext = LocationData.Item2;
                var dt = new List<LocationList>();
                foreach (var locationModel in SoftwareSettingsModel.ListLocationModel.Where(x => x.IsSelected))
                    try
                    {
                        if (ListCountry.Any(x => x.CountryName.Equals(locationModel.CountryName)))
                            continue;

                        var url = "https://countriesnow.space/api/v0.1/countries/cities";
                        var json = $"{{\"country\":\"{locationModel?.CountryName}\"}}";

                        using (HttpClient client = new HttpClient())
                        {
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response1 = client.PostAsync(url, content).Result;
                            var result = response1.Content.ReadAsStringAsync().Result;
                            try
                            {
                                var obj = JObject.Parse(result);
                                var cityList = obj["data"]?.ToObject<List<string>>();
                                cityList.ForEach(x =>
                                {
                                    var lst = new LocationList
                                    {
                                        CountryName = locationModel.CountryName,
                                        CityName = x,
                                        IsSelected = false
                                    };
                                    dt.Add(lst);
                                });
                            }
                            catch { }
                            
                        }
                        #region Obsolute Code.
                        ////TODO: Make url at some standard place
                        //var request = (HttpWebRequest)WebRequest.Create($"https://builds.socinator.com/DownloadForSocinator/CityListByCountries/{locationModel.CountryName}.txt");
                        //var response = request.GetResponse();
                        //var cityResponse = string.Empty;
                        //using (var responseStream = response.GetResponseStream())
                        //{
                        //    if (responseStream != null)
                        //    {
                        //        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        //        cityResponse = reader.ReadToEnd();
                        //    }
                        //}

                        //var cityList = Regex.Split(cityResponse, "\r\n").ToList();
                        //cityList.ForEach(x =>
                        //{
                        //    var lst = new LocationList
                        //    {
                        //        CountryName = locationModel.CountryName,
                        //        CityName = x,
                        //        IsSelected = false
                        //    };
                        //    dt.Add(lst);
                        //});
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                if(DbContext != null)
                    DbContext.AddRange(dt);
                ToasterNotification.ShowSuccess("Location Details Downloaded Successfully");
            }
            catch (WebException)
            {
                ToasterNotification.ShowError("failed To Downloaded Location Details");
            }
            catch (Exception)
            {
                ToasterNotification.ShowError("failed To Downloaded Location Details");
            }

            ProgressRing = false;
        }
    }
}