using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for MediaDownloader.xaml
    /// </summary>
    public partial class MediaDownloader
    {
        public MediaDownloader(IMediaDownloaderViewModel mediaDownloader)
        {
            InitializeComponent();
            MainGrid.DataContext = mediaDownloader;
        }
    }
}
