using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDominatorCore.Response
{
 public class SendMessageResponseHandler :RDResponseHandler
    {
        public SendMessageResponseHandler(IResponseParameter response) : base(response)
        {

            try
            {
                if (Success && response.Response == "{}")
                {
                    Success = true;
                }
                else
                {
                    response.HasError = response.Response.Contains("Forbidden");
                    Success = false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
