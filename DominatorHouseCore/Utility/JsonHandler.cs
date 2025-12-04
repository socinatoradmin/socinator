#region

using System;
using Newtonsoft.Json.Linq;

#endregion

namespace DominatorHouseCore.Utility
{
    public class JsonHandler:IDisposable
    {
        private readonly JObject _jObject;
        private static readonly object _lock = new object();
        private static volatile JsonHandler Instance;
        public static JsonHandler GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (_lock)
                    {
                        if (Instance == null)
                            Instance = new JsonHandler("{}");
                    }
                }
                return Instance;
            }
        }
        public JObject ParseJsonToJsonObject(string JsonString, JToken jToken = null)
        {
            try
            {
                if (jToken != null)
                    return (JObject)jToken;
                return JObject.Parse(JsonString);
            }
            catch (Exception ex) { ex.DebugLog(); return new JObject(); }
        }
        public JsonHandler(string jsonString)
        {
            try
            {
                _jObject = JObject.Parse(jsonString);
            }
            catch { }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public JsonHandler(JToken jToken)
        {
            _jObject = (JObject)jToken;
        }

        public string GetElementValue(params object[] elementsNameList)
        {
            try
            {

                if (_jObject == null || elementsNameList == null || elementsNameList.Length == 0)
                    return string.Empty;
                var tempToken = _jObject[elementsNameList[0]];
                if (tempToken != null)
                {
                    for (var index = 1; index < elementsNameList.Length; index++)
                        tempToken = tempToken[elementsNameList[index]];
                }
                return tempToken == null ? "" : tempToken.ToString();
            }
            catch (Exception)
            {
                // Ignored  ex.DebugLog();
            }

            return string.Empty;
        }

        public string GetJTokenValue(JToken gotToken, params object[] elementsNameList)
        {
            var elementValue = string.Empty;
            try
            {
                for (var index = 0; index < elementsNameList.Length && gotToken != null; index++)
                {
                    var tempToken = elementsNameList[index];
                    if (index == elementsNameList.Length - 1 && tempToken != null)
                        return elementValue = gotToken[tempToken]?.ToString() ?? "";
                    else if (tempToken != null)
                        gotToken = gotToken[tempToken];
                    else break;
                }
            }
            catch (Exception)
            {
                // Ignored ex.DebugLog();
            }

            return elementValue;
        }

        public JToken GetJToken(params object[] elementsNameList)
        {
            try
            {
                var tempToken = _jObject[elementsNameList[0]];
                for (var index = 1; index < elementsNameList.Length && tempToken != null; index++)
                    tempToken = tempToken[elementsNameList[index]];
                return tempToken ?? new JArray();
            }
            catch (Exception)
            {
                // Ignored  ex.DebugLog();
            }

            return new JArray();
        }

        public JToken GetJTokenOfJToken(JToken gotToken, params object[] elementsNameList)
        {
            try
            {
                var returnToken = gotToken;
                foreach (var element in elementsNameList)
                {
                    if (returnToken == null) break;
                    returnToken = returnToken[element];
                }

                return returnToken ?? new JArray();
            }
            catch (Exception)
            {
                // Ignored ex.DebugLog();
            }

            return new JArray();
        }

        public void Dispose()
        {
            try
            {
                Instance = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { }
        }
    }
}