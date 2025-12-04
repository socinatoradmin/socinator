#region

using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    ///     IAccountCountFactory is used to specify the account count details with their respective visiblity
    ///     such as for instagram - FollowerCount (HeaderColumn1Value) , FollowingCount (HeaderColumn2Value) ,
    ///     PostCount(HeaderColumn3Value)
    ///     NOTE : HeaderColumn1Value is available for all dominator
    /// </summary>
    public interface IAccountCountFactory
    {
        string HeaderColumn1Value { get; set; }

        bool HeaderColumn1Visiblity { get; set; }

        string HeaderColumn2Value { get; set; }

        bool HeaderColumn2Visiblity { get; set; }

        string HeaderColumn3Value { get; set; }

        bool HeaderColumn3Visiblity { get; set; }

        string HeaderColumn4Value { get; set; }

        bool HeaderColumn4Visiblity { get; set; }

        string HeaderColumn5Value { get; set; }

        bool HeaderColumn5Visiblity { get; set; }
    }

    public interface IColumnSpecificationProvider
    {
        IEnumerable<string> VisibleHeaders { get; }
    }

    public static class ColumnSpecificationGetter
    {
        public static IColumnSpecificationProvider GetColumnSpecificationProvider(
            this IAccountCountFactory misnamed_factory)
        {
            if (typeof(IColumnSpecificationProvider).IsAssignableFrom(misnamed_factory.GetType()))
                return (IColumnSpecificationProvider) misnamed_factory;
            return new ColumnSpecificationAdapter(misnamed_factory);
        }

        public static IColumnSpecificationProvider Combine(this IColumnSpecificationProvider prov1,
            IColumnSpecificationProvider prov2)
        {
            return new ColumnSpecificationAdapter(prov1, prov2);
        }
    }
}