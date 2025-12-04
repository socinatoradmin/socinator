using ProtectedCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProtectedServerApp
{
    //SocialNetworks.Social,
    //SocialNetworks.Facebook,
    //SocialNetworks.Twitter,
    //SocialNetworks.Gplus,
    //SocialNetworks.Instagram,
    //SocialNetworks.LinkedIn,
    //SocialNetworks.Quora,
    //SocialNetworks.Pinterest,
    //SocialNetworks.Tumblr,
    //SocialNetworks.Youtube,
    //SocialNetworks.Reddit

    public class ConfigurationsController: ApiController
    {
        static Dictionary<string, string> __licenses = new Dictionary<string, string>
        {
            {"License", "Social,Facebook,Twitter" }
        };
        // GET api/configurations/license 
        public string Get(string id)
        {
            string result;
            if (__licenses.TryGetValue(id, out result))
            {
                var signature = Signature.Sign(result);
                return result + "/" + signature;
            }
            return "//";
        }

    }
}
