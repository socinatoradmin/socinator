using System.Windows;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for OtherConfig.xaml
    /// </summary>
    public partial class OtherConfig
    {
        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherConfigProperty =
            DependencyProperty.Register("OtherConfigFilter", typeof(OtherConfigModel), typeof(OtherConfig),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        private readonly Dialog dialog = new Dialog();
        public MultiMessage MultiMessageForUserHasNotReplied = new MultiMessage();
        public MultiMessage MultiMessageForUserHasReplied = new MultiMessage();

        public OtherConfig()
        {
            InitializeComponent();
            OtherConfigFilter = new OtherConfigModel();
            MainGrid.DataContext = this;
        }

        public OtherConfigModel OtherConfigFilter
        {
            get => (OtherConfigModel) GetValue(OtherConfigProperty);
            set => SetValue(OtherConfigProperty, value);
        }

        private void BtnSendIfUserHasReplied_OnClick(object sender, RoutedEventArgs e)
        {
            var win = dialog.GetMetroWindow(MultiMessageForUserHasReplied, "Messages for user who has replied");
            win.Show();
        }

        private void BtnSendIfUserHasNotReplied_OnClick(object sender, RoutedEventArgs e)
        {
            var win = dialog.GetMetroWindow(MultiMessageForUserHasNotReplied, "Messages for user who has not replied");
            win.Show();
        }
    }
}