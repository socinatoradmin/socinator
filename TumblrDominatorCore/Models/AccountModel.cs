using DominatorHouseCore;
using DominatorHouseCore.Models;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    [ProtoContract]
    public class AccountModel
    {
        public Dictionary<string, Dictionary<string, string>> Experiments =
            new Dictionary<string, Dictionary<string, string>>();

        public AccountModel(DominatorAccountModel dominatorAccountModel)
        {
            DominatorAccountModel = dominatorAccountModel;

            try
            {
                // this._dominatorAccountModel.ModulePrivateDetails = (Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public DominatorAccountModel DominatorAccountModel { get; set; }

        public List<TumblrUser> LstFollowings { get; set; }

        public List<TumblrPost> LstPost { get; set; }

        public string TumblrFormKey { get; set; }


        public int BlogCount { get; set; }

        public int FollowersCount
        {
            get
            {
                var value = DominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
            set => DominatorAccountModel?.SetModulePrivateDetailsValue(value);
        }

        public int FollowingCount
        {
            get
            {
                var value = DominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
            set => DominatorAccountModel?.SetModulePrivateDetailsValue(value);
        }

        public int PostsCount
        {
            get
            {
                var value = DominatorAccountModel?.GetModulePrivateDetailsValue();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
            set => DominatorAccountModel?.SetModulePrivateDetailsValue(value);
        }

        //public List<InstagramUser> LstFollowings { get; set; } = new List<InstagramUser>();

        //public List<UserInfoDetailed> LstFollowers { get; set; } = new List<UserInfoDetailed>();
    }
}