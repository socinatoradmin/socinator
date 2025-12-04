using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum MarketplaceCategory
    {
        [Description("LangKeyAllMarketplace")]
        AllMarketplace = 1,

        [Description("LangKeyVehicles")]
        Vehicles = 1006,

        [Description("LangKeyHomeAndGarden")]
        HomeAndGarden = 1013,

        [Description("LangKeyHousingRent")]
        HousingRent = 1009,

        [Description("LangKeyHousingSale")]
        HousingSale = 1009,

        [Description("LangKeyEntertainment")]
        Entertainment = 1007,

        [Description("LangKeyClothingAndAccessories")]
        ClothingAndAccessories = 1007,

        [Description("LangKeyFamily")]
        Family = 1007,

        [Description("LangKeyHobbies")]
        Hobbies = 1019,

        [Description("LangKeyClassifieds")]
        Classifieds = 2612,

        [Description("LAngKeyDeals")]
        Deals = 1007
    }


    public enum MarketplacePropertyType
    {
        [Description("LangKeyAll")]
        All = 1,

        [Description("LangKeyHouse")]
        House = 1006,

        [Description("LangKeyFlat")]
        Flat = 1013,

        [Description("LangKeyTownHouse")]
        TownHouse = 1009,

        [Description("LangKeyApartment")]
        Apartment = 1009,

        [Description("LangKeyPrivateRoom")]
        PrivateRoom = 1009,

        [Description("LangKeySharedRoom")]
        SharedRoom = 1009
    }
}
