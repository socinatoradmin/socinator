using CefSharp;
using DominatorHouseCore.Utility;
using System;

namespace EmbeddedBrowser.BrowserHelper
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int Refresh = 1;
        private const int Back = 2;
        private const int Forward = 3;
        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            model?.Clear();
#if DEBUG
            model.AddItem(CefMenuCommand.CustomFirst, "Inspect");
            model.AddItem(CefMenuCommand.ViewSource, "View Page Source");
            model.AddItem(CefMenuCommand.CustomLast, "Save As Pdf");
#endif
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == Refresh) browser.Reload();
            if ((int)commandId == Back) browser.GoBack();
            if ((int)commandId == Forward) browser.GoForward();
#if DEBUG
            if (commandId == CefMenuCommand.ViewSource)
            {
                browser.ViewSource();
                return true;
            }
            else if (commandId == CefMenuCommand.CustomLast)
            {
                var path = $"{ConstantVariable.GetDebugActivityDirectory()}\\{ConstantVariable.ApplicationName}_{DateTime.Now.Ticks.ToString()}_{Utilities.GetGuid()}.pdf";
                browser.PrintToPdfAsync(path, null);
                return true;
            }
            else if (commandId == CefMenuCommand.CustomFirst)
            {
                browser.ShowDevTools(null, parameters.XCoord, parameters.YCoord);
                return true;
            }
            else
                return false;
#else
            return false;
#endif
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}