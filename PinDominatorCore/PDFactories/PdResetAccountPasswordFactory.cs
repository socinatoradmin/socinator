using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDUtility;
using Unity;

namespace PinDominatorCore.PDFactories
{
    public class PdResetAccountPasswordFactory : IAccountVerificationFactory
    {
        private IPinFunction _pinFunct;

        public Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, accountModel.UserName,
                "LangKeyResetPassword".FromResourceDictionary(),
                "LangKeyAutoEmailVerificationStartedMessage".FromResourceDictionary());

            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _pinFunct = accountScopeFactory[$"{accountModel.AccountId}_UpdateCheck"].Resolve<IPinFunction>();

            var linkResponseHandler = _pinFunct.SendResetPasswordLink(accountModel).Result;
            if (linkResponseHandler.Success)
            {
                var isFetched = _pinFunct.ReadResetPasswordLinkFromEmail(accountModel);
                if (isFetched)
                    return VerifyAccountAsync(accountModel, VerificationType.Email, accountModel.Token);
                return Task.FromResult(isFetched);
            }

            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, accountModel.UserName,
                "LangKeyResetPassword".FromResourceDictionary(),
                string.Format("LangKeyFailedToResetMessage".FromResourceDictionary(), linkResponseHandler?.Issue?.Message));

            return Task.FromResult(false);
        }

        public Task<bool> SendVerificationCode(DominatorAccountModel accountModel, VerificationType verificationType,
            CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, accountModel.UserName,
                "LangKeyVerification".FromResourceDictionary(),
                "LangKeyResetPasswordLinkSendingProcessStarted".FromResourceDictionary());

            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _pinFunct = accountScopeFactory[$"{accountModel.AccountId}_UpdateCheck"].Resolve<IPinFunction>();

            var linkResponseHandler = _pinFunct.SendResetPasswordLink(accountModel).Result;

            if (linkResponseHandler == null)
                return Task.FromResult(false);
            if (linkResponseHandler.Success)
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, accountModel.UserName,
                    "LangKeyVerification".FromResourceDictionary(),
                    "LangKeyResetPasswordLinkSentSuccessfulMessage".FromResourceDictionary());
            else
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, accountModel.UserName,
                    "LangKeyVerification".FromResourceDictionary(),
                    string.Format("LangKeyResetPasswordLinkFailed", linkResponseHandler.Issue?.Message));
            return Task.FromResult(linkResponseHandler.Success);
        }

        public Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, VerificationType verificationType,
            CancellationToken token)
        {
            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _pinFunct = accountScopeFactory[$"{accountModel.AccountId}_UpdateCheck"].Resolve<IPinFunction>();
            return _pinFunct.ResetPasswordWithLink(accountModel);
        }
    }
}