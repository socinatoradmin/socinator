using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;

namespace PinDominatorCore.PDLibrary.DAL
{
    public interface IDbGlobalService
    {
        void Add<T>(T data) where T : class, new();
        List<BlackListUser> GetBlackListedUser();
        List<WhiteListUser> GetWhiteListUser();
    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly IGlobalDatabaseConnection _dataBaseConnectionGlb;
        private DbOperations _dbOperations;

        public DbGlobalService()
        {
            _dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbOperations = new DbOperations(_dataBaseConnectionGlb.GetSqlConnection());
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public List<BlackListUser> GetBlackListedUser()
        {
            _dbOperations =
                new DbOperations(
                    _dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Pinterest, UserType.BlackListedUser));
            return _dbOperations.Get<BlackListUser>();
        }

        public List<WhiteListUser> GetWhiteListUser()
        {
            _dbOperations =
                new DbOperations(
                    _dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Pinterest, UserType.WhiteListedUser));
            return _dbOperations.Get<WhiteListUser>();
        }
    }
}