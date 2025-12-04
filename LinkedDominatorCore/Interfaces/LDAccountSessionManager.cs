using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;

namespace LinkedDominatorCore.Interfaces
{
    public interface ILDAccountSessionManager
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount,bool Update=false);
        void InitializeAllSession();
    }
    public class LDAccountSessionManager : ILDAccountSessionManager
    {
        public List<AccountsSessionsTable> LDAccountSessions { get; set; }
        public string SessionFileName { get; set; } = "LinkedInAccountSession.db";
        public string SessionFolder { get; set; }
        public SQLiteConnection Connection { get; set; }
        private readonly object locker = new object();
        private readonly object sessionLocker= new object();
        private static string CurrentSessionId { get; set; }
        public static bool IsInitialize { get; set; }
        private readonly ILdHttpHelper httpHelper;
        private List<AccountsSessionsTable> LdAccountSession;
        public LDAccountSessionManager()
        {
            
            SessionFolder = ConstantVariable.GetIndexAccountDir() + $"\\DB\\{SessionFileName}";
            httpHelper = InstanceProvider.GetInstance<ILdHttpHelper>();
            UpdateCurrentSession();
            if (!IsInitialize)
                Initializedb();
            InitializeAllSession();
        }

        private void UpdateCurrentSession()
        {
            if(string.IsNullOrEmpty(CurrentSessionId))
                CurrentSessionId=GetCurrentSessionId();
        }

        private void Initializedb()
        {
            try
            {
                string dbDir = Path.GetDirectoryName(SessionFolder);
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }
                Connection = new SQLiteConnection(SessionFolder);
                Connection.CreateTable<AccountsSessionsTable>();
                IsInitialize = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                IsInitialize = false;
            }
        }
        public string GetCurrentSession(DominatorAccountModel account)
        {
            if (!string.IsNullOrEmpty(account.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(account.AccountBaseModel.AccountProxy.ProxyPort))
                return account.AccountId + Getproxy(account.AccountBaseModel.AccountProxy.ProxyIp);
            else
                return account.AccountId + CurrentSessionId;
        }
        public string Getproxy(string proxy) => string.IsNullOrEmpty(proxy) ? proxy : proxy?.Replace(".","")?.Replace(":","")?.Trim();
        public void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool update = false)
        {
            try
            {
                Monitor.Enter(sessionLocker);
                LdAccountSession= Get<AccountsSessionsTable>();
                var Account = dominatorAccount;
                var ID = GetCurrentSession(Account);
                if (LdAccountSession.Exists(x => x.AccountId == Account.AccountId && (x.CurrentSessionId == ID || x.CurrentSessionId==CurrentSessionId)))
                {
                    if(update)
                    {
                        var currentSession=LdAccountSession.FirstOrDefault(x => x.AccountId == Account.AccountId && (x.CurrentSessionId == ID ||x.CurrentSessionId==CurrentSessionId));
                        currentSession.CurrentSession = Serialize(GetCurrentSessionFromCookies(Account.Cookies));
                        currentSession.CurrentSessionId = ID;
                        currentSession.AccountId = Account.AccountId;
                        currentSession.LastUpdated = DateTime.Now.GetCurrentEpochTimeMilliSeconds().ToString();
                        Update(currentSession);
                    }
                    else
                    {
                        var currentSession = LdAccountSession.FirstOrDefault(x => x.AccountId == Account.AccountId && (x.CurrentSessionId == ID || x.CurrentSessionId == CurrentSessionId));
                        dominatorAccount.Cookies = GetCookiesFromSession(currentSession.CurrentSession);
                    }
                }
                else
                {
                    dominatorAccount.Cookies = dominatorAccount.Cookies ?? new CookieCollection();
                    AddNewSession(Account);
                }
                
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally { Monitor.Exit(sessionLocker); }
        }

        private HashSet<CookieHelper> GetCurrentSessionFromCookies(CookieCollection cookies)
        {
            return cookies?.Cast<Cookie>().Select(cookie => new CookieHelper
            {
                Domain = cookie.Domain,
                Name = cookie.Name,
                Value = cookie.Value,
                Expires = cookie.Expires,
                HttpOnly = false,
                Secure = true
            }).ToHashSet();
        }

        private CookieCollection GetCookiesFromSession(string currentSession)
        {
            var cookieCollection=new CookieCollection();

            try
            {
                JArray.Parse(currentSession).ToObject<HashSet<CookieHelper>>().ForEach(x =>
                    {
                        cookieCollection.Add(new Cookie
                        {
                            Domain = x.Domain,
                            Name = x.Name,
                            Value = x.Value,
                            Expires = x.Expires,
                            HttpOnly = x.HttpOnly,
                            Secure = x.Secure
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return cookieCollection;
        }

        public void InitializeAllSession()
        {
            try
            {
                var accountModels = InstanceProvider.GetInstance<IDominatorAccountViewModel>();
                var accounts = accountModels.LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == DominatorHouseCore.Enums.SocialNetworks.LinkedIn).ToList();
                LdAccountSession = Get<AccountsSessionsTable>();
                try
                {
                    //Remove 3 months Old Session.
                    var oldSessions = LdAccountSession?.FindAll(x => (GetDayFromEpochTime(x.LastUpdated)) > 180);
                    if (oldSessions != null && oldSessions.Count > 0)
                    {
                        oldSessions.ForEach(x => Remove(x));
                        LdAccountSession = Get<AccountsSessionsTable>();
                    }
                }
                catch { LdAccountSession = Get<AccountsSessionsTable>(); }
                if(accounts.Count >= LdAccountSession?.Count && LdAccountSession?.Count > 0)
                {
                    foreach (var item in accounts.SkipWhile(x=> LdAccountSession.Any(y=>y.AccountId==x.AccountId)))
                    {
                        AddNewSession(item);
                    }
                }
                else
                {
                    foreach (var item in accounts.SkipWhile(x => LdAccountSession.Any(y => y.AccountId == x.AccountId)))
                    {
                        AddNewSession(item);
                    }
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
        }

        private int GetDayFromEpochTime(string lastUpdated)
        {
            long.TryParse(lastUpdated, out var time);
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(time);
            var dateTime = dateTimeOffset.DateTime;
            var difference=DateTime.Now - dateTime;
            return difference.Days;
        }

        public void AddNewSession(DominatorAccountModel accountModel)
        {
            var accountToBeUpdate = new AccountsSessionsTable
            {
                AccountId = accountModel.AccountId,
                CurrentSessionId = GetCurrentSession(accountModel),
                CurrentSession = Serialize(GetCurrentSessionFromCookies(accountModel.Cookies)),
                LastUpdated = DateTime.Now.GetCurrentEpochTimeMilliSeconds().ToString(),
            };
            Add(accountToBeUpdate);
        }
        private string GetCurrentSessionId()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                if (host != null)
                    return host.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString()?.Replace(".", "")?.Replace(":", "");
                else
                    return GetIpFromWeb();
            }
            catch (Exception)
            {
                return GetIpFromWeb();
            }
            
        }

        private string GetIpFromWeb()
        {
            var response = httpHelper.GetRequest("https://app.multiloginapp.com/WhatIsMyIP").Response;
            return Getproxy(Utils.GetBetween(response, "bgm-green\">\r\n                    <h2>", "</h2>")?.Replace("\r\n", ""))?.Replace(".", "")?.Replace(":", "");
        }

        public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);
        public T Deserialize<T>(string obj) => JsonConvert.DeserializeObject<T>(obj);
        public bool Add<T>(T data) where T : class, new()
        {
            lock (locker)
            {
                return Connection.Insert(data) > 0;
            }
        }
        public bool AddRange<T>(List<T> data) where T : class, new()
        {
            lock (locker)
            {
                return Connection.InsertAll(data) > 0;
            }
        }
        public bool Update<T>(T t) where T : class, new()
        {
            lock (locker)
            {
                return Connection.Update(t) > 0;
            }
        }
        public bool Remove<T>(T t) where T : class
        {
            lock (locker)
            {
                return Connection.Delete<T>(t) > 0;
            }
        }

        public bool RemoveAll<T>() where T : class, new()
        {
            lock (locker)
            {
                return Connection.DeleteAll<T>() > 0;
            }
        }


        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (locker)
            {
                Remove(Connection.Table<T>().Where(expression).FirstOrDefault());
            }
        }


        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (locker)
            {
                var matchedItems = Connection.Table<T>().Where(expression);
                foreach (var items in matchedItems)
                    Remove(items);
            }
        }
        public bool Any<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            lock (locker)
            {
                return Connection.Table<T>().Where(expression).Any();
            }
        }
        public bool UpdateRange<T>(List<T> data) where T : class, new()
        {
            lock (locker)
            {
                return Connection.UpdateAll(data) > 0;
            }
        }
        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            lock (locker)
            {
                return expression == null
                    ? Connection?.Table<T>()?.ToList()
                    : Connection?.Table<T>()?.Where(expression)?.ToList();
            }
        }
    }
}
