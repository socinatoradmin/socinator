using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDUtility;
using System;
using System.Collections.Specialized;

namespace RedditDominatorCore.Response
{
    public class SubRedditSubmitValidationResponseHandler : RdResponseHandler
    {
        public SubRedditSubmitValidationResponseHandler(IResponseParameter response) : base(response)
        {
            if (Success)
            {
            }
        }
    }

    public class PublisherPostUploadResponseHandler : RdResponseHandler
    {
        public PublisherPostUploadResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                jSonHandler = jsonHandler;
                if (!Success) return;
                if (response == null || response.Response == null ? true : response.Response.Contains("Request forbidden by administrative rules"))
                {
                    HasError = true;
                    return;
                }
                jObject = jSonHandler.ParseJsonToJObject(response.Response);
                UrlPath = jSonHandler.GetJTokenValue(jObject, "args", "action");
                var Count = jSonHandler.GetJArrayElement(jSonHandler.GetJTokenValue(jObject, "data","createMediaUploadLease","uploadLease","uploadLeaseHeaders"))?.Count;
                for (int I = 0; I < Count; I++)
                    SetValue(I);
                UpdatePropertyValue();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdatePropertyValue()
        {
            try
            {
                Acl = nameValueCollection.Get("acl");
                Bucket = nameValueCollection.Get("bucket");
                Key = nameValueCollection.Get("key");
                XAmzAlgorithm = nameValueCollection.Get("x-amz-algorithm");
                XAmzSecurityToken = nameValueCollection.Get("x-amz-security-token");
                XAmzStorageClass = nameValueCollection.Get("x-amz-storage-class");
                XAmzDate = nameValueCollection.Get("x-amz-date");
                SuccessActionStatus = nameValueCollection.Get("success_action_status");
                XAmzSignature = nameValueCollection.Get("x-amz-signature");
                XAmzMetaExt = nameValueCollection.Get("x-amz-meta-ext");
                Policy = nameValueCollection.Get("policy");
                XAmzCredential = nameValueCollection.Get("x-amz-credential");
                ContentType = nameValueCollection.Get("Content-Type");
            }
            catch (Exception ex) { ex.DebugLog(); }
        }

        private void SetValue(int Index) => nameValueCollection.Add(new NameValueCollection() { { jSonHandler.GetJTokenValue(jObject, "data", "createMediaUploadLease", "uploadLease", "uploadLeaseHeaders", Index, "header"), jSonHandler.GetJTokenValue(jObject, "data", "createMediaUploadLease", "uploadLease", "uploadLeaseHeaders", Index, "value") } });
        JsonJArrayHandler jSonHandler { get; set; }
        JObject jObject { get; set; }
        public NameValueCollection nameValueCollection { get; set; } = new NameValueCollection();
        public string UploadedImageUrl { get; set; }
        public string Acl { get; set; }
        public string Key { get; set; }
        public string XAmzCredential { get; set; }
        public string XAmzAlgorithm { get; set; }
        public string XAmzDate { get; set; }
        public string SuccessActionStatus { get; set; }
        public string XAmzStorageClass { get; set; }
        public string ContentType { get; set; }
        public string XAmzMetaExt { get; set; }
        public string Policy { get; set; }
        public string XAmzSignature { get; set; }
        public string XAmzSecurityToken { get; set; }
        public string Bucket { get; set; }
        public string UrlPath { get; set; }
    }
}