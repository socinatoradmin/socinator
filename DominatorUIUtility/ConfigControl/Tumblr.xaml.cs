using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for Tumblr.xaml
    /// </summary>
    public partial class Tumblr
    {
        private readonly IOtherConfigFileManager _otherConfigFileManager;

        public Tumblr()
        {
            InitializeComponent();
            _otherConfigFileManager = InstanceProvider.GetInstance<IOtherConfigFileManager>();
            TumblrModel = _otherConfigFileManager.GetOtherConfig<TumblrModel>() ?? TumblrModel;
            MainGrid.DataContext = TumblrModel;
        }

        private TumblrModel TumblrModel { get; } = new TumblrModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (_otherConfigFileManager.SaveOtherConfig(TumblrModel))
                Dialog.ShowDialog("Success", "Tumblr Configuration sucessfully saved !!");
        }
    }
}