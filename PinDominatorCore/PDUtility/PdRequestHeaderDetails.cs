using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PinDominatorCore.PDUtility
{
    public class PdRequestHeaderDetails
    {
        private static JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        public string AppVersion { get; set; } = string.Empty;
        public string ExperimentHash { get; set; } = string.Empty;
        public string BoardID { get; set; } = string.Empty;
        public string ProfileID { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PinID { get; set; } = string.Empty;
        public string AggregatedPinID { get; set; } = string.Empty;
        public JToken jToken { get; set; }
        private static PdRequestHeaderDetails Instance;
        public static string GetJsonString(string PageResponse,bool IsNewUI=false)
        {
            Instance = Instance ?? (Instance = new PdRequestHeaderDetails());
            return Instance.GetJsonResponseFromPagesource(PageResponse, IsNewUI);
        }
        public static PdRequestHeaderDetails GetRequestHeader(string Response, TokenDetailsType tokenDetailsType = TokenDetailsType.Boards)
        {
            var RequestHeaderDetails = new PdRequestHeaderDetails();
            try
            {
                var jsonData = RequestHeaderDetails.GetJsonResponseFromPagesource(Response);
                var jObject = handler.ParseJsonToJObject(jsonData);
                RequestHeaderDetails.AppVersion = handler.GetJTokenValue(jObject, "appVersion");
                jsonData = RequestHeaderDetails.GetJsonResponseFromPagesource(Response,true);
                jObject = handler.ParseJsonToJObject(jsonData);
                var tokenType = tokenDetailsType == TokenDetailsType.Boards ? "boards" :
                    tokenDetailsType == TokenDetailsType.Users ? "users" :
                    tokenDetailsType == TokenDetailsType.Pins ? "pins":"session";
                RequestHeaderDetails.jToken = tokenDetailsType != TokenDetailsType.Session ?
                    handler.GetJTokenOfJToken(jObject, "initialReduxState", tokenType)?.FirstOrDefault()?.FirstOrDefault():
                    handler.GetJTokenOfJToken(jObject, "initialReduxState", tokenType);
                if (RequestHeaderDetails.jToken == null)
                {
                    jsonData = RequestHeaderDetails.GetJsonResponseFromPagesource(Response);
                    jObject = handler.ParseJsonToJObject(jsonData);
                    RequestHeaderDetails.jToken = handler.GetJTokenOfJToken(jObject, "resource_response","data");
                }

                var ID = handler.GetJTokenValue(RequestHeaderDetails.jToken, "id");
                if (tokenDetailsType == TokenDetailsType.Boards)
                    RequestHeaderDetails.BoardID = ID;
                if (tokenDetailsType == TokenDetailsType.Users)
                {
                    RequestHeaderDetails.ProfileID = ID;
                    RequestHeaderDetails.Username = handler.GetJTokenValue(RequestHeaderDetails.jToken, "username");
                }
                if (tokenDetailsType == TokenDetailsType.Pins)
                {
                    RequestHeaderDetails.PinID = ID;
                    RequestHeaderDetails.AggregatedPinID = handler.GetJTokenValue(RequestHeaderDetails.jToken, "aggregated_pin_data", "id");
                }
            }
            catch { }
            return RequestHeaderDetails;
        }

        public string GetJsonResponseFromPagesource(string pageSource, bool UpdatedUI = false)
        {
            var jsonData = string.Empty;
            try
            {
                if (UpdatedUI)
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.NewJsonPwsData, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.ApplicationJsonDoubleInitialStateSingle, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                {
                    if (!string.IsNullOrEmpty(Utilities.GetBetween(pageSource, "script data-test-id=\"resource-response-data\" type=\"application/json\">{\"resource\":{\"name\":\"BoardResource", "</script>")))
                        jsonData = "{\"resource\":{\"name\":\"BoardResource" + Utilities.GetBetween(pageSource, "script data-test-id=\"resource-response-data\" type=\"application/json\">{\"resource\":{\"name\":\"BoardResource", "</script>");
                }
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.ApplicationJsonDoubleInitialStateDouble, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.InitialStateDoubleApplicationJsonDouble, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.ApplicationJsonDoubleJsInitSingle, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.ApplicationJsonDoubleJsInitDouble, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script>");
                if (string.IsNullOrEmpty(jsonData))
                {
                    jsonData = Utilities.GetBetween(pageSource, PdConstants.WindowJsonString, PdConstants.Script);
                    if (!string.IsNullOrEmpty(jsonData) && jsonData.StartsWith("JSON.parse"))
                    {
                        jsonData = jsonData.Replace("JSON.parse(", "").TrimEnd(';');
                        jsonData = jsonData.TrimEnd(')');
                        jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData).ToString();
                    }
                }
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Regex.Match(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">{1}.*\"site\":\"www\"}{1}").Value?.Replace("script id=\"__PWS_DATA__\" type=\"application/json\">", "");
                if (string.IsNullOrEmpty(jsonData))
                {
                    var match = Regex.Match(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">{1}.*\"site\":\"www\"{1}").Value?.Replace("script id=\"__PWS_DATA__\" type=\"application/json\">", "");
                    if(!string.IsNullOrEmpty(match))
                        jsonData = match + "}";
                }
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script><link data-chunk=\"");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script><script nonce=");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "type=\"application/json\">", "</script><link");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script><!-- -->");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script><script id=\"__LOADABLE_REQUIRED_CHUNKS__\" type=\"application/json\"");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(pageSource, "type=\"application/json\">", "</script><script id=");
                if (!string.IsNullOrEmpty(jsonData) && jsonData.Contains("</script><script nonce="))
                    jsonData = Regex.Replace(jsonData, "</script><script nonce={1}.*", "");
                if (string.IsNullOrEmpty(jsonData) && pageSource.Contains("\"resource_response\":"))
                    jsonData = pageSource;
                jsonData = jsonData.Contains("\"site\":\"www\"") ? Regex.Replace(jsonData, "\"site\":\"www\"{1}(.*)", "") + "\"site\":\"www\"}" : jsonData;
            }
            catch (Exception)
            {
                // ignored
            }

            return jsonData;
        }
    }
    public enum TokenDetailsType
    {
        Pins=1,
        Users=2,
        Boards=3,
        Session = 4,

    }
}
