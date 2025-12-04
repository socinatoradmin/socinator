#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public interface IDbOperations : IDisposable
    {
        bool Add<T>(T data) where T : class, new();
        bool AddRange<T>(List<T> data) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        bool Update<T>(T t) where T : class, new();
        bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel);
        bool Remove<T>(T t) where T : class;
        bool RemoveAll<T>() where T : class, new();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool Any<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool UpdateRange<T>(List<T> data) where T : class, new();
        SocialNetworks SocialNetworks { get; }
        string AccountId { get; }
    }

    public class DbOperations : IDbOperations
    {
        private static readonly ConcurrentDictionary<string, object> SyncDictionary;
        private readonly object _syncObject;

        private readonly SQLiteConnection _context;
        public SocialNetworks SocialNetworks { get; }
        public string AccountId { get; }

        static DbOperations()
        {
            SyncDictionary = new ConcurrentDictionary<string, object>();
        }

        public DbOperations(SQLiteConnection context)
        {
            SocialNetworks = SocialNetworks.Social;
            _context = context;
            _syncObject = SyncDictionary.GetOrAdd(_context.DatabasePath, a => new object());
        }

        /// <summary>
        ///     To Get the database operation with auto generated dbcontext for your account or campaigns
        /// </summary>
        /// <param name="id">
        ///     If you need to perform with account, then pass id as account id where as in campaign case pass
        ///     campaign id
        /// </param>
        /// <param name="networks">Accounts or campaign which belongs to which social network</param>
        /// <param name="type">
        ///     Specify whether you account id or campaign id in
        ///     <see cref="DominatorHouseCore.Utility.ConstantVariable.GetAccountDb" /> or
        ///     <see cref="DominatorHouseCore.Utility.ConstantVariable.GetCampaignDb" />
        /// </param>
        public DbOperations(string id, SocialNetworks networks, string type)
        {
            AccountId = id;
            SocialNetworks = networks;
            if (type == ConstantVariable.GetAccountDb)
            {
                var databaseConnection =
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(networks.ToString());
                _context = databaseConnection.GetSqlConnection(id);
            }

            if (type == ConstantVariable.GetCampaignDb)
            {
                var databaseConnection =
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(networks.ToString());
                _context = databaseConnection.GetSqlConnection(id);
            }

            _syncObject = SyncDictionary.GetOrAdd(_context.DatabasePath, a => new object());
        }


        #region Create operations

        /// <summary>
        ///     To add the data to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Add<T>(T data) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.Insert(data) > 0;
            }
        }

        public bool AddRange<T>(List<T> data) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.InsertAll(data) > 0;
            }
        }

        #endregion

        #region Read Operations

        /// <summary>
        ///     To get the count of the matched expression count
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Filtering expression</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            lock (_syncObject)
            {
                return expression == null ? _context.Table<T>().Count() : _context.Table<T>().Where(expression).Count();
            }
        }


        /// <summary>
        ///     To get the records which matches the expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Matching expression</param>
        /// <returns></returns>
        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            lock (_syncObject)
            {
                return expression == null
                    ? _context.Table<T>().ToList()
                    : _context.Table<T>().Where(expression).ToList();
            }
        }

        /// <summary>
        ///     To get the first record which matches the given expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.Table<T>().Where(expression).FirstOrDefault();
            }
        }


        /// <summary>
        ///     To get the records which matches the expression in async mode
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            lock (_syncObject)
            {
                return expression == null
                    ? _context.Table<T>().ToList()
                    : _context.Table<T>().Where(expression).ToList();
            }
        }

        #endregion

        #region Update Operations

        /// <summary>
        ///     To update the record in the database
        /// </summary>
        /// <typeparam name="T">Targer type</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.Update(t) > 0;
            }
        }


        public bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            lock (_syncObject)
            {
                var dataToUpdate = _context.Table<AccountDetails>()
                    .FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);

                if (dataToUpdate == null)
                {
                    AddIfNotExist(dominatorAccountModel);
                    return false;
                }

                dataToUpdate.UserName = dominatorAccountModel.AccountBaseModel.UserName;
                dataToUpdate.Password = dominatorAccountModel.AccountBaseModel.Password;
                dataToUpdate.AccountNetwork = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString();
                dataToUpdate.ProxyIP = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyIp;
                dataToUpdate.ProxyPort = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyPort;
                dataToUpdate.ProxyUserName = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyUsername;
                dataToUpdate.ProxyPassword = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyPassword;
                dataToUpdate.UserFullName = dominatorAccountModel.AccountBaseModel.UserFullName;
                dataToUpdate.Status = dominatorAccountModel.AccountBaseModel.Status.ToString();
                dataToUpdate.AccountName = dominatorAccountModel.AccountBaseModel.AccountName;
                dataToUpdate.Cookies = JsonConvert.SerializeObject(dominatorAccountModel.Cookies);
                dataToUpdate.ProfilePictureUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                dataToUpdate.DisplayColumnValue1 = dominatorAccountModel.DisplayColumnValue1;
                dataToUpdate.DisplayColumnValue2 = dominatorAccountModel.DisplayColumnValue2;
                dataToUpdate.DisplayColumnValue3 = dominatorAccountModel.DisplayColumnValue3;
                dataToUpdate.DisplayColumnValue4 = dominatorAccountModel.DisplayColumnValue4;
                if (dominatorAccountModel.ActivityManager != null)
                    UpdateAccountActivityManager(dominatorAccountModel);
                return Update(dataToUpdate);
            }
        }

        private void AddIfNotExist(DominatorAccountModel dominatorAccountModel)
        {
            Add(new AccountDetails
            {
                AccountNetwork = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString(),
                AccountId = dominatorAccountModel.AccountBaseModel.AccountId,
                AccountGroup = dominatorAccountModel.AccountBaseModel.AccountGroup.Content,
                UserName = dominatorAccountModel.AccountBaseModel.UserName,
                Password = dominatorAccountModel.AccountBaseModel.Password,
                UserFullName = dominatorAccountModel.AccountBaseModel.UserFullName,
                Status = dominatorAccountModel.AccountBaseModel.Status.ToString(),
                ProxyIP = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyIp,
                ProxyPort = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyPort,
                ProxyUserName = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyUsername,
                ProxyPassword = dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyPassword,
                ProfilePictureUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl,
                AccountName = dominatorAccountModel.AccountBaseModel.AccountName,
                Cookies = JsonConvert.SerializeObject(dominatorAccountModel.Cookies),
                UserAgent = dominatorAccountModel.UserAgentWeb,
                AddedDate = DateTime.Now,
                ActivityManager = JsonConvert.SerializeObject(dominatorAccountModel.ActivityManager),
                DisplayColumnValue1 = dominatorAccountModel.DisplayColumnValue1,
                DisplayColumnValue2 = dominatorAccountModel.DisplayColumnValue2,
                DisplayColumnValue3 = dominatorAccountModel.DisplayColumnValue3,
                DisplayColumnValue4 = dominatorAccountModel.DisplayColumnValue4
            });
        }

        public void UpdateAccountActivityManager(DominatorAccountModel dominatorAccountModel)
        {
            lock (_syncObject)
            {
                var dataToUpdate = _context.Table<AccountDetails>()
                    .FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);

                if (dataToUpdate == null)
                    return;

                dataToUpdate.ActivityManager = JsonConvert.SerializeObject(dominatorAccountModel.ActivityManager);
                Update(dataToUpdate);
            }
        }

        #endregion

        #region Delete Operations

        public bool Remove<T>(T t) where T : class
        {
            lock (_syncObject)
            {
                return _context.Delete<T>(t) > 0;
            }
        }

        public bool RemoveAll<T>() where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.DeleteAll<T>() > 0;
            }
        }


        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (_syncObject)
            {
                Remove(_context.Table<T>().Where(expression).FirstOrDefault());
            }
        }


        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (_syncObject)
            {
                var matchedItems = _context.Table<T>().Where(expression);
                foreach (var items in matchedItems)
                    Remove(items);
            }
        }


        public bool Any<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.Table<T>().Where(expression).Any();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        public List<string> GetSingleColumn<T>(Func<T, string> query, Expression<Func<T, bool>> expression = null)
            where T : class, new()
        {
            lock (_syncObject)
            {
                return expression == null
                    ? _context.Table<T>().Select(query).ToList()
                    : _context.Table<T>().Where(expression).Select(query).ToList();
            }
        }

        public List<string> GetUniqueSingleColumn<T>(Func<T, string> query, Func<T, string> groupByQuery,
            Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            lock (_syncObject)
            {
                return expression == null
                    ? _context.Table<T>().GroupBy(groupByQuery).Select(x => x.FirstOrDefault()).Select(query).ToList()
                    : _context.Table<T>().Where(expression).GroupBy(groupByQuery).Select(x => x.FirstOrDefault())
                        .Select(query).ToList();
            }
        }

        public bool UpdateRange<T>(List<T> data) where T : class, new()
        {
            lock (_syncObject)
            {
                return _context.UpdateAll(data) > 0;
            }
        }

        private void AddAccountBackupIfNotExist(DominatorAccountModel dominatorAccountModel, string deviceId,
            string UserAgent)
        {
            Add(new InstaAccountBackup
            {
                AccountName = dominatorAccountModel.UserName,
                DeviceId = deviceId,
                UserAgent = UserAgent
            });
        }

        public bool UpdateAccountBackupDetails(DominatorAccountModel dominatorAccountModel, string deviceId,
            string userAgent)
        {
            lock (_syncObject)
            {
                var dataToUpdate = _context.Table<InstaAccountBackup>()
                    .FirstOrDefault(x => x.AccountName == dominatorAccountModel.UserName);

                if (dataToUpdate == null)
                {
                    AddAccountBackupIfNotExist(dominatorAccountModel, deviceId, userAgent);
                    return false;
                }

                dataToUpdate.AccountName = dominatorAccountModel.AccountBaseModel.UserName;
                dataToUpdate.DeviceId = deviceId;
                dataToUpdate.UserAgent = userAgent;
                if (dominatorAccountModel.ActivityManager != null)
                    UpdateAccountActivityManager(dominatorAccountModel);
                return Update(dataToUpdate);
            }
        }
    }
}