using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class PreLoginData:IGResponseHandler
    {
        public string CsrfToken { get; set; }
        public int KeyID { get; set; }
        public string PublicKey { get; set; }
        public string Version { get; set; }
        public string WebDeviceID { get; set; }
        public PreLoginData(IResponseParameter response):base(response)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(response.Response);
                CsrfToken = handler.GetJTokenValue(obj, "config", "csrf_token");
                WebDeviceID = handler.GetJTokenValue(obj, "device_id");
                var encryption = handler.GetJTokenOfJToken(obj, "encryption");
                PublicKey = handler.GetJTokenValue(encryption, "public_key");
                Version = handler.GetJTokenValue(encryption, "version");
                int.TryParse(handler.GetJTokenValue(encryption, "key_id"), out int keyId);
                KeyID = keyId;
            }
            catch { }
        }
        public PreLoginData() { }
    }
}
