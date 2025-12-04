using System;
using System.Windows.Controls;
using DominatorHouseCore;

namespace DominatorUIUtility.Behaviours
{
    public class ViewUtilites
    {
        public static void OpenContextMenu(object sender)
        {
            try
            {
                var contextMenu = ((Button) sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button) sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}