#region

using System;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class Configuration
    {
        [ProtoMember(1)] public DateTime ConfigurationDate { get; set; }

        [ProtoMember(2)] public string ConfigurationType { get; set; }

        [ProtoMember(3)] public string ConfigurationSetting { get; set; }
    }

    [ProtoContract]
    public class Themes
    {
        [ProtoMember(1)] public Theme SelectedTheme { get; set; }

        [ProtoMember(2)] public AccentColors SelectedAccentColor { get; set; }
    }

    public class AccentColors
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public AccentColors(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class Theme
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Theme(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}