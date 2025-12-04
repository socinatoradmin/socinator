#region

using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class InstagramViewModel : BindableBase
    {
        private InstagramModel _instagramModel = new InstagramModel();

        public InstagramModel InstagramModel
        {
            get => _instagramModel;
            set
            {
                if (_instagramModel == value)
                    return;
                SetProperty(ref _instagramModel, value);
            }
        }
    }
}