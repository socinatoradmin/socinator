using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.Request;

namespace GramDominatorCore.GDFactories
{
    public class GdAccountVerificationFactory : IAccountVerificationFactory
    {
        //private IGdHttpHelper httpHelper;

      //  private IInstaFunction instaFunction;
        private IGdLogInProcess loginProcess;
        public async Task<bool> SendVerificationCode(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, accountModel.UserName, "LangKeyVerification".FromResourceDictionary(), "Verificarion code sending process has been started. Please wait...");
            loginProcess = InstanceProvider.GetInstance<IGdLogInProcess>();
            return await loginProcess.SendSecurityCodeAsync(accountModel,token, verificationType);       
        }

        public Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, accountModel.UserName, "LangKeyVerification".FromResourceDictionary(), "Auto email verification process has been started. Please wait...");
            //bool isAutoVerify = true;
            // bool verify = false;
            loginProcess = InstanceProvider.GetInstance<IGdLogInProcess>();
           // LogInProcess logInProcess = new LogInProcess(httpHelper, instaFunction);
            if (loginProcess.SendSecurityCodeAsync(accountModel,token,VerificationType.Email, true).Result)
            {                 
               LogInProcess.ReadVerificationCodeFromEmail(accountModel);
                return VerifyAccountAsync(accountModel, VerificationType.Email, accountModel.Token);
            }

            return Task.FromResult(false);
        }

        

        public async Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token)
        {
            loginProcess = InstanceProvider.GetInstance<IGdLogInProcess>();
            // LogInProcess logInProcess = new LogInProcess(httpHelper, instaFunction);
            return await loginProcess.VerifyAccountAsync(accountModel,token);
        }
    }
}
