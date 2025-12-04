using System.Runtime.InteropServices;
using System.Text;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public class KeyCodeHelper
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetKeyNameText(uint lParam, [Out] StringBuilder lpString, int nSize);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        private const uint MAPVK_VK_TO_VSC = 0;

        /// <summary>
        /// Gets the name of the keyboard key associated with the specified <paramref name="scanCode"/>.
        /// </summary>
        /// <param name="scanCode">The scan code of the keyboard key to get the name of.</param>
        /// <returns>The name of the key as a <see cref="string"/>.</returns>
        public static string FromScanCode(uint scanCode)
        {
            var sb = new StringBuilder(32);
            GetKeyNameText(scanCode << 16, sb, 32);
            return sb.ToString();
        }

        /// <summary>
        /// Gets the name of the keyboard key associated with the specified <paramref name="virtualKeyCode"/>.
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code of the keyboard key to get the name of.</param>
        /// <returns>The name of the key as a <see cref="string"/>.</returns>
        public static string FromVirtualKey(uint virtualKeyCode)
        {
            return FromScanCode(MapVirtualKey(virtualKeyCode, MAPVK_VK_TO_VSC));
        }
    }
}
