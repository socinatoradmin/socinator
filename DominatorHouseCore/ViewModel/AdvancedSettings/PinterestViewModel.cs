#region

using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class PinterestViewModel : BindableBase
    {
        private PinterestModel _pinterestModel = new PinterestModel();

        public PinterestModel PinterestModel
        {
            get => _pinterestModel;
            set
            {
                if (_pinterestModel == value)
                    return;
                SetProperty(ref _pinterestModel, value);
            }
        }
    }
}