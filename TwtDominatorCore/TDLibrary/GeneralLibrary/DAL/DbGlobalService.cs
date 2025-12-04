using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.DAL
{
    public interface IDbGlobalService
    {
        IReadOnlyCollection<BlackListUser> GetAllBlackListUsers();
        IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers();
        void Add<T>(T data) where T : class, new();
    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbBlackListOperations;
        private readonly DbOperations _dbWhiteListOperations;

        public DbGlobalService()
        {
            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbBlackListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Twitter, UserType.BlackListedUser));
            _dbWhiteListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Twitter, UserType.WhiteListedUser));
        }

        /// <summary>
        ///     we have to different db for whitelist and blacklist
        ///     therefore we have different dbcontext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Add<T>(T data) where T : class, new()
        {
            var type = typeof(T).Name;
            if (type == "WhiteListUser")
                _dbWhiteListOperations.Add(data);
            else
                _dbBlackListOperations.Add(data);
        }

        public IReadOnlyCollection<BlackListUser> GetAllBlackListUsers()
        {
            return _dbBlackListOperations.Get<BlackListUser>();
        }

        public IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers()
        {
            return _dbWhiteListOperations.Get<WhiteListUser>();
        }
    }
}