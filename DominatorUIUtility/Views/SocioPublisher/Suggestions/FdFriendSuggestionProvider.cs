using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.Views.SocioPublisher.Suggestions
{
    public class FdFriendSuggestionProvider : ISuggestionProvider
    {
        public FdFriendSuggestionProvider()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            ListOfAccount = accountsFileManager.GetAll(SocialNetworks.Facebook).Select(i =>
                new KeyValuePair<string, string>(i.AccountBaseModel.UserName, i.AccountBaseModel.AccountId)).ToList();
            ListOfMacros.AddRange(ConstantVariable.FdMacros);
            ListOfAccount.ForEach(x => ListOfMacros.Add(new SocinatorIntellisenseModel
                {Key = "{" + x.Key + ":", Value = "{" + x.Key + ":"}));
        }


        public List<SocinatorIntellisenseModel> ListOfMacros { get; set; } = new List<SocinatorIntellisenseModel>();

        public IEnumerable<KeyValuePair<string, string>> ListOfAccount { get; set; }


        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            if (!filter.StartsWith("{") && !filter.EndsWith("}"))
                return null;

            var accountUsername = Utilities.GetBetween(filter, "{", ":").Trim();

            var accountId = ListOfAccount.FirstOrDefault(x => x.Key == accountUsername).Value;

            if (!string.IsNullOrEmpty(accountId))
                try
                {
                    var accountsDetailsSelector = SocinatorInitialize
                        .GetSocialLibrary(SocialNetworks.Facebook)
                        .GetNetworkCoreFactory().AccountDetailsSelectors;

                    //var friendsName = accountsDetailsSelector.GetFriendshipNames(accountId, accountUsername);

                    //friendsName.Result.ForEach(x =>
                    //    ListOfMacros.Add(new SocinatorIntellisenseModel { Key = x, Value = x }));

                    //Task.Factory.StartNew(async () =>
                    //{
                    //    var friendsName = await accountsDetailsSelector.GetFriendshipNames(accountId, accountUsername);

                    //    friendsName.ForEach(x =>
                    //        ListOfMacros.Add(new SocinatorIntellisenseModel { Key = x, Value = x }));
                    //});
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return ListOfMacros.Where(x => x.Key.ToLower().Contains(filter.ToLower()));
        }
    }
}