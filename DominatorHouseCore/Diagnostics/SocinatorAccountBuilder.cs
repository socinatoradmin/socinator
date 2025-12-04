#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CommonServiceLocator;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorAccountBuilder
    {
        private readonly IAccountsFileManager _accountsFileManager;
        private DominatorAccountModel DominatorAccountModel { get; }

        public SocinatorAccountBuilder(string accountId)
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            DominatorAccountModel = _accountsFileManager.GetAccountById(accountId);
        }
        public static SocinatorAccountBuilder SaveAccount(DominatorAccountModel dominatorAccount)
        {
            var instance = Instance(dominatorAccount.AccountId)
                .AddOrUpdateCookies(dominatorAccount.Cookies)
                .AddOrUpdateBrowserCookies(dominatorAccount.BrowserCookies)
                .AddOrUpdateBrowserSettings(dominatorAccount.IsRunProcessThroughBrowser)
                .AddOrUpdateDominatorAccountBase(dominatorAccount.AccountBaseModel)
                .AddOrUpdateLoginStatus(dominatorAccount.IsUserLoggedIn)
                .AddOrUpdateUserAgentWeb(dominatorAccount.UserAgentWeb)
                .AddOrUpdateMobileAgentWeb(dominatorAccount.UserAgentMobile)
                .AddOrUpdateDisplayColumn1(dominatorAccount.DisplayColumnValue1)
                .AddOrUpdateDisplayColumn2(dominatorAccount.DisplayColumnValue2)
                .AddOrUpdateDisplayColumn3(dominatorAccount.DisplayColumnValue3)
                .AddOrUpdateDisplayColumn4(dominatorAccount.DisplayColumnValue4)
                .AddOrUpdateDisplayColumn11(dominatorAccount.DisplayColumnValue11)
                .UpdateLastUpdateTime(dominatorAccount.LastUpdateTime)
                .AddOrUpdateProxy(dominatorAccount.AccountBaseModel.AccountProxy)
                .AddOrUpdateMailCredentials(dominatorAccount.MailCredentials);
            instance.SaveToBinFile();
            return instance;
        }
        public static SocinatorAccountBuilder Instance(string accountId)
        {
            return new SocinatorAccountBuilder(accountId);
        }

        public SocinatorAccountBuilder AddOrUpdateCookies(CookieCollection cookies)
        {
            DominatorAccountModel.Cookies = cookies;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateBrowserCookies(CookieCollection cookies)
        {
            DominatorAccountModel.BrowserCookies = cookies;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateBrowserSettings(bool isBrowerAutomationActive)
        {
            DominatorAccountModel.IsRunProcessThroughBrowser = isBrowerAutomationActive;
            return this;
        }


        public SocinatorAccountBuilder AddOrUpdateDominatorAccountBase(DominatorAccountBaseModel accountBaseModel)
        {
            DominatorAccountModel.AccountBaseModel = accountBaseModel;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateLoginStatus(bool status)
        {
            DominatorAccountModel.IsUserLoggedIn = status;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateUserAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentWeb = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMobileAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentMobile = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(Dictionary<string, string> extraProperity)
        {
            try
            {
                DominatorAccountModel.ExtraParameters = extraProperity;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(string key, string value)
        {
            try
            {
                if (DominatorAccountModel.ExtraParameters.ContainsKey(key))
                    DominatorAccountModel.ExtraParameters[key] = value;
                else
                    DominatorAccountModel.ExtraParameters.Add(key, value);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn1(int? value)
        {
            DominatorAccountModel.DisplayColumnValue1 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn2(int? value)
        {
            DominatorAccountModel.DisplayColumnValue2 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn3(int? value)
        {
            DominatorAccountModel.DisplayColumnValue3 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn4(int? value)
        {
            DominatorAccountModel.DisplayColumnValue4 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn11(string value)
        {
            DominatorAccountModel.DisplayColumnValue11 = value;
            return this;
        }

        public SocinatorAccountBuilder UpdateLastUpdateTime(int value)
        {
            DominatorAccountModel.LastUpdateTime = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateProxy(Proxy proxy)
        {
            DominatorAccountModel.AccountBaseModel.AccountProxy = proxy;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMailCredentials(MailCredentials mailCredentials)
        {
            DominatorAccountModel.MailCredentials = mailCredentials;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateIsAutoVerifyByEmail(bool IsAutoVerifyByEmail)
        {
            DominatorAccountModel.IsAutoVerifyByEmail = IsAutoVerifyByEmail;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdatePaginationId(string key, string value)
        {
            try
            {
                if (DominatorAccountModel.PaginationId.Keys.Contains(key))
                    DominatorAccountModel.PaginationId[key] = value;
                else
                    DominatorAccountModel.PaginationId.Add(key, value);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateUseSslValue(bool useSsl)
        {
            DominatorAccountModel.IsUseSSL = useSsl;
            return this;
        }

        public bool SaveToBinFile()
        {
            return _accountsFileManager.Edit(DominatorAccountModel);
        }
    }
}