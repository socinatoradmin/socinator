#region

using System;
using System.Threading;
using AutoIt;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Utility
{
    public class AutoItTool
    {
        public double WidthRatio = 1;
        public double HeightRatio = 1;

        //public AutoItTool()
        //{
        //    //ClUtility.GetScreenResolution(ref HeightRatio, ref WidthRatio);
        //    ////For Temporary purpose setting value 1.00
        //    //WidthRatio = 1.00;
        //    //HeightRatio = 1.00;
        //}

        public void WinActivate(string name, double delayBefore = 0, double delayAfter = 0)
        {
            var win = AutoItX.WinActive(name);
            if (win == 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
                AutoItX.WinActivate(name);
                Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
            }
        }

        public void WinActivate(IntPtr intPtr, double delayBefore = 0, double delayAfter = 0)
        {
            var win = AutoItX.WinActive(intPtr);
            if (win == 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
                AutoItX.WinActivate(intPtr);
                Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
            }
        }

        public string GetLastCopied()
        {
            return AutoItX.ClipGet();
        }

        public void CopyToClip(string text)
        {
            AutoItX.ClipPut(text);
        }

        public void MouseClick(MouseKeys key = MouseKeys.Left, int count = 1, double delayBetween = 0.5,
            double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            var iteration = 0;
            while (iteration < count)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBetween));
                AutoItX.MouseClick($"{key.ToString().ToUpper()}");
                iteration++;
            }

            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void TypeString(string parameter, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.Send(parameter);
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }
    }
}