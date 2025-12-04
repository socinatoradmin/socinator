using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Post.xaml
    /// </summary>
    public partial class Post : UserControl
    {
        public Post(IPostViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}