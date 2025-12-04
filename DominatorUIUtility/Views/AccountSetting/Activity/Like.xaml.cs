using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Like.xaml
    /// </summary>
    public partial class Like : UserControl
    {
        public Like(ILikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}