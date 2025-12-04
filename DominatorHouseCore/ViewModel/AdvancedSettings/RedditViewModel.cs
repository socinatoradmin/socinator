#region

using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class RedditViewModel : BindableBase
    {
        private RedditModel _redditModel = new RedditModel();

        public RedditModel RedditModel
        {
            get => _redditModel;
            set
            {
                if (_redditModel == value)
                    return;
                SetProperty(ref _redditModel, value);
            }
        }
    }
}