using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;

namespace Tumblr.TumblrResponseHandler
{
    public class LogInResponseHandler : ResponseHandler
    {


        public LogInResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
