using AutoMapper;
using DominatorHouse;
using DominatorHouse.AutoMapping;
using DominatorHouse.Utilities.Facebook;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Module;
using DominatorUIUtility.ViewModel.Startup;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.Interception;
using MessageBox = System.Windows.MessageBox;

// ReSharper disable once CheckNamespace
// ReSharper disable once IdentifierTypo
namespace Socinator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private SocinatorSplashScreen splashScreen = null;
        public void CheckAllForExpand(object sender, RoutedEventArgs e)
        {
            HeaderHelper.UpdateToggleButtonInCampaignMode?.Invoke();
            HeaderHelper.UpdateToggleButtonInAccountActivityMode?.Invoke();
        }
        protected override async void OnInitialized()
        {
            base.OnInitialized();

            splashScreen = new SocinatorSplashScreen();
            splashScreen.Show();
            await Task.Yield(); // Ensure splash actually renders
            // Run initialization asynchronously
            await Task.Run(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
                InitializeAutoMapper();
            });
            await Task.Delay(TimeSpan.FromSeconds(4));
            // Now construct shell
            await Current.Dispatcher.InvokeAsync(() =>
            {
                MahApps.Metro.ThemeManager.AddAccent(
                    "PrussianBlue",
                    new Uri("pack://application:,,,/DominatorUIUtility;component/Themes/PrussianBlue.xaml"));
                var shell = Container.Resolve<MainWindow>(); // Still blocking but happens later
                if (splashScreen != null)
                    splashScreen?.Close();
                shell.Show();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        protected override Window CreateShell()
        {
            return null;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                ex.DebugLog();
            }
            catch (StackOverflowException ex)
            {
                GC.Collect();
            }
            catch (Exception ex)
            {
                
            }
        }


        // Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (IsAlreadyRunning())
            {
                MessageBox.Show("LangKeySocinatorAlreadyRunning".FromResourceDictionary(), "LangKeyWarning".FromResourceDictionary(), MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }
            var container = containerRegistry.GetContainer();
            container.AddNewExtension<Interception>();
            container.AddNewExtension<CoreUnityExtension>();
            Task.Run(() => container.LoadConfiguration());
            StartupBaseViewModel.GetFaceBookActivity = activityType => new FacebookActivity().GetActivity(activityType);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<UiModule>();
        }
        private void InitializeAutoMapper()
        {
            var moduleProfiles = InstanceProvider.GetAllInstance<Profile>();
            AutoMapperConfiguration.Init(moduleProfiles);
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        bool IsAlreadyRunning()
        {
            return CheckByProcess();
        }

        bool CheckByProcess()
        {
            try
            {
                var existed = false;
                var itemCount = 0;

                foreach (var item in System.Diagnostics.Process.GetProcesses())
                {
                    try
                    {
                        if (item.ProcessName != "Socinator")
                            continue;
                        itemCount++;
                        if (itemCount <= 1) continue;
                        existed = true;
                        break;
                    }
                    catch
                    { /* ignored*/ }
                }
                
                return existed;
            }
            catch
            { return false; }
        }

        private void OnSocinatorStartup(object sender, StartupEventArgs e)
        {
            try
            {
                UpdateOldVersionDirectory();
            }
            catch { }
        }
        private void UpdateOldVersionDirectory()
        {
            try
            {
                var SourceDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConstantVariable.ApplicationName);
                var TargetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConstantVariable.AssemblyName);
                if(Directory.Exists(SourceDirectory) && !Directory.Exists(TargetDirectory))
                {
                    Directory.Move(SourceDirectory, TargetDirectory);
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    var startInfo = new ProcessStartInfo(exeName)
                    {
                        UseShellExecute = true,
                    };
                    Current.Shutdown();
                    Process.Start(startInfo);
                    Process.GetCurrentProcess().Kill();
                    Environment.Exit(0);
                }
            }
            catch { }
        }
    }
}
