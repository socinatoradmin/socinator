using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.Utility;
using System;
using System.Linq;

namespace PinDominatorCore.Response
{
    public class VideoUploadParametersRespHand : PdResponseHandler
    {
        public string Xamzdate;
        public string Xamzsignature;
        public string Xamzsecuritytoken;
        public string Xamzalgorithm;
        public string Key;
        public string Policy;
        public string Xamzcredential;
        public string ContentType;
        public string UploadId;
        public string UserId;
        public VideoUploadParametersRespHand(IResponseParameter response) : base(response)
        {
            try
            {
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var jsonData = jsonHandler.ParseJsonToJObject(response.Response);
                var Data = jsonHandler.GetJTokenOfJToken(jsonData, "resource_response", "data")?.FirstOrDefault()?.First();
                var uploadParameter = jsonHandler.GetJTokenOfJToken(Data, "upload_parameters");
                Xamzdate = jsonHandler.GetJTokenValue(uploadParameter, "x-amz-date");
                Xamzsignature = jsonHandler.GetJTokenValue(uploadParameter, "x-amz-signature");
                Xamzsecuritytoken = jsonHandler.GetJTokenValue(uploadParameter, "x-amz-security-token");
                Xamzalgorithm = jsonHandler.GetJTokenValue(uploadParameter, "x-amz-algorithm");
                Key = jsonHandler.GetJTokenValue(uploadParameter, "key");
                Policy = jsonHandler.GetJTokenValue(uploadParameter, "policy");
                Xamzcredential = jsonHandler.GetJTokenValue(uploadParameter, "x-amz-credential");
                ContentType = jsonHandler.GetJTokenValue(uploadParameter, "Content-Type");
                UploadId = jsonHandler.GetJTokenValue(Data, "upload_id");
                UserId = jsonHandler.GetJTokenValue(jsonData, "client_context", "user", "id");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
