using System;
using System.Runtime.InteropServices;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public class PuppeteerWIN32
    {
        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
        [DllImport("user32.dll", SetLastError = false)] public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }
    }
}
