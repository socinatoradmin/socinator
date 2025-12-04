using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram
    {
        private readonly IOtherConfigFileManager InstagramConfig;

        private Instagram()
        {
            InitializeComponent();
            InstagramConfig = InstanceProvider.GetInstance<IOtherConfigFileManager>();
            InstagramModel = InstagramConfig.GetOtherConfig<InstagramModel>() ?? InstagramModel;
            MainGrid.DataContext = InstagramModel;
        }

        private InstagramModel InstagramModel { get; } = new InstagramModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstagramConfig.SaveOtherConfig(InstagramModel))
                Dialog.ShowDialog("Success", "Instagram Configuration sucessfully saved !!");
        }
    }
}