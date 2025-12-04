using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace LinkedDominatorCore.LDModel
{
    public class MediaUploadHeader
    {
        [JsonProperty("access-control-allow-origin")]
        public string AccessControlAllowOrigin { get; set; } = "https://www.linkedin.com";
        [JsonProperty("cache-control")]
        public string CacheControl { get;set; }
        [JsonProperty("content-length")]
        public string ContentLength {  get; set; }
        [JsonProperty("content-security-policy")]
        public string ContentSecurityPolicy {  get; set; }
        [JsonProperty("date")]
        public string Date {  get; set; }
        [JsonProperty("expires")]
        public string Expires {  get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("pragma")]
        public string Pragma {  get; set; }
        [JsonProperty("server-timing")]
        public string ServerTiming { get; set; } = "sxdkHLqc;desc=\"eyJjdXJyZW50Ijp7ImJyb3dzZXIiOiJjaHJvbWUiLCJvcyI6ImxpbnV4IiwidXNlckFnZW50IjoiTW96aWxsYS81LjAgKFgxMTsgRGViaWFuOyBMaW51eCB4ODZfNjQpIEFwcGxlV2ViS2l0LzUzNy4zNiAoS0hUTUwsIGxpa2UgR2Vja28pIENocm9tZS8xMjcuMC42NjYwLjE5MiBTYWZhcmkvNTM3LjM2IiwidmVyc2lvbiI6eyJicm93c2VyIjp7ImZ1bGwiOiIxMjcuMC42NjYwLjE5MiIsIm1ham9yIjoxMjd9fX0sImJyYW5kcyI6eyJtYWpvciI6W3siYnJhbmQiOiIoTm90KEE6QnJhbmQiLCJ2ZXJzaW9uIjoiOTkifSx7ImJyYW5kIjoiR29vZ2xlIENocm9tZSIsInZlcnNpb24iOiIxMjcifSx7ImJyYW5kIjoiQ2hyb21pdW0iLCJ2ZXJzaW9uIjoiMTI3In1dLCJmdWxsIjpbeyJicmFuZCI6IihOb3QoQTpCcmFuZCIsInZlcnNpb24iOiI5OS4wLjAuMCJ9LHsiYnJhbmQiOiJHb29nbGUgQ2hyb21lIiwidmVyc2lvbiI6IjEyNyJ9LHsiYnJhbmQiOiJDaHJvbWl1bSIsInZlcnNpb24iOiIxMjcifV19LCJwbGF0Zm9ybSI6IkxpbnV4IiwiaXNNb2JpbGUiOmZhbHNlfQ__\"";
        [JsonProperty("strict-transport-security")]
        public string StrictTransportSecurity {  get; set; }
        [JsonProperty("x-ambry-creation-time")]
        public string XAmbryCreationTime {  get; set; }
        [JsonProperty("x-cache")]
        public string XCache {  get; set; }
        [JsonProperty("x-content-type-options")]
        public string XContentTypeOptions {  get; set; }
        [JsonProperty("x-frame-options")]
        public string XFrameOptions { get; set;}
        [JsonProperty("x-li-fabric")]
        public string XLiFabric {  get; set; }
        [JsonProperty("x-li-pop")]
        public string XLIpop {  get; set; }
        [JsonProperty("x-li-proto")]
        public string XLiProto {  get; set; }
        [JsonProperty("x-li-uuid")]
        public string XLiUuid {  get; set; }
        [JsonProperty("x-msedge-ref")]
        public string XMsedgeRef {  get; set; }
    }
    public class HeaderCollection
    {
        [JsonProperty("headers")]
        public MediaUploadHeader uploadHeaders { get; set; } = new MediaUploadHeader();
        [JsonProperty("httpStatusCode")]
        public int StatusCode { get; set; } = 201;
    }
}
