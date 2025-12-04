using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    ///     Interaction logic for Twitter.xaml
    /// </summary>
    public partial class Twitter
    {
        private readonly IOtherConfigFileManager _otherConfigFileManager;

        private Twitter()
        {
            InitializeComponent();
            _otherConfigFileManager = InstanceProvider.GetInstance<IOtherConfigFileManager>();
            TwitterModel = _otherConfigFileManager.GetOtherConfig<TwitterModel>() ?? TwitterModel;
            MainGrid.DataContext = TwitterModel;
        }

        private TwitterModel TwitterModel { get; } = new TwitterModel();

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (_otherConfigFileManager.SaveOtherConfig(TwitterModel))
                Dialog.ShowDialog("Success", "Twitter Configuration sucessfully saved !!");
        }
    }
}