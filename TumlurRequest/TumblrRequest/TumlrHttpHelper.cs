using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Request;

namespace Tumblr.TumblrRequest
{
    public class TumblrHttpHelper : HttpHelper
    {

        public TumblrRequestParameter tumblrRequestparameter { get; set; }

        public TumblrHttpHelper() : base()
        {
            
        }

        public TumblrHttpHelper(TumblrRequestParameter tumblrRequestParameter) : base(tumblrRequestParameter)
        {
            try
            {
                this.tumblrRequestparameter = tumblrRequestParameter;
            } 
            catch (Exception e)
            {

            }
        }

    }
}
