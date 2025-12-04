using System.Collections.ObjectModel;
using System.Windows;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for ChatMediaContainer.xaml
    /// </summary>
    public partial class ChatMediaContainer
    {
        public static readonly DependencyProperty ListMediaUrlsProperty =
            DependencyProperty.Register("ListMediaUrls", typeof(ObservableCollection<string>),
                typeof(ChatMediaContainer), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public ChatMediaContainer()
        {
            InitializeComponent();
        }

        public ObservableCollection<string> ListMediaUrls
        {
            get => (ObservableCollection<string>) GetValue(ListMediaUrlsProperty);
            set => SetValue(ListMediaUrlsProperty, value);
        }
    }
}