using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;

namespace TwtDominatorCore.TDFactories
{
    public class TdAccountVerificationFactory : IAccountVerificationFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDelayService _delayService;
        private IResponseParameter Firstresponse;

        public TdAccountVerificationFactory(IAccountScopeFactory accountScopeFactory, IDelayService delayService)
        {
            _accountScopeFactory = accountScopeFactory;
            _delayService = delayService;
        }

        private LogInResponseHandler LogInResponse { get; set; } = new LogInResponseHandler();
        private static string Domain=>TdConstants.Domain;
        public async Task<bool> SendVerificationCode(DominatorAccountModel accountModel,
            VerificationType verificationType, CancellationToken cancellationToken)
        {
            return await Task.FromResult(true);
        }


        /// <summary>
        ///     Logging in again for verify account
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="verificationType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> VerifyAccountAsync(DominatorAccountModel dominatorAccountModel,
            VerificationType verificationType, CancellationToken cancellationToken)
        {
            try
            {
                var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITdHttpHelper>();
                var twtFunc = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwitterFunctions>();
                var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
                if (string.IsNullOrEmpty(dominatorAccountModel.VarificationCode.Trim()))
                    return false;

                var accountModel = new AccountModel(dominatorAccountModel);

                #region  login process

                try
                {
                    logInProcess.SetRequestParameter(ref dominatorAccountModel);
                    var ctoRandomly = TdUtility.GetRandomHexNumber(32).ToLower();
                    httpHelper.GetRequestParameter().Cookies
                        .Add(new Cookie("ct0", ctoRandomly) {Domain = $"{Domain}"});
                    Firstresponse =
                        await httpHelper.GetRequestAsync($"https://{Domain}",
                            cancellationToken);
                    if (string.IsNullOrEmpty(Firstresponse.Response))
                    {
                        await _delayService.DelayAsync(new Random().Next(0, 6000), cancellationToken);
                        Firstresponse =
                            await httpHelper.GetRequestAsync($"https://{Domain}",
                                cancellationToken);
                    }

                    if (Firstresponse.HasError)
                    {
                        if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        else
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;

                        dominatorAccountModel.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName,
                            string.Format("LangKeyProxyNotWorkingMessage".FromResourceDictionary(),
                                Firstresponse?.Exception?.Message));
                        return false;
                    }

                    accountModel.CsrfToken = ctoRandomly;
                    dominatorAccountModel.Cookies = new CookieCollection();
                    var postAuthenticityToken =
                        TdUtility.GetPostAuthenticityToken(Firstresponse.Response, "postAuthenticityToken");
                    accountModel.postAuthenticityToken = postAuthenticityToken;
                    LogInResponse = await twtFunc.LogInAsync(dominatorAccountModel, cancellationToken);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                return RedirectAccountVerification(dominatorAccountModel, new AccountModel(dominatorAccountModel),
                    LogInResponse);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Redirect  Account verification on basis of challenge type
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="LogInResponse"></param>
        /// <returns></returns>
        private bool RedirectAccountVerification(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            LogInResponseHandler LogInResponse)
        {
            try
            {
                var accountVerification = new AccountVerification();
                accountVerification.AccountModel = accountModel;
                accountVerification.LogInPageResponse = LogInResponse;
                GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName);
                if (accountVerification.VerifyingAccount(dominatorAccountModel, LogInResponse))
                    return true;

                GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName,
                    string.Format("LangKeyNotValidVerificationInputFor".FromResourceDictionary(),
                        LogInResponse.status));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
    }
}