using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherSharePostViewModel : BindableBase
    {
        private SharePostModel _sharePostModel = new SharePostModel();

        public PublisherSharePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            SharePostModel = tabItemsControl.SharePostModel;
        }

        public SharePostModel SharePostModel
        {
            get => _sharePostModel;
            set
            {
                if (value == _sharePostModel)
                    return;
                SetProperty(ref _sharePostModel, value);
            }
        }
    }
}