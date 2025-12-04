using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using System.Collections.Generic;

namespace YoutubeDominatorCore.YoutubeLibrary.DAL
{
    public interface IDbGlobalService
    {
        IReadOnlyCollection<BlackListUser> GetAllBlackListUsers();
        IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers();

        bool AddSingle<T>(T data) where T : class, new();
        bool UpdateSingle<T>(T data) where T : class, new();
    }

    public class DbGlobalService : IDbGlobalService
    {
        private readonly DbOperations _dbBlackListOperations;
        private readonly DbOperations _dbOperations;
        private readonly DbOperations _dbWhiteListOperations;

        public DbGlobalService()
        {
            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            _dbOperations = new DbOperations(dataBaseConnectionGlb.GetSqlConnection());
            _dbBlackListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.YouTube, UserType.BlackListedUser));
            _dbWhiteListOperations =
                new DbOperations(
                    dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.YouTube, UserType.WhiteListedUser));
        }

        public IReadOnlyCollection<BlackListUser> GetAllBlackListUsers()
        {
            return _dbBlackListOperations.Get<BlackListUser>();
        }

        public IReadOnlyCollection<WhiteListUser> GetAllWhiteListUsers()
        {
            return _dbWhiteListOperations.Get<WhiteListUser>();
        }

        /// <summary>
        ///     we have to different db for whitelist and blacklist
        ///     therefore we have different dbcontext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddSingle<T>(T data) where T : class, new()
        {
            return typeof(T).Name == "WhiteListUser"
                ? _dbWhiteListOperations.Add(data)
                : _dbBlackListOperations.Add(data);
        }


        public bool UpdateSingle<T>(T data) where T : class, new()
        {
            return typeof(T).Name == "WhiteListUser"
                ? _dbWhiteListOperations.Update(data)
                : _dbBlackListOperations.Update(data);
        }
    }
}