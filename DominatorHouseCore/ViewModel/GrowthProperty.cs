#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class GrowthProperty : BindableBase
    {
        private string _propertyName;
        private int _propertyValue;
        private bool _isChecked = true;

        public string PropertyName
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value);
        }

        public int PropertyValue
        {
            get => _propertyValue;
            set => SetProperty(ref _propertyValue, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}