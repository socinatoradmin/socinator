using GramDominatorCore.GDLibrary.Response;
using System;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore;

namespace GramDominatorCore.Response
{
    public class ResendTwoFactorLoginCodeResponseHandler : IGResponseHandler
    {
        public ResendTwoFactorLoginCodeResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success)
                {
                }
                else
                {
                    obfuscated_phone_number = RespJ["two_factor_info"]["obfuscated_phone_number"].ToString();
                    two_factor_identifier = RespJ["two_factor_info"]["two_factor_identifier"].ToString();
                    status = RespJ["two_factor_info"].ToString();
                    two_factor_required = RespJ["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                throw new Exception("Error from LoginIgResponseHandler class constructor => " + response.Response);
            }

        }

        public string obfuscated_phone_number { get; set; }

        public string two_factor_identifier { get; set; }

        public string status { get; set; }

        public string two_factor_required { get; set; }
    }
}
