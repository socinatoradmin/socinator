#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LocationModel : BindableBase
    {
        private string _countryName = string.Empty;
        private string _cityName = string.Empty;
        private bool _iSelected;

        [ProtoMember(1)]
        public string CountryName
        {
            get => _countryName;
            set => SetProperty(ref _countryName, value, nameof(CountryName));
        }

        [ProtoMember(2)]
        public string CityName
        {
            get => _cityName;
            set => SetProperty(ref _cityName, value, nameof(CityName));
        }

        [ProtoMember(3)]
        public bool IsSelected
        {
            get => _iSelected;
            set => SetProperty(ref _iSelected, value, nameof(IsSelected));
        }
    }
}