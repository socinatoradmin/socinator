using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Unfriend.xaml
    /// </summary>
    public partial class Unfriend : UserControl
    {
        private IUnfriendViewModel _viewModel;

        public Unfriend(IUnfriendViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }
        //private void TypeChecked(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.TypeCount++;
        //}

        //private void TypeUnChecked(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.TypeCount--;
        //}

        //private void Source_UnChecked(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.Count--;
        //}

        //private void Source_Checked(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.Count++;
        //}

        //private void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.LstFilterText = Regex.Split(_viewModel.FilterText, "\r\n").ToList();
        //}
    }
}