using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorUI.FileManagers;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Settings
{
    /// <summary>
    ///     Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        public Setting()
        {
            InitializeComponent();

            SettingsModel = SettingFileManager.GetSettings() ?? SettingsModel;
            MainGrid.DataContext = SettingsModel;
        }

        private SettingsModel SettingsModel { get; } = new SettingsModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (SettingFileManager.SaveSetting(SettingsModel))
                Dialog.ShowDialog(Application.Current.MainWindow, "Success",
                    "Setting sucessfully saved !!");
            var setting = SettingFileManager.GetSettings();
        }
    }
}