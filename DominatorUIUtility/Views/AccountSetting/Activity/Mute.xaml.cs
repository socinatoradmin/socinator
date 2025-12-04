using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Mute.xaml
    /// </summary>
    public partial class Mute : UserControl
    {
        public Mute(IMuteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}