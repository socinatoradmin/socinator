#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class IdNameBinderModel : BindableBase
    {
        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}