using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;

namespace LinkedDominatorCore.LDLibrary.DAL
{
    public interface IDbGlobalService
    {
        bool CheckForBlackListedUser(SocialNetworks network, string userName);
        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        bool Add<T>(T data) where T : class, new();
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
            var _network = network.ToString();
            return _dbOperations.Any<BlackWhiteListUser>(x =>
                x.Network == _network &&
                x.UserName == userName);
        }

        public bool Add<T>(T data) where T : class, new()
        {
            return _dbOperations.Add(data);
        }

        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            try
            {
                // Get the matched expression records, If expression is null returns full details
                return _dbOperations.Get(expression);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }
    }
}