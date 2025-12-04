#region

using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class GeneralViewModel : BindableBase
    {
        private GeneralModel _generalModel = new GeneralModel();

        public GeneralModel GeneralModel
        {
            get => _generalModel;
            set
            {
                if (_generalModel == value)
                    return;
                SetProperty(ref _generalModel, value);
            }
        }
    }
}