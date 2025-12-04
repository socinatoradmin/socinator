using Newtonsoft.Json.Linq;
using System.Linq;

namespace FaceDominatorCore.Utility
{
    public class JsonSearcher
    {
        public static JToken FindByKey(JToken token, string key)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var child in token.Children<JProperty>())
                {
                    if (child.Name == key)
                        return child.Value;

                    var found = FindByKey(child.Value, key);
                    if (found != null && found.Count() != 0)
                        return found;
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var child in token.Children())
                {
                    var found = FindByKey(child, key);
                    if (found != null && found.Count() != 0)
                        return found;
                }
            }
            return new JArray();
        }
        public static string FindStringValueByKey(JToken token, string key)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var child in token.Children<JProperty>())
                {
                    if (child.Name == key && !child.Value.HasValues && child.Value.Next == null)
                        return child.Value.ToString();

                    var found = FindStringValueByKey(child.Value, key);
                    if (!string.IsNullOrEmpty(found))
                        return found.ToString();
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var child in token.Children())
                {
                    var found = FindStringValueByKey(child, key);
                    if (!string.IsNullOrEmpty(found))
                        return found.ToString();
                }
            }
            return string.Empty;
        }
    }
}
