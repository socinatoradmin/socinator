using System;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.Request;
using Unity;

namespace LinkedDominatorCore.Factories
{
    public class LDAccountVerficationFactory : IAccountVerificationFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IResponseParameter Firstresponse;

        public LDAccountVerficationFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }


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
                var httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ILdHttpHelper>();
                var LdFunction = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ILdFunctions>();
                if (string.IsNullOrEmpty(dominatorAccountModel.VarificationCode.Trim()))
                    return false;

                // #region  login process

                try
                {
                    LdFunction.SetCookieAndProxy(dominatorAccountModel, httpHelper);
                    LdFunction.PreLoginResponse(cancellationToken, false);
                    Thread.Sleep(5000);

                    await LdFunction.Login(cancellationToken);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                //    #endregion

                return false;
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
    }
}