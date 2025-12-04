using Newtonsoft.Json.Linq;
using System;

namespace TwtDominatorCore.TDUtility
{
    public class JsonJArrayHandler:IDisposable
    {
        private static volatile JsonJArrayHandler Instance;
        private static readonly object _lock = new object();
        public static JsonJArrayHandler GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (_lock)
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
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return jArray;
        }

        public string GetJTokenValue(JToken gotToken, params object[] elementsNameList)
        {
            try
            {
                for (var index = 0; index < elementsNameList.Length && gotToken != null; index++)
                {
                    gotToken = gotToken[elementsNameList[index]];

                    if (gotToken == null)
                        return string.Empty;
                }

                // Once loop is done, gotToken should be the final value (JValue or something else)
                return gotToken != null ? gotToken.Type == JTokenType.Null ? string.Empty : gotToken.ToString():string.Empty;
            }
            catch (Exception)
            {
                // Optionally log the exception here
                return string.Empty;
            }
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
