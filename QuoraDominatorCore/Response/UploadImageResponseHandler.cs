using System;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;

namespace QuoraDominatorCore.Response
{
    public class UploadImageResponseHandler : QuoraResponseHandler
    {
        public string uploadedUrl = string.Empty;

        public UploadImageResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var jObject = JObject.Parse(response.Response);
                uploadedUrl = jObject["qimg_urls"].ToString()?.Replace("\r\n", "").Replace("[  \"", "")
                    .Replace("\"]", "");
            }
            catch (Exception)
            {
            }
        }
    }
}