using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Request;

namespace Tumblr.TumblrRequest
{
    public class TumblrRequestParameter : RequestParameters
    {
        public TumblrRequestParameter() : base()
        {
           InitializeHeders();
        }


        public void InitializeHeders()
        {

          //  base.Referer = "https://www.tumblr.com/login";

            base.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            base.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

           // base.ContentType = "application/x-www-form-urlencoded";

          //  base.AddHeader("Origin", "https://www.tumblr.com");

        }

    }

    public class products
    {

        public int ProductId { get; set; }

        public string status { get; set; }
    }
}
