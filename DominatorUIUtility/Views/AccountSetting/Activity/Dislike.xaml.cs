using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Dislike.xaml
    /// </summary>
    public partial class Dislike : UserControl
    {
        public Dislike(IDislikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}