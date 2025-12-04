using Newtonsoft.Json.Linq;
using System;

namespace PinDominatorCore.Utility
{
    public class JsonJArrayHandler:IDisposable
    {
        private static readonly object Lock = new object();
        private static volatile JsonJArrayHandler Instance;
        public static JsonJArrayHandler GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (Lock)
                    {
                        if (Instance == null)
                            Instance = new JsonJArrayHandler();
                    }
                }
                return Instance;
            }
        }
        public JToken GetTokenElement(JObject jObject, string elementName)
        {
            JToken jToken = null;
            try
            {
                jToken = jObject[elementName];
            }
            catch (Exception)
            {
                //ignored
            }

            return jToken;
        }
        public bool IsValidJson(string JsonResponse)
        {
            try
            {
                JObject.Parse(JsonResponse);
                return true;
            }
            catch { return false; }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public JObject ParseJsonToJObject(string jsonResponse)
        {
            try
            {
                return JObject.Parse(jsonResponse);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public JToken GetTokenElement(JToken jTokenData, string elementName)
        {
            JToken jToken = null;
            try
            {
                jToken = jTokenData[elementName];
            }
            catch (Exception)
            {
                //ignored
            }

            return jToken;
        }


        public JToken GetTokenElement(JToken jTokenData, params object[] elementsNameList)
        {
            try
            {
                for (var index = 0; index < elementsNameList.Length && jTokenData != null; index++)
                {
                    if (index == elementsNameList.Length - 1)
                        return jTokenData = jTokenData[elementsNameList[index]];
                    jTokenData = jTokenData[elementsNameList[index]];
                }
            }
            catch (Exception)
            {
                // Ignored 
            }

            return jTokenData;
        }

        public JArray GetJArrayElement(string jData)
        {
            JArray jArray = null;
            try
            {
                jArray = JArray.Parse(jData);
            }
            catch (Exception)
            {
                //ignored
            }

            return jArray;
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
                // Ignored 
            }

            return elementValue;
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
