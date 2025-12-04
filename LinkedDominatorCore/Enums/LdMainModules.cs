namespace LinkedDominatorCore.Enums
{
    public enum LdMainModules
    {
        GrowConnection,
        Messenger,
        Engage,
        Group,
        Scraper,
        Profilling,
        SalesNavigatorScraper
    }

    public enum WhitelistblacklistType
    {
        Group = 1,
        Private = 2,
        Both = 3
    }

    public class LdQueryTypes
    {
        public const string SalesNavigatorSearchUrl = "Sales Navigator SearchUrl";
    }

    public class AttributeTypes
    {
        public const string Button = "button";

        public const string ArtdecoDropdownItem = "artdeco-dropdown-item";
    }
}