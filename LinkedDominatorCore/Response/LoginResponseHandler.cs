using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.Response
{
    public class LoginResponseHandler : LdResponseHandler
    {
        public LoginResponseHandler()
        {
        }
        private JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public LoginResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            try
            {
                LoginResult = jsonHandler.GetJTokenValue(RespJ,"login_result");
                ChallengeUrl = jsonHandler.GetJTokenValue(RespJ,"challenge_url");
                ChallengeId = jsonHandler.GetJTokenValue(RespJ, "Challenge_id");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public LoginResponseHandler(string loginPageResponse) : base(loginPageResponse)
        {
            if (!Success)
                return;
            try
            {
                LoginResult = jsonHandler.GetJTokenValue(RespJ, "login_result");
                ChallengeUrl = jsonHandler.GetJTokenValue(RespJ, "challenge_url");
                ChallengeId = jsonHandler.GetJTokenValue(RespJ, "Challenge_id");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public string LoginResult { get; }
        public string ChallengeUrl { get; }
        public string ChallengeId { get; }
    }
}