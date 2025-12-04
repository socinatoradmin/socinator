using System;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using System.Linq.Expressions;

namespace GramDominatorCore.GDLibrary.DAL
{
    public interface IDbGlobalService
    {
      //  bool CheckForBlackListedUser(SocialNetworks network, string userName);

        bool Add(AccountDetails objAccountDetails);

        bool update(AccountDetails objAccountDetails);

         T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbOperations;

        public DbGlobalService()
        {
            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbOperations = new DbOperations(dataBaseConnectionGlb.GetSqlConnection());
        }

        //public bool CheckForBlackListedUser(SocialNetworks network, string userName)
        //{
        //    return _dbOperations.Any<BlackWhiteListUser>(x =>
        //        x.Network == network.ToString() &&
        //        x.UserName == userName);
        //}

        public  bool Add(AccountDetails objAccountDetails)
        {
            return _dbOperations.Add(objAccountDetails);
        }

        public bool update(AccountDetails objAccountDetails)
        {
            return _dbOperations.Update(objAccountDetails);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }
    }
}
