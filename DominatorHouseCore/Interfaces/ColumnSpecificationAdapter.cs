#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace DominatorHouseCore.Interfaces
{
    internal class ColumnSpecificationAdapter : IColumnSpecificationProvider
    {
        private readonly List<string> names;

        public ColumnSpecificationAdapter(IAccountCountFactory misnamed_factory)
        {
            names = new List<string>();
            if (misnamed_factory.HeaderColumn5Visiblity) names.Add(misnamed_factory.HeaderColumn5Value);
            if (misnamed_factory.HeaderColumn1Visiblity) names.Add(misnamed_factory.HeaderColumn1Value);
            if (misnamed_factory.HeaderColumn2Visiblity) names.Add(misnamed_factory.HeaderColumn2Value);
            if (misnamed_factory.HeaderColumn3Visiblity) names.Add(misnamed_factory.HeaderColumn3Value);
            if (misnamed_factory.HeaderColumn4Visiblity) names.Add(misnamed_factory.HeaderColumn4Value);
        }

        public ColumnSpecificationAdapter(IColumnSpecificationProvider prov1, IColumnSpecificationProvider prov2)
        {
            names = prov1.VisibleHeaders
                .Concat(prov2.VisibleHeaders)
                .GroupBy(k => k)
                .Select(grp => grp.Key)
                .ToList();
        }

        public IEnumerable<string> VisibleHeaders => names.ToArray();
    }
}