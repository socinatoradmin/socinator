using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using System.Collections.Generic;
using System.Linq;

namespace TumblrDominatorCore.TumblrLibrary.DAL
{
    public interface IDbGlobalService
    {
        List<BlackListUser> GetAllBlackListUsers();
        List<WhiteListUser> GetAllWhiteListUsers();
        void Add<T>(T t) where T : class, new();
    }


    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbBlackListOperations;
        private readonly DbOperations _dbWhiteListOperations;

        private readonly DbOperations _dbOperations;

        public DbGlobalService()
        {

            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbBlackListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Tumblr, UserType.BlackListedUser));
            _dbWhiteListOperations =
               new DbOperations(
                   dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Tumblr, UserType.WhiteListedUser));
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbBlackListOperations.Add(data);
        }
        public List<BlackListUser> GetAllBlackListUsers()
        {
            return _dbBlackListOperations.Get<BlackListUser>()?.ToList() ?? new List<BlackListUser>();
        }
        public List<WhiteListUser> GetAllWhiteListUsers()
        {
            return _dbWhiteListOperations.Get<WhiteListUser>()?.ToList() ?? new List<WhiteListUser>();
        }

    }
}