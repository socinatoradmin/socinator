using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    public class AccountModel
    {
        public AccountModel(DominatorAccountModel dominatorAccountModel)
        {
            DominatorAccountModel = dominatorAccountModel;
            try
            {
                DominatorAccountModel.ModulePrivateDetails = JsonConvert.SerializeObject(this);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private DominatorAccountModel DominatorAccountModel { get; }

        [ProtoMember(1)]
        public string CsrfToken
        {
            get
            {
                try
                {
                    return DominatorAccountModel.GetModulePrivateDetailsValue();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return null;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public string ModulePrivateDetails { get; set; }

        public string UserName
        {
            get
            {
                try
                {
                    return DominatorAccountModel.GetModulePrivateDetailsValue();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return null;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public string AccountId
        {
            get
            {
                try
                {
                    return DominatorAccountModel.GetModulePrivateDetailsValue();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return null;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int FollowersCount
        {
            get
            {
                try
                {
                    return int.Parse(DominatorAccountModel.GetModulePrivateDetailsValue());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return 0;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int FollowingCount
        {
            get
            {
                try
                {
                    return int.Parse(DominatorAccountModel.GetModulePrivateDetailsValue());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return 0;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int BoardCount
        {
            get
            {
                try
                {
                    return int.Parse(DominatorAccountModel.GetModulePrivateDetailsValue());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return 0;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public int PinsCount
        {
            get
            {
                try
                {
                    return int.Parse(DominatorAccountModel.GetModulePrivateDetailsValue());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return 0;
                }
            }
            set => DominatorAccountModel.SetModulePrivateDetailsValue(value);
        }

        public List<PinterestUser> LstFollowings { get; set; } = new List<PinterestUser>();

        public List<UserInfoDetailed> LstFollowers { get; set; } = new List<UserInfoDetailed>();
    }
}