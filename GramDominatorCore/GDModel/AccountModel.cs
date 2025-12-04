using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class AccountModel
    {
        private DominatorAccountModel _dominatorAccountModel { get; }

        public AccountModel(DominatorAccountModel dominatorAccountModel)
        {
            _dominatorAccountModel = dominatorAccountModel;


            _dominatorAccountModel.ModulePrivateDetails = (Newtonsoft.Json.JsonConvert.SerializeObject(this));

        }

        [ProtoMember(23)]
        public string CsrfToken
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(24)]
        public string Uuid
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(25)]
        public string Device_Id
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetMobileDeviceId() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        // public Dictionary<string, Dictionary<string, string>> Experiments = new Dictionary<string, Dictionary<string, string>>();
        [ProtoMember(26)]
        public string PhoneId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(27)]
        public string Id
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(28)]
        public string AdId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(29)]
        public string Guid
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }


        [ProtoMember(30)]
        public string GoogleId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(31)]
        public string AttributionId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(32)]
        public string FamilyId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(33)]
        public string PigeonSessionId
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? Utilities.GetGuid() : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(34)]
        public string AuthorizationHeader
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? "" : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(35)]
        public string MidHeader
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? "" : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(36)]
        public string AccountVerifyType
        {
            get
            {
                return string.IsNullOrEmpty(_dominatorAccountModel?.GetModulePrivateDetailsValue()) ? "" : _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        [ProtoMember(37)]
        public string WwwClaim
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(38)]
        public string DsUserId
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        [ProtoMember(39)]
        public string WaterfallId
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string RankToken
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        //public int LastExperiments
        //{
        //    get
        //    {
        //        string value = _dominatorAccountModel?.GetModulePrivateDetailsValue();
        //        return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);


        //    }
        //    set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        //}

        public int FollowersCount
        {
            get
            {
                string value = _dominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);

            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        public int FollowingCount
        {
            get
            {
                string value = _dominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);

            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        public int PostsCount
        {
            get
            {
                string value = _dominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);

            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        //bloks_action
        public string blockAction
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        //perf_logging_id
        public string PerfLoggingId
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }

        public string CountryName
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string CountryCode
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string codeSubmittingtype
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string apiUrl
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string challengeContext
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public string AccountStatus
        {
            get
            {
                return _dominatorAccountModel?.GetModulePrivateDetailsValue();
            }
            set { _dominatorAccountModel?.SetModulePrivateDetailsValue(value); }
        }
        public List<InstagramUser> LstFollowings { get; set; } = new List<InstagramUser>();

        public List<UserInfoDetailed> LstUserInfoDetailed { get; set; } = new List<UserInfoDetailed>();

        public List<InstagramUser> Lst_Followers { get; set; } = new List<InstagramUser>();

        public string publicKey { get; set; }
        public string publicId { get; set; }
        public string EncPwd { get; set; }
        public string UsernameTextId { get; set; }
        public string PasswordTextId { get; set; }
        public string MarkerId { get; set; }
        public string ScreenId { get; set; }
        public string ComponentId { get; set; }
        public string TypeAheadId { get; set; }
        public string TextInputId { get; set; }
        public string Fdid { get; set; }
        public string TextInstanceId { get; set; }
        public string InstanceId { get; set; }
    }

}