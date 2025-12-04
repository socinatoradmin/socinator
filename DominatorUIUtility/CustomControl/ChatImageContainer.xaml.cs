using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for ChatImageContainer.xaml
    /// </summary>
    public partial class ChatImageContainer
    {
        public static readonly DependencyProperty list_SelectedImagesProperty =
            DependencyProperty.Register("List_SelectedImages", typeof(ObservableCollection<string>),
                typeof(ChatImageContainer), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(ChatImageContainer),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(ChatImageContainer),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty ContolBackgroundProperty =
            DependencyProperty.Register("ContolBackground", typeof(Brush), typeof(ChatImageContainer),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(int), typeof(ChatImageContainer),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(int), typeof(ChatImageContainer),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public ChatImageContainer()
        {
            InitializeComponent();
            ImageGrid.DataContext = this;
        }

        public ObservableCollection<string> List_SelectedImages
        {
            get => (ObservableCollection<string>) GetValue(list_SelectedImagesProperty);
            set => SetValue(list_SelectedImagesProperty, value);
        }

        public int ImageWidth
        {
            get => (int) GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public int ImageHeight
        {
            get => (int) GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }


        public Brush ContolBackground
        {
            get => (Brush) GetValue(ContolBackgroundProperty);
            set => SetValue(ContolBackgroundProperty, value);
        }

        public int RadiusX
        {
            get => (int) GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        public int RadiusY
        {
            get => (int) GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }

        private void RemoveImageFromList(object sender, RoutedEventArgs e)
        {
            var currentImage = ((Button) sender).DataContext;
            List_SelectedImages.Remove(currentImage.ToString());
        }
    }
}