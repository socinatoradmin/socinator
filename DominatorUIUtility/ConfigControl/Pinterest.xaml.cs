using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest
    {
        private readonly IOtherConfigFileManager PinterestConfig;

        private Pinterest()
        {
            InitializeComponent();
            PinterestConfig = InstanceProvider.GetInstance<IOtherConfigFileManager>();
            PinterestModel = PinterestConfig.GetOtherConfig<PinterestModel>() ?? PinterestModel;
            MainGrid.DataContext = PinterestModel;
        }

        private PinterestModel PinterestModel { get; } = new PinterestModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (PinterestConfig.SaveOtherConfig(PinterestModel))
                Dialog.ShowDialog("Success", "Pinterest Configuration sucessfully saved !!");
        }
    }
}