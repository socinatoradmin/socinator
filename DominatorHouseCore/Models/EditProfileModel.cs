#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class EditProfileModel : BindableBase
    {
        private string _profilePicPath;
        private string _fullname;
        private string _username;
        private string _externalUrl;
        private string _bio;
        private string _email;
        private string _phoneNumber;
        private bool _isMaleChecked;
        private bool _isFemaleChecked;
        private bool _isNonSpecifiedChecked;
        private bool _IsCheckedPrivateInfo;

        [ProtoMember(1)]
        public string ProfilePicPath
        {
            get => _profilePicPath;
            set
            {
                _profilePicPath = value;
                SetProperty(ref _profilePicPath, value);
            }
        }

        [ProtoMember(2)]
        public string Fullname
        {
            get => _fullname;
            set
            {
                SetProperty(ref _fullname, value);
            }
        }

        [ProtoMember(3)]
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
            }
        }

        [ProtoMember(4)]
        public string ExternalUrl
        {
            get => _externalUrl;
            set
            {
                SetProperty(ref _externalUrl, value);
            }
        }

        [ProtoMember(5)]
        public string Bio
        {
            get => _bio;
            set
            {
                SetProperty(ref _bio, value);
            }
        }

        [ProtoMember(6)]
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
            }
        }

        [ProtoMember(7)]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                SetProperty(ref _phoneNumber, value);
            }
        }

        [ProtoMember(8)]
        public bool IsMaleChecked
        {
            get => _isMaleChecked;
            set
            {
                SetProperty(ref _isMaleChecked, value);
            }
        }

        [ProtoMember(9)]
        public bool IsFemaleChecked
        {
            get => _isFemaleChecked;
            set
            {
                SetProperty(ref _isFemaleChecked, value);
            }
        }

        [ProtoMember(10)]
        public bool IsNonSpecifiedChecked
        {
            get => _isNonSpecifiedChecked;
            set
            {
                SetProperty(ref _isNonSpecifiedChecked, value);
            }
        }
        [ProtoMember(11)]
        public bool IsCheckedPrivateInfo
        {
            get => _IsCheckedPrivateInfo;
            set
            {
                SetProperty(ref _IsCheckedPrivateInfo, value);
            }
        }
    }
}