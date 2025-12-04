#region

using System;
using System.Windows.Controls;

#endregion

namespace DominatorHouseCore.Models
{
    public class TabItemTemplates
    {
        public string Title { get; set; }

        public Lazy<UserControl> Content { get; set; }
    }
}