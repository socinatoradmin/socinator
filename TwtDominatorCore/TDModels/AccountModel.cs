using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using Newtonsoft.Json;

namespace TwtDominatorCore.TDModels
{
    public class AccountModel
    {
        public AccountModel(DominatorAccountModel dominatorAccountModel)
        {
            _dominatorAccountModel = dominatorAccountModel;
            try
            {
                _dominatorAccountModel.ModulePrivateDetails = JsonConvert.SerializeObject(this);
                CsrfToken = dominatorAccountModel.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")?.Value;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private DominatorAccountModel _dominatorAccountModel { get; }


        public string CsrfToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue();
                return null;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public string postAuthenticityToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue();
                return null;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int FollowersCount
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value) && Regex.IsMatch(Value, @"^\d+$"))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int FollowingCount
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value) && Regex.IsMatch(Value, @"^\d+$"))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int TweetsCount
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value) && Regex.IsMatch(Value, @"^\d+$"))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public bool IsPrivate
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue().Equals("True");
                return false;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public string Email
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue();
                return null;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public bool FollowersUpdated
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue() == "True";
                return false;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public bool FollowingsUpdated
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue() == "True";
                return false;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public bool FeedsUpdated
        {
            get
            {
                if (!string.IsNullOrEmpty(_dominatorAccountModel.GetModulePrivateDetailsValue()))
                    return _dominatorAccountModel.GetModulePrivateDetailsValue() == "True";
                return false;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int LastFollowingsUpdatedTime
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int LastFollowersUpdatedTime
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int LastFeedsUpdatedTime
        {
            get
            {
                var Value = _dominatorAccountModel.GetModulePrivateDetailsValue();
                if (!string.IsNullOrEmpty(Value))
                    return int.Parse(_dominatorAccountModel.GetModulePrivateDetailsValue());
                return 0;
            }
            set => _dominatorAccountModel.SetModulePrivateDetailsValue(value);
        }
    }
}