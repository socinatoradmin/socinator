#region

using System;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class CookieHelper : BindableBase
    {
        private string _name = string.Empty;

        [ProtoMember(1)]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                SetProperty(ref _name, value);
            }
        }

        private string _value = string.Empty;

        [ProtoMember(2)]
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                SetProperty(ref _value, value);
            }
        }

        [ProtoMember(3)] public string Domain { get; set; }


        private DateTime _Expires;

        [ProtoMember(4)]
        public DateTime Expires
        {
            get => _Expires;
            set
            {
                if (_Expires == value)
                    return;
                SetProperty(ref _Expires, value);
            }
        }

        private bool _HttpOnly;

        [ProtoMember(5)]
        public bool HttpOnly
        {
            get => _HttpOnly;
            set
            {
                if (_HttpOnly == value)
                    return;
                SetProperty(ref _HttpOnly, value);
            }
        }

        private bool _Secure = true;

        [ProtoMember(6)]
        public bool Secure
        {
            get => _Secure;
            set
            {
                if (_Secure == value)
                    return;
                SetProperty(ref _Secure, value);
            }
        }
    }
}