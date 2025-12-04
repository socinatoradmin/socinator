using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;

namespace QuoraDominatorCore.QdLibrary.DAL
{
    public interface IDbGlobalService
    {
        bool CheckForWhiteListedUser(string userName);
        IReadOnlyCollection<BlackListUser> GetAllBlackListUsers();
        IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers();
        bool Add<T>(T data) where T : class, new();
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
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Quora, UserType.BlackListedUser));
            _dbWhiteListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Quora, UserType.WhiteListedUser));
        }

        /// <summary>
        ///     we have to different db for whitelist and blacklist
        ///     therefore we have different dbcontext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Add<T>(T data) where T : class, new()
        {
            var type = typeof(T).Name;
            return type == "WhiteListUser" ? _dbWhiteListOperations.Add(data) : _dbBlackListOperations.Add(data);
        }

        public bool CheckForWhiteListedUser(string userName)
        {
            return _dbWhiteListOperations.Any<WhiteListUser>(x => x.UserName == userName);
        }

        public IReadOnlyCollection<BlackListUser> GetAllBlackListUsers()
        {
            return _dbBlackListOperations.Get<BlackListUser>();
        }

        public IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers()
        {
            return _dbWhiteListOperations.Get<WhiteListUser>();
        }

        public bool CheckForBlackListedUser(string userName)
        {
            return _dbBlackListOperations.Any<BlackListUser>(x => x.UserName == userName);
        }
    }
}