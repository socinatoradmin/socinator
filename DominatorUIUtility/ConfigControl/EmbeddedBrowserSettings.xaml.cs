using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for EmbeddedBrowserSettings.xaml
    /// </summary>
    public partial class EmbeddedBrowserSettings
    {
        private readonly IOtherConfigFileManager embeddedBrowserSettings;

        private EmbeddedBrowserSettings()
        {
            InitializeComponent();
            embeddedBrowserSettings = InstanceProvider.GetInstance<IOtherConfigFileManager>();
            EmbeddedBrowserSettingsModel = embeddedBrowserSettings.GetOtherConfig<EmbeddedBrowserSettingsModel>() ??
                                           EmbeddedBrowserSettingsModel;
            MainGrid.DataContext = EmbeddedBrowserSettingsModel;
        }

        private EmbeddedBrowserSettingsModel EmbeddedBrowserSettingsModel { get; } = new EmbeddedBrowserSettingsModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (embeddedBrowserSettings.SaveOtherConfig(EmbeddedBrowserSettingsModel))
                Dialog.ShowDialog("Success", "Embedded Browser Settings sucessfully saved !!");
        }
    }
}