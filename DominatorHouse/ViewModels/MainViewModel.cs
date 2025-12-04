using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.Common;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DominatorHouse.ViewModels
{
    public class MainViewModel : BindableBase, IMainViewModel
    {
        private readonly IApplicationResourceProvider _applicationResourceProvider;
        private readonly ISchedulerProxy _schedulerProxy;
        private Dock _tabDock;
        private bool _IsDark;
        private bool _validating = false;
        private float opacity = 1f;
        private readonly IPuppeteerBrowserManager browserManager;
        public ILogViewModel LogViewModel { get; }
        public SubscribtionPopUpModel PopUpModel { get; set; } = new SubscribtionPopUpModel();
        public ObservableCollection<DominatorAccountModel> AccountList { get; set; }
        public IPerfCounterViewModel PerfCounterViewModel { get; }

        public SelectableViewModel<string> Themes { get; }

        public ISelectedNetworkViewModel AvailableNetworks { get; }

        public SelectableViewModel<TabItemTemplates> TabItems { get; }

        public AccessorStrategies Strategies { get; set; }
        string _fatalError { get; set; }
        private bool IsCancelFromLicenceValidationState { get; set; }
        public bool IsDark
        {
            get => _IsDark;
            set
            {
                SetProperty(ref _IsDark, value, nameof(IsDark));
            }
        }
        public bool IsValidating
        {
            get => _validating;
            set
            {
                SetProperty(ref _validating, value, nameof(IsValidating));
                if(value)
                    Opacity = 0.5f;
                else
                    Opacity = 1f;
            }
        }
        public bool IsDebug
        {
            get
            {
              #if DEBUG
                return true;
             #endif
                return false;
            }
        }

        public Dock TabDock
        {
            get
            {
                return _tabDock;
            }
            set
            {
                SetProperty(ref _tabDock, value, nameof(TabDock));
            }
        }
        public float Opacity
        {
            get => opacity;
            set => SetProperty(ref opacity, value, nameof(Opacity));
        }
        public KeyValuePair<int, int> ScreenResolution { get; set; } = new KeyValuePair<int, int>();

        public MainViewModel(ILogViewModel logViewModel, IApplicationResourceProvider applicationResourceProvider, IPerfCounterViewModel perfCounterViewModel, ISelectedNetworkViewModel availableNetworks, ISchedulerProxy schedulerProxy)
        {
            SocinatorKeyHelper.InitilizeKey();
            RemoveLocationDataFromTemplate().Wait();
            Application.Current.Dispatcher.InvokeAsync(async () => await FatalErrorDiagnosis());
            browserManager = InstanceProvider.GetInstance<IPuppeteerBrowserManager>();
            Application.Current.MainWindow.Closing += (s, e) => OnClosing(e);
            LogViewModel = logViewModel;
            _applicationResourceProvider = applicationResourceProvider;
            PerfCounterViewModel = perfCounterViewModel;
            AvailableNetworks = availableNetworks;
            _schedulerProxy = schedulerProxy;
            Themes = new SelectableViewModel<string>(GetThemes());
            AvailableNetworks.ItemSelected += OnAvailableNetworks_ItemSelected;
            TabItems = new SelectableViewModel<TabItemTemplates>(new List<TabItemTemplates>());
            TabItems.ItemSelected += OnTabItems_ItemSelected;
            TabSwitcher.ChangeTabIndex = (mainTabIndex, subTabIndex) =>
            {
                TabItems.SelectByIndex(mainTabIndex);

                if (subTabIndex == null)
                    return;

                //TODO : it's awful! (use dymanic for it) change it later!
                var selectedTabObject = TabItems.Selected?.Content.Value;

                ((dynamic)selectedTabObject)?.SetIndex((int)subTabIndex);
            };

            // Go to campaign from respective module after campaign saved
            TabSwitcher.GoToCampaign = ()
                => TabItems.Selected =
                    TabItems.Items.FirstOrDefault(x => x.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyCampaigns));

            TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;

            Strategies = new AccessorStrategies
            {
                _determine_available = a => AvailableNetworks.Contains(a),
                _inform_warnings = GlobusLogHelper.log.Warn
            };

            var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountList = new ObservableCollection<DominatorAccountModel>(accountFileManager.GetAll()
                .OrderBy(s => s.AccountBaseModel.UserName).ToList());

            Socinator.DominatorCores.DominatorCoreBuilder.Strategies = Strategies;
            Constants.StartTime = DateTime.Now;
        }
        public async Task<bool> InstallFFMpeg()
        {
            var file = GetFilePath(IsDebug);
            var exe = ConstantVariable.FFMPegPath = Path.Combine(file, "ffmpeg.exe");
            try
            {
                if (File.Exists(exe))
                    return true;
                //using (HttpClient client = new HttpClient())
                //using (var response = await client.GetAsync(ConstantVariable.FFMPegExecutable))
                //using (var fs = new FileStream(exe, FileMode.Create))
                //{
                //    await response.Content.CopyToAsync(fs);
                //}

                WebRequest request = WebRequest.Create(ConstantVariable.FFMPegExecutable);
                request.Method = "GET";

                // Get the response
                using (WebResponse response = request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(exe, FileMode.Create, FileAccess.Write))
                {
                    await responseStream.CopyToAsync(fileStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog("Failed To Download FFMPEG!");
                return false;
            }
            finally
            {
                if (IsDebug)
                {
                    try
                    {
                        File.Copy(exe, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"), true);
                    }
                    catch { }
                }
            }
        }
        public async Task InstallVisualCplus()
         {
            try
            {
                // Run the WMI query off the UI thread to avoid blocking
                bool isVcInstalled = await Task.Run(() =>
                {
                    try
                    {
                        using (var mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name LIKE '%Microsoft Visual C++%'"))
                        {
                            if (mos.Get().Cast<ManagementObject>().Any(x => Convert.ToInt32(x["Version"].ToString().Substring(0, 2)) >= 14))
                                return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog("Error checking Visual C++ install: " + ex.Message);
                    }
                    return false;
                });

                if (isVcInstalled)
                    return;
                string appFilePath = Path.Combine(ConstantVariable.GetVisualPath(), "VC_redist.x86.exe");

                try
                {
                    //using (HttpClient client = new HttpClient())
                    //{
                    //    using (var response = await client.GetAsync(Constants.VisualCPlusUrl))
                    //    {
                    //        response.EnsureSuccessStatusCode();
                    //         using (var fs = new FileStream(appFilePath, FileMode.Create, FileAccess.Write, FileShare.None)) 
                    //               await response.Content.CopyToAsync(fs);
                    //    }

                    WebRequest request = WebRequest.Create(Constants.VisualCPlusUrl);
                    request.Method = "GET";

                    // Get the response
                    using (WebResponse response = request.GetResponse())
                    using (Stream responseStream = response.GetResponseStream())
                    using (FileStream fileStream = new FileStream(appFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog("Download failed: " + ex.Message);
                    return;
                }

                await InstallVcRedistAsync();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public string GetFilePath(bool IsDebug)
        {
            return !IsDebug ? AppDomain.CurrentDomain.BaseDirectory : ConstantVariable.GetCefDependency();
        }
       
        public async Task<bool> InstallDependency()
        {
            var file = GetFilePath(IsDebug);
            var exe = Path.Combine(file, "Dependency.zip");
            var isNeedToInstall = await IsNeedToInstallDependency(file);
            if (!isNeedToInstall && !IsDebug) return true;

            try
            {
                if (isNeedToInstall)
                {
                    GlobusLogHelper.log.Info("Installing Dependencies...");
                    //using (HttpClient client = new HttpClient())
                    //using (var response = await client.GetAsync(ConstantVariable.DependencyFile))
                    //using (var fs = new FileStream(exe, FileMode.Create))
                    //    await response.Content.CopyToAsync(fs);
                    WebRequest request = WebRequest.Create(ConstantVariable.DependencyFile);
                    request.Method = "GET";

                    // Get the response
                    using (WebResponse response = request.GetResponse())
                    using (Stream responseStream = response.GetResponseStream())
                    using (FileStream fileStream = new FileStream(exe, FileMode.Create, FileAccess.Write))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error downloading dependency", ex);
                return false;
            }

            try
            {
                if (isNeedToInstall || IsDebug)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(exe))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            try
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(entry.Name)) continue;
                                    var destinationPath = Path.Combine(IsDebug ? AppDomain.CurrentDomain.BaseDirectory : file, entry.Name);
                                    entry.ExtractToFile(destinationPath, overwrite: true);
                                }
                                catch (Exception e)
                                {}
                                if (IsDebug)
                                {
                                    var destination1 = Path.Combine(file, entry.Name);
                                    entry.ExtractToFile(destination1, overwrite: true);
                                }
                            }
                            catch(Exception c)
                            { }
                        }
                    }
                    await InstallVisualCplus();
                    if (isNeedToInstall)
                        GlobusLogHelper.log.Info("Dependencies installed successfully!");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error extracting dependency", ex);
                return false;
            }
            finally
            {
                try
                {
                    if (!IsDebug && File.Exists(exe))
                        File.Delete(exe);
                }
                catch (Exception)
                {
                }
            }
        }


        public async Task<bool> IsNeedToInstallDependency(string folderPath)
        {
            try
            {
                string[] filesCheck = {
                    "AutoItX3.dll", "AutoItX3_x64.dll", "chrome_100_percent.pak", "chrome_200_percent.pak",
                    "chrome_elf.dll", "d3dcompiler_47.dll", "icudtl.dat", "libcef.dll", "libEGL.dll",
                    "libGLESv2.dll", "resources.pak", "snapshot_blob.bin","v8_context_snapshot.bin", "vk_swiftshader.dll",
                    "vk_swiftshader_icd.json", "vulkan-1.dll"
                };

                bool allExist = filesCheck.All(fileName =>
                    File.Exists(Path.Combine(folderPath, fileName)));

                return !allExist; // return true if any file is missing
            }
            catch
            {
                return true; // if there's any error (e.g., access denied), assume installation is needed
            }
        }

        private IEnumerable<string> GetThemes()
        {
            var list = InstanceProvider.GetInstance<IBinFileHelper>().ThemesList();
            var text = list.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("IsDarkEnabled"));
            if (!string.IsNullOrEmpty(text))
            {
                return (IsDark = text.Contains("\"IsDarkEnabled\":true")) ? new List<string> { "Dark", "Light" } : new List<string> { "Light", "Dark" };
            }
            else
            {
                IsDark = list?.FirstOrDefault() == "Dark";
            }
            return list;
        }

        public async Task RemoveLocationDataFromTemplate()
        {
            try
            {
                var newtemplateFileManger = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var newtemplateDetails = newtemplateFileManger.Get();
                newtemplateDetails.ForEach(x =>
                {
                    x.ActivitySettings = System.Text.RegularExpressions.Regex.Replace(x.ActivitySettings, ",\"ListLocationModel\":\\[(.*?)\\]", string.Empty)
                    ;
                });
                newtemplateFileManger.Save(newtemplateDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private async Task InstallVcRedistAsync()
        {
            var appFilePath = Path.Combine(ConstantVariable.GetVisualPath(), "VC_redist.x86.exe");

            if (!File.Exists(appFilePath))
            {
                GlobusLogHelper.log.Error("VC++ redistributable file not found after download.");
                return;
            }

            try
            {
                var process = new Process();
                process.StartInfo.FileName = appFilePath;
                process.StartInfo.Arguments = "/quiet /norestart";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                var tcs = new TaskCompletionSource<int>();

                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    tcs.TrySetResult(process.ExitCode);
                    process.Dispose();
                };

                process.Start();

                int exitCode = await tcs.Task;

                if (exitCode == 0 || exitCode == 3010 || exitCode == 1638)
                {
                    GlobusLogHelper.log.Info("VC++ installed successfully.");
                    FileUtilities.DeleteFile(appFilePath);
                }
                else
                {
                    GlobusLogHelper.log.Warn("VC++ did not install successfully. Please install the x86 version manually.");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error($"Exception during VC++ install: {ex.Message}");
            }
        }

        private async void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                bool isClose = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(), String.Format("LangKeyConfirmationToCloseApplication".FromResourceDictionary(), "LangKeySocinator".FromResourceDictionary()), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MessageDialogResult.Affirmative;
                if (isClose)
                {
                    //AccessLog().Wait();
                    await browserManager.RemoveAllBrowser();
                    DominatorHouseCore.Utility.Utilities.KillGecko();
                    Application.Current.Shutdown();
                    Process.GetCurrentProcess().Kill();
                }
                else if (IsCancelFromLicenceValidationState)
                    await FatalErrorDiagnosis();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private async Task IsCheck()
        {
            try
            {
                var key = SocinatorKeyHelper.Key;
                if (key != null && (!string.IsNullOrEmpty(key?.FatalErrorMessage)) && !key.FatalErrorMessage.Contains("SOC-"))
                    key.FatalErrorMessage = AesDecryption.DecryptKey(key.FatalErrorMessage);
                var networks = await UtilityManager.LogIndividualNetworksExceptions(key.FatalErrorMessage);

                if (networks.Count <= 1)
                {
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Application.Current.Shutdown();
                            Process.GetCurrentProcess().Kill();
                        });
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        Process.GetCurrentProcess().Kill();
                    }
                }
                else
                    ShowSubscriptionPopup(PopUpModel = UtilityManager.PopUpModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool _isStartedfirstTime, _isSameKeyUsed = false;
        private async Task FatalErrorDiagnosis()
        {
            string fatalError;
            await UpdateSoftware();

            var key = SocinatorKeyHelper.Key;
            if (key != null)
            {
                _isStartedfirstTime = true;

                if (await DiagnoseFatalError(key.FatalErrorMessage))
                {
                    if (!_isStartedfirstTime)
                        return;
                    var defaultText = string.IsNullOrEmpty(key.FatalErrorMessage) ? "" : key.FatalErrorMessage;
                    var settings = new MetroDialogSettings
                    {
                        DefaultText = string.IsNullOrEmpty(key.FatalErrorMessage) ? "" : key.FatalErrorMessage,
                        AffirmativeButtonText = "LangKeyValidate".FromResourceDictionary()
                    };
                    while (true)
                    {
                        try
                        {
                            fatalError = Dialog.GetInputDialog("LangKeySocinator".FromResourceDictionary(), "LangKeyLicense".FromResourceDictionary(), defaultText, "LangKeyValidate".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                            if (string.IsNullOrEmpty(fatalError))
                            {
                                Application.Current.MainWindow.Close();
                                continue;
                            }
                            if (await IsProcessFatalError(fatalError))
                                // ReSharper disable once RedundantJumpStatement
                                continue;
                            // ReSharper disable once RedundantIfElseBlock
                            //else if (_isStartedfirstTime)
                            break;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
            }
            else
                while (true)
                {
                    try
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "LangKeySocinator".FromResourceDictionary(), "LangKeyLicense".FromResourceDictionary());
                        if (await IsProcessFatalError(fatalError))
                            // ReSharper disable once RedundantJumpStatement
                            continue;
                        break;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

        }

        private async Task UpdateSoftware()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SoftwareType"] == "Others")
                    return;

                var controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "LangKeyCheckingUpdates".FromResourceDictionary(),
                        "LangKeyTakeFewMoments".FromResourceDictionary());
                controller.SetIndeterminate();

                if (await UtilityManager.CheckForNewUpdates())
                {
                    controller.SetProgress(0);
                    bool isInstallUpdates = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(), "LangKeyAnUpdatedVersionAvailable".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MessageDialogResult.Affirmative;
                    if (isInstallUpdates)
                    {
                        controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "LangKeyUpdatingSocinator".FromResourceDictionary(),
                            "LangKeyTakeFewMoments".FromResourceDictionary());
                        controller.SetIndeterminate();

                        if (await UtilityManager.InstallNewUpdates())
                        {
                            Application.Current.Shutdown();
                            Process.Start(Application.ResourceAssembly.Location);
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);
                        }
                    }
                }
                await controller.CloseAsync();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void DialogNotification(InvoiceManager InvoiceManager)
        {
            if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
            {
                FileUtilities.Copy(
                    ConstantVariable.MyAppFolderPath + @"\" + $"{"LangKeySocinator".FromResourceDictionary()}{ConstantVariable.GetIconExtension()}",
                    ConstantVariable.GetSocinatorIcon());
                if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
                    DominatorHouseCore.Utility.Utilities.DownloadSocinatorIcon();
            }
            var npop = new NotificationPopUp(InvoiceManager);
            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetMetroWindow(npop, "Notification");
            dialogWindow.ShowDialog();
        }
        private async Task<bool> DiagnoseFatalError(string fatalError)
        {
            //var controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "LangKeyCheckingLicense".FromResourceDictionary(),
            //    "LangKeyTakeFewMoments".FromResourceDictionary());
            //await Task.Delay(1000);
            //controller.SetIndeterminate();
            IsValidating = true;
            await Task.Delay(TimeSpan.FromSeconds(1));
            _fatalError = fatalError;
            if (!SocinatorKeyHelper.Key.IsUnSubscribed || !SocinatorKeyHelper.Key.IsSubscribed)
            {
                try
                {
                    var InvoiceManager = await UtilityManager.CheckPaymentType(fatalError);
                    if (InvoiceManager != null)
                    {
                        if (!SocinatorKeyHelper.Key.IsUnSubscribed || (!SocinatorKeyHelper.Key.IsSubscribed &&
                            InvoiceManager.NetworkAccessDetails.license_expires.Date <= DateTime.Now.Date))
                        {
                            DialogNotification(InvoiceManager);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            var networks = UtilityManager.Networks.Count == 0 ? await UtilityManager.LogIndividualNetworksExceptions(_fatalError) : UtilityManager.Networks;
            if (networks == null)
            {
                //await controller.CloseAsync();
                IsValidating = false;
                return await DiagnoseFatalError(fatalError);
            }
            if (networks.Count <= 1)
            {
                if (networks.Count == 0)
                    _isSameKeyUsed = true;

                //await controller.CloseAsync();
                IsValidating = false;
                if (!_isStartedfirstTime)
                    await FatalErrorDiagnosis();
                return true;
            }
            _isStartedfirstTime = false;
            _isSameKeyUsed = false;
            IsCancelFromLicenceValidationState = false;
            var fatalErrorHandler = new FatalErrorHandler
            {
                FatalErrorMessage = fatalError,
                FatalErrorAddedDate = DateTime.Now,
                ErrorNetworks = networks,
                IsSubscribed = SocinatorKeyHelper.Key.IsSubscribed,
                IsUnSubscribed = SocinatorKeyHelper.Key.IsUnSubscribed
            };
            SocinatorKeyHelper.SaveKey(fatalErrorHandler);

            FeatureFlags.Check("SocinatorInitializer", SocinatorInitializer);
            IsValidating = false;
            try
            {
                PopUpModel = UtilityManager.PopUpModel;
                PopUpModel.Key = fatalError;
                ShowSubscriptionPopup(PopUpModel);
            }
            catch { }
            return true;
        }
        private void ShowSubscriptionPopup(SubscribtionPopUpModel popUpModel)
        {
            try
            {
                if (popUpModel.IsTrial && popUpModel.expires != null && popUpModel.expires != new DateTime())
                {
                    var diffday = (int)(popUpModel.expires - DateTime.Now).TotalDays;
                    var popupdetails = ConstantVariable.GetPopupdetails();
                    var nextShowDay = -1;
                    if (popupdetails != null && popupdetails.nextTimeToShow != null)
                        nextShowDay = (int)(popupdetails.nextTimeToShow - DateTime.Now).TotalDays;
                    if (((popupdetails == null && diffday <= 3) || (nextShowDay >= 0 && nextShowDay <= 3) || (popupdetails != null && popupdetails.Key != popUpModel.Key)))
                    {
                        if (diffday == 0)
                        {
                            popUpModel.Title = "Warning";
                            popUpModel.Description = "You trial subscription is expiring today!";
                            popUpModel.IsAboutToExpire = true;
                        }
                        else if (diffday >= 1 && diffday <= 3)
                        {
                            popUpModel.Title = "Warning";
                            popUpModel.Description = $"Your trial is about to expire in {diffday} days";
                            popUpModel.IsAboutToExpire = true;
                        }
                        else
                        {
                            popUpModel.Title = "Reminder";
                            popUpModel.Description = $"Your trial is started and valid upto {DateTime.Now.AddDays(diffday)}";
                        }
                        var view = new SubscriptionPopUp(popUpModel);
                        Opacity = 0.5f;
                        //view.Width = popUpModel.IsAboutToExpire ? 500:350;
                        //view.Height = popUpModel.IsAboutToExpire ? 150:100;
                        //var width = (int)Application.Current.MainWindow.ActualWidth - (view.Width +20);
                        //var height = (int)Application.Current.MainWindow.ActualHeight - (view.Height+20);
                        view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        //view.Left = width;
                        //view.Top = height;
                        view.Closing += (s, e) =>
                        {
                            Opacity = 1f;
                        };
                        view.ShowDialog();
                    }
                }
            }
            catch { }
        }
        private async Task<bool> IsProcessFatalError(string fatalError)
        {
            if (!string.IsNullOrEmpty(fatalError) && await DiagnoseFatalError(fatalError))
            {
                if (_isSameKeyUsed)
                    return true;
                return false;
            }
            if (fatalError == null)
            {
                IsCancelFromLicenceValidationState = true;
                if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
            }
            else
            {
                if (Dialog.ShowDialog("LangKeyLicense".FromResourceDictionary(), "LangKeyValidateSocinator".FromResourceDictionary()) == MessageDialogResult.Affirmative)
                    return true;
                if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
            }

            return false;
        }

        private void SocinatorInitializer()
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                 //   CheckMSVCPlusPlusInstalled();
                    FeatureFlags.UpdateFeatures();
                    var modules = InstanceProvider.GetAllInstance<ISocialNetworkModule>();
                    foreach (var socialNetworkModule in modules.Where(a => SocinatorInitialize.IsNetworkAvailable(a.Network)))
                    {
                        var module = socialNetworkModule;
                        if (FeatureFlags.Instance.ContainsKey(module.Network.ToString()))
                        {
                            try
                            {
                                SocinatorInitialize.SocialNetworkRegister(
                                    module.GetNetworkCollectionFactory(Strategies), module.Network);
                                PublisherInitialize.SaveNetworkPublisher(module.GetPublisherCollectionFactory(),
                                    module.Network);
                                AddNetwork(socialNetworkModule.Network);
                            }
                            catch (AggregateException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        Task.Delay(5);
                    }

                    SetActiveNetwork(SocialNetworks.Social);
                });
                ThreadFactory.Instance.Start(() =>
                {
                    _schedulerProxy.AddJob(InitializeJobCores, x => x.ToRunNow());
                });

                ConfigFileManager.ApplyTheme();

                Application.Current.MainWindow.Closed += (o, e) => Process.GetCurrentProcess().Kill();

            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void InitializeJobCores()
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var nextDayTime = DateTime.Now.AddDays(1);

                    //_schedulerProxy.AddJob(InitializeJobCores,
                    //    x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                    //        .AndEvery(1).Days());
                    _schedulerProxy.AddJob(InitializeJobCores,
                        x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1)));
                });

                FeatureFlags.UpdateFeatures();

                Task.Factory.StartNew(() =>
                {
                    #region log deletion and backup Account

                    DirectoryUtilities.DeleteOldLogsFile();
                    DirectoryUtilities.CompressAccountDetails();

                    #endregion

                    #region SoftwareSettings

                    var softwareSetting = InstanceProvider.GetInstance<ISoftwareSettings>();
                    softwareSetting.InitializeOnLoadConfigurations();

                    //  softwareSetting.ActivityManagerInitializer();

                    //softwareSetting.ScheduleAutoUpdation();
                    //if (SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook) != null)
                    //    softwareSetting.ScheduleAdsScraping();

                    #endregion

                });
                Task.Factory.StartNew(() =>
                {


                    #region Publisher

                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                    //PublishScheduler.ScheduleTodaysPublisher();
                    PublishScheduler.UpdateNewGroupList();

                    var publisherPostFetcher = new PublisherPostFetcher();
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(50));
                    publisherPostFetcher.StartFetchingPostData();

                    #endregion

                    var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                    var deletionPostlist =
                        genericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                            .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                });

                #region Commented
                //Parallel.Invoke(() =>
                //                 {
                //                     DirectoryUtilities.DeleteOldLogsFile();
                //                     DirectoryUtilities.CompressAccountDetails();
                //                 },
                //                 () =>
                //                  {
                //                    var softwareSetting = InstanceProvider.GetInstance<ISoftwareSettings>();
                //                    softwareSetting.InitializeOnLoadConfigurations();
                //                    softwareSetting.ActivityManagerInitializer();
                //                    softwareSetting.ScheduleAutoUpdation();
                //                    if (SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook) != null)
                //                        softwareSetting.ScheduleAdsScraping();
                //                },

                //                () =>
                //                {
                //                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                //                    PublishScheduler.ScheduleTodaysPublisher();
                //                    PublishScheduler.UpdateNewGroupList();
                //                },
                //               () =>
                //                {
                //                    var publisherPostFetcher = new PublisherPostFetcher();
                //                    publisherPostFetcher.StartFetchingPostData();
                //                },
                //                () =>
                //                {
                //                    var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                //                    var deletionPostlist =
                //                        genericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                //                            .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                //                    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                //                }); 
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    //_schedulerProxy.AddJob(async () => await IsCheck(),
                    //    x => x.ToRunOnceAt(DateTime.Now.AddHours(1))
                    //        .AndEvery(1).Hours());
                    _schedulerProxy.AddJob(async () => await IsCheck(),
                        x => x.ToRunOnceAt(DateTime.Now.AddDays(1)));
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog();
            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            //try
            //{
            //    ThreadFactory.Instance.Start(() =>
            //    {
            //        _schedulerProxy.AddJob(async () => await AccessLog(),
            //            x => x.WithName("logAct").ToRunOnceAt(DateTime.Now.AddMinutes(1)).AndEvery(1).Days());
            //    });
            //}
            //catch (OperationCanceledException ex)
            //{
            //    ex.DebugLog();
            //}
            //catch (AggregateException ex)
            //{
            //    ex.DebugLog();
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //}
        }

        private void OnTabItems_ItemSelected(object sender, TabItemTemplates itemTemplate)
        {
            if (itemTemplate == null) return;
            if (itemTemplate.Title ==
                _applicationResourceProvider.GetStringResource(ApplicationResourceProvider
                    .LangKeyAccountsActivity))
            {
                InstanceProvider.GetInstance<IDominatorAutoActivityViewModel>().CallRespectiveView(SocialNetworks.Social);
            }

            if (itemTemplate.Title ==
                _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyPublisher))
            {
                PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl =
                    Home.GetSingletonHome();
            }

            if (itemTemplate.Title ==
                _applicationResourceProvider.GetStringResource(ApplicationResourceProvider
                    .LangKeyAccountsManager))
            {
                /* LastControlType will be have value "AccountManager" if last opened UserControl was "Account Manager" itselt, it won't let to change UserControl if "Account Details" was opened. */
                if (AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType == "AccountDetail")
                    return;

                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, Strategies);
            }

            if (itemTemplate.Title ==
                _applicationResourceProvider.GetStringResource(
                    ApplicationResourceProvider.LangKeySociopublisher))
            {
                PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                    PublisherDefaultPage.Instance();
            }
        }

        private void OnAvailableNetworks_ItemSelected(object sender, SocialNetworks? network)
        {
            if (!network.HasValue)
                return;

            TabDock = Dock.Top;
            //if (network == SocialNetworks.Social)
            //    TabDock = Dock.Left;

            // if "Account details" was opened in account manager, then discard all account details changes while switching network 
            var isAccountDetailsOpened = AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType;
            if (isAccountDetailsOpened == "AccountDetail")
            {
                ((AccountDetail)(AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl)).AccountDetailsViewModel.UpdateCurrentDominatorAccountModel();
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType = "AccountManager";
            }

            TabInitialize(network.Value);
        }

        public void AddNetwork(SocialNetworks socialNetwork)
        {
            AvailableNetworks.Add(socialNetwork);
        }

        public void SetActiveNetwork(SocialNetworks social)
        {
            AvailableNetworks.Selected = social;
        }

        public void TabInitialize(SocialNetworks network)
        {
            try
            {
                var tabHandler = SocinatorInitialize.GetSocialLibrary(network).GetNetworkCoreFactory().TabHandlerFactory;
                if (tabHandler == null)
                    return;
                TabItems.Renew(tabHandler.NetworkTabs);
                TabItems.SelectByIndex(0);
                tabHandler.UpdateAccountCustomControl(network);
                SocinatorInitialize.SetAsActiveNetwork(network);
            }
            catch (Exception ex)
            {
                //TabDock = Dock.Left;

                Dialog.ShowDialog("LangKeyFatalError".FromResourceDictionary(),
                    String.Format("LangKeyPurchaseAccessOfNetwork".FromResourceDictionary(), network));
                ex.DebugLog();
            }
        }

        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            var AutoActivityViewModel = InstanceProvider.GetInstance<IDominatorAutoActivityViewModel>();
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                TabItems.SelectByIndex(index);
                AutoActivityViewModel.NewAutoActivityObject(network, selectedAccount);
            }
            else
            {
                TabItems.SelectByIndex(index);
                AutoActivityViewModel.CallRespectiveView(SocialNetworks.Social);
                AutoActivityViewModel.NewAutoActivityObject(network, selectedAccount);
            }
        }

        public void Dispose()
        {
            PerfCounterViewModel?.Dispose();
            AvailableNetworks.ItemSelected -= OnAvailableNetworks_ItemSelected;
            TabItems.ItemSelected -= OnTabItems_ItemSelected;
        }

        public void InstallRequiredDependencies()
        {
           
            Task.Run(async () =>
            {
                try
                {
                    await InstallFFMpeg();
                    var Installed = await InstallDependency();
                    if (Installed)
                        ScheduleActivity.StartScheduling();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
           
        }
    }
}
