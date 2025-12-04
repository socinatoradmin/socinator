using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdAccountVerificationFactory : IAccountVerificationFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        public YdAccountVerificationFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
        }

        public async Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token)
        {
            return await new Task<bool>(() => false);
        }

        public async Task<bool> SendVerificationCode(DominatorAccountModel accountModel,
            VerificationType verificationType, CancellationToken token)
        {
            try
            {
                var accountUpdateFactory = InstanceProvider.GetInstance<IYdAccountUpdateFactory>();

                accountModel.IsVerificationCodeSent = true;
                new Task(() => accountUpdateFactory.CheckStatusAsync(accountModel, token), token).Start();
                var result = TaskCompleted(accountModel, token);
                return result;
            }
            catch (Exception)
            {
                // ignore
            }

            return await new Task<bool>(() => false);
        }

        public async Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel,
            VerificationType verificationType, CancellationToken token)
        {
            if (accountModel.AccountBaseModel.Status == AccountStatus.Success)
            {
                accountModel.IsVerificationCodeSent = false;
                return false;
            }

            if (accountModel.IsVerificationCodeSent) return true;
            accountModel.IsVerificationCodeSent = true;
            var result = TaskCompleted(accountModel, token);
            return result;
        }

        private bool TaskCompleted(DominatorAccountModel accountModel, CancellationToken token)
        {
            var timeForNext2Minutes = DateTime.Now.AddMinutes(3.1);
            while (accountModel.IsVerificationCodeSent)
            {
                if (timeForNext2Minutes < DateTime.Now || token.IsCancellationRequested ||
                    accountModel.AccountBaseModel.Status == AccountStatus.Success ||
                    accountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnSignIn ||
                    accountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification)
                {
                    accountModel.IsVerificationCodeSent = false;
                    return false;
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            accountModel.IsVerificationCodeSent = false;
            return true;
        }
    }
}