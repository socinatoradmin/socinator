using System.Windows.Controls;

namespace DominatorUIUtility.Views.Publisher.AdvancedOptions
{
    /// <summary>
    ///     Interaction logic for AdvancedOption.xaml
    /// </summary>
    public partial class AdvancedOption : UserControl
    {
        public AdvancedOption()
        {
            InitializeComponent();
            var ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            MainGrid.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel;
        }
    }
}