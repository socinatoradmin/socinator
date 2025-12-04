using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum FanpageCategory
    {
        [Description("Any category")]
        AnyCategory = 1,

        [Description("Local Business or Place")]
        LocalBusinessorPlace = 1006,

        [Description("Company Organization or Institution")]
        CompanyOrganizationorInstitution = 1013,

        [Description("Brand or Product")]
        BrandorProduct = 1009,

        [Description("Artist Band or Public Figure")]
        ArtistBandorPublicFigure = 1007,

        [Description("Entertainment")]
        Entertainment = 1019,

        [Description("Cause or Community")]
        CauseorCommunity = 2612
    }

    public enum FanpageCategoryWeb
    {
        [Description("Any category")]
        AnyCategory = 0,

        [Description("Local Business or Place")]
        LocalBusinessorPlace = 1,

        [Description("Company Organization or Institution")]
        CompanyOrganizationorInstitution = 2,

        [Description("Brand or Product")]
        BrandorProduct = 3,

        [Description("Artist Band or Public Figure")]
        ArtistBandorPublicFigure = 4,

        [Description("Entertainment")]
        Entertainment = 5,

        [Description("Cause or Community")]
        CauseorCommunity = 6
    }
}
