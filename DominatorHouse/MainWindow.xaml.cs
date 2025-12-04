#region Namespaces
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.Forms.MessageBox;
using Panel = System.Windows.Controls.Panel;

#endregion

namespace Socinator
{
    public interface IMainWindow
    {

    }
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :Window, IMainWindow
    {
        private bool IsClickedFromMainWindow { get; set; } = true;
        public IMainViewModel mainViewModel {  get; set; }
        private Rect _restoreBounds;
        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);
                Application.Current.MainWindow = this;
                InitializeComponent();

                SocinatorInitialize.LogInitializer(this);

                var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
                var activeScreen = Screen.FromHandle(interopHelper.Handle);
                var IsDebugMode = false;
#if DEBUG
                IsDebugMode = true;
#endif
                if (!IsDebugMode && !IsRunningAsAdmin() && IsNeedToInstallDependency(AppDomain.CurrentDomain.BaseDirectory))
                {
                    Process elevatedProcess = null;
                    try
                    {
                        var exeName = Process.GetCurrentProcess().MainModule.FileName;
                        var startInfo = new ProcessStartInfo(exeName)
                        {
                            UseShellExecute = true,
                            Verb = "runas" // This triggers the UAC prompt
                        };
                        Application.Current.Shutdown();
                        elevatedProcess  = Process.Start(startInfo);
                        Process.GetCurrentProcess().Kill();
                        Environment.Exit(0);
                    }
                    catch(Exception e)
                    {
                        if (elevatedProcess != null && !elevatedProcess.HasExited)
                        {
                            try
                            {
                                elevatedProcess.Kill();
                            }
                            catch { /* Optional: Log or ignore */ }
                        }
                        MessageBox.Show("This application requires administrator privileges to run.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                mainViewModel = InstanceProvider.GetInstance<IMainViewModel>();

                mainViewModel.ScreenResolution = new KeyValuePair<int, int>
                    (activeScreen.WorkingArea.Width, activeScreen.WorkingArea.Height);

                SocinatorWindow.DataContext = mainViewModel;
                Loaded +=(o, e) =>
                {
                    GlobusLogHelper.log.Info(String.Format("LangKeyWelcomeToApplication".FromResourceDictionary(), ConstantVariable.ApplicationName));
                    mainViewModel.InstallRequiredDependencies();
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public bool IsRunningAsAdmin()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception)
            {
                return false; // If there's an error, assume not running as admin
            }
        }
        public bool IsNeedToInstallDependency(string folderPath)
        {
            try
            {
                string[] filesCheck = {
                    "AutoItX3.dll", "AutoItX3_x64.dll", "chrome_100_percent.pak", "chrome_200_percent.pak",
                    "chrome_elf.dll", "d3dcompiler_47.dll", "icudtl.dat", "libcef.dll", "libEGL.dll",
                    "libGLESv2.dll", "resources.pak", "snapshot_blob.bin","v8_context_snapshot.bin", "vk_swiftshader.dll",
                    "vk_swiftshader_icd.json", "vulkan-1.dll","ffmpeg.exe"
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
        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            statusbar.IsEnabled = false;
            if (!IsClickedFromMainWindow) return;
            var dialog = new Dialog();
                
            var activityLogWindow = dialog.GetMetroWindow(Logger, "LangKeyActivityLog".FromResourceDictionary());

            IsClickedFromMainWindow = false;
            activityLogWindow.Closing += (senders, events) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // Safely detach from old parent before re-adding
                        if (Logger.Parent is Panel oldParent)
                            oldParent.Children.Remove(Logger);
                        else if (Logger.Parent is ContentControl contentParent)
                            contentParent.Content = null;

                        Grid.SetRow(Logger, 2);
                        MainGridContainer.Children.Add(Logger);

                        Logger.Children.Remove(RootLayout);
                        Logger.Children.Add(RootLayout);

                        MainGridContainer.RowDefinitions[2].Height = new GridLength(200);
                        IsClickedFromMainWindow = true;
                        statusbar.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            };
            MainGridContainer.RowDefinitions[2].Height = new GridLength(0);
            MainGridContainer.Children.Remove(Logger);
            activityLogWindow.Show();
        }
        private void ChangeThemes(string Selected,string ThemesName,string ColorName,string ThemeString)
        {
            try
            {
                switch (Selected)
                {
                    case "Light":
                        {
                            Application.Current.Resources["LoggerListBackground"] = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            Application.Current.Resources["TabBorderSolidBrush"] = new SolidColorBrush(Color.FromRgb(15, 52, 87));
                            Application.Current.Resources["CampaignShadow"] = Color.FromRgb(147,180,201);
                            Application.Current.Resources["CampaignBGColor"] =
                                new SolidColorBrush(Color.FromRgb(200,215,249));
                            Application.Current.Resources["CampaignTextBgColor"] =
                                new SolidColorBrush(Color.FromRgb(24, 4, 67));
                            Application.Current.Resources["UserControlBackgroundBrush"] =
                                new SolidColorBrush(
                                    Color.FromRgb(255, 255, 255)); // White
                            Application.Current.Resources["SelectedTabBorderBrush"] =
                                new SolidColorBrush(
                                    Color.FromRgb(240, 248, 255)); //Black
                            Application.Current.Resources["TextColorBrushAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(240, 248, 255)); // Pure Black
                            Application.Current.Resources["IconFillBrushAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(1, 0, 0)); // Black
                            Application.Current.Resources["TextColorBrushAccordingTheme1"] =
                                new SolidColorBrush(
                                    Color.FromRgb(35, 49, 64)); // #233140
                            Application.Current.Resources["ListItemsMouseHoverColorAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(83,130,94)); // LightBlue (Much Lighter)
                            Application.Current.Resources["GreenColorAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(0, 128, 0)); // Green

                        }
                        break;

                    case "Dark":
                        {
                            //GreenColorAccordingTheme
                            ThemeString = "Dark\r\nLight";
                            Application.Current.Resources["LoggerListBackground"] = new SolidColorBrush(Colors.Transparent);
                            Application.Current.Resources["TabBorderSolidBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            Application.Current.Resources["CampaignShadow"] = Color.FromRgb(200, 215, 249);
                            Application.Current.Resources["CampaignTextBgColor"] =
                                new SolidColorBrush(Color.FromRgb(224, 224, 224));
                            Application.Current.Resources["CampaignBGColor"] =
                                new SolidColorBrush(Color.FromRgb(45, 128, 138));
                            Application.Current.Resources["UserControlBackgroundBrush"] =
                                new SolidColorBrush(
                                    Color.FromRgb(2,56, 81)); // Black
                            Application.Current.Resources["SelectedTabBorderBrush"] =
                                new SolidColorBrush(
                                    Color.FromRgb(37, 37, 41)); // Black
                            Application.Current.Resources["TextColorBrushAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(255, 255, 255)); // White
                            Application.Current.Resources["IconFillBrushAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(6, 82, 101)); // Dark Blue
                            Application.Current.Resources["TextColorBrushAccordingTheme1"] =
                                new SolidColorBrush(
                                    Color.FromRgb(255, 255, 255)); // Teal(1,166,163)
                            Application.Current.Resources["ListItemsMouseHoverColorAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(47, 79, 79)); // DarkSlateGrey
                            Application.Current.Resources["GreenColorAccordingTheme"] =
                                new SolidColorBrush(
                                    Color.FromRgb(144, 238, 144)); // LightGreen

                        }
                        break;
                }

                var newAccent = ThemeManager.GetAccent(ColorName);
                var newAppTheme = ThemeManager.GetAppTheme(ThemesName);
                ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);

                InstanceProvider.GetInstance<IBinFileHelper>().SetTheme(ThemeString);
            }
            catch { }
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                OuterBorder.Margin = new Thickness(3);
            }
            else
            {
                OuterBorder.Margin = new Thickness(10);
            }
        }
        private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var setThemeString = "Light\r\nDark";
                var selected = (sender as ComboBox)?.SelectedItem as string;

                string themeName = $"Base{selected}";
                string colorName = selected == "Light" ? "PrussianBlue" : "Teal";
                ChangeThemes(selected, themeName, colorName, setThemeString);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var setThemeString = "Light\r\nDark";
                var selectedValue = (sender as ToggleButton)?.IsChecked;
                var selected = (bool)selectedValue ? "Dark" : "Light";
                string themeName = $"Base{selected}";
                string colorName = selected == "Light" ? "PrussianBlue" : "Teal";
                ChangeThemes(selected, themeName, colorName, setThemeString);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            OnChecked(sender, e);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxRestore_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaxRestorePath.Data = Geometry.Parse("M 0 0 L 0 10 L 10 10 L 10 0 Z"); // Maximize icon
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaxRestorePath.Data = Geometry.Parse("M 1 3 L 1 11 L 9 11 L 9 3 Z M 3 1 L 11 1 L 11 9 L 9 9 M 3 1 L 3 3 L 9 3"); // Restore icon
            }
        }

        private void OnLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}