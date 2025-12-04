using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook
    {
        private readonly IFBFileManager fbFilemanager;

        private Facebook()
        {
            InitializeComponent();
            fbFilemanager = InstanceProvider.GetInstance<IFBFileManager>();
            ConfigFacebookModel = fbFilemanager.GetFacebookConfig() ?? ConfigFacebookModel;
            MainGrid.DataContext = ConfigFacebookModel;
        }

        private ConfigFacebookModel ConfigFacebookModel { get; } = new ConfigFacebookModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (fbFilemanager.SaveFacebookConfig(ConfigFacebookModel))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyFacebookConfigurationSaved".FromResourceDictionary());
        }
    }
}