using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GramDominatorCore.GDModel
{
    public class ExternalMetadata
    {
        private string caption;

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
                if (caption == null)
                    return;
                ExtractHashTags();
            }
        }

        public List<HashTag> HashTags { get; } = new List<HashTag>();

       public Location Location { get; set; }

       // public List<UserTag> UserTags { get; set; }

        private void ExtractHashTags()
        {
            HashTags.Clear();
            foreach (string str in Regex.Matches(Caption, "(?<=#)(\\w+)").Cast<Match>().Select(match => match.Value).ToList())
               HashTags.Add(new HashTag()
                {
                    TagName = str,
                    X = 1,
                    Y = 1,
                    Width = 1,
                    Height = 1,
                    Rotation = 0,
                    IsSticker = false,
                    UseCustomTitle = false
                });
        }

        public class HashTag
        {
            [JsonProperty(PropertyName = "height")]
            public int Height { get; set; }

            [JsonProperty(PropertyName = "is_sticker")]
            public bool IsSticker { get; set; }

            [JsonProperty(PropertyName = "rotation")]
            public int Rotation { get; set; }

            [JsonProperty(PropertyName = "tag_name")]
            public string TagName { get; set; }

            [JsonProperty(PropertyName = "use_custom_title")]
            public bool UseCustomTitle { get; set; }

            [JsonProperty(PropertyName = "width")]
            public int Width { get; set; }

            [JsonProperty(PropertyName = "x")]
            public int X { get; set; }

            [JsonProperty(PropertyName = "y")]
            public int Y { get; set; }
        }

        public class UserTag
        {
            [JsonProperty(PropertyName = "user_id")]
            public string Pk { get; set; }

            [JsonProperty(PropertyName = "position")]
            public double[] Position { get; set; }
        }
    }
}
