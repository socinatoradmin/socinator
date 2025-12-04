using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using System;
// ReSharper disable All

namespace FaceDominatorCore.FDLibrary.DAL
{
    public interface IDbGlobalService
    {

        bool CheckForBlackListedUser(SocialNetworks network, string userName);

        bool UpdateCookies(DominatorAccountModel account);

    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbOperations;

        public DbGlobalService()
        {
            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbOperations = new DbOperations(dataBaseConnectionGlb.GetSqlConnection());
        }


        public bool CheckForBlackListedUser(SocialNetworks network, string userName)
        {
            var socialNetwork = network.ToString();
            return _dbOperations.Any<BlackWhiteListUser>(x =>
                x.Network == socialNetwork &&
                x.UserName == userName);
        }

        public bool UpdateCookies(DominatorAccountModel account)
        {
            try
            {
                var accountDetails = _dbOperations.GetSingle<AccountDetails>(x => x.AccountId == account.AccountId);
                accountDetails.Cookies = JsonConvert.SerializeObject(account.CookieHelperList).Replace(",", "<>");
                _dbOperations.Update(accountDetails);
                return true;
            }
            catch (ArgumentNullException e) { e.DebugLog(); }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

    }
}
