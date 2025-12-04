#region

using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class TwitterViewModel : BindableBase
    {
        private TwitterModel _twitterModel = new TwitterModel();

        public TwitterModel TwitterModel
        {
            get => _twitterModel;
            set
            {
                if (_twitterModel == value)
                    return;
                SetProperty(ref _twitterModel, value);
            }
        }
    }
}