using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;

namespace AltTabHelperV2
{
    class ProcessKeyboardEvent
    {
        public static void PutNonMaximizedWindowsInThisDesktopOnTop()
        {
            var foregroundWindow = OpenWindowGetter.GetForegroundWindow();
            if (OpenWindowGetter.IsZoomed(foregroundWindow))
            {
                var currentDesktop = OpenWindowGetter.GetWindowDesktopId(foregroundWindow);
                var listOfOpenWindows = OpenWindowGetter.GetInterestingOpenWindows();
                foreach (var thisWindow in listOfOpenWindows)
                {
                    if (currentDesktop == OpenWindowGetter.GetWindowDesktopId(thisWindow))
                    {
                        OpenWindowGetter.SetActiveWindow(thisWindow);
                        OpenWindowGetter.SetForegroundWindow(thisWindow);
                    }
                }
            }
        }

        public static void GiveFocusToNextSimilarWindow(bool isShiftDown)
        {
          
            var listOfOpenWindows = OpenWindowGetter.GetOpenWindowsWithTheSameNameAsTheCurrentProcess().OrderBy(x => x.ToInt64()).ToArray();

            if (listOfOpenWindows.Count() < 2)
            {
                return;
            }

            var foregroundWindow = OpenWindowGetter.GetForegroundWindow();

            Console.WriteLine("-------------------------");
            foreach (var proc in listOfOpenWindows)
            {
                Console.WriteLine(OpenWindowGetter.GetProcessInfo(proc));
            }
            Console.WriteLine("-------------------------");

            for (var i = 0; i < listOfOpenWindows.Length; i++)
            {
                if (foregroundWindow == listOfOpenWindows[i])
                {
                    var delta = isShiftDown ? -1 : 1;
                    var nextOrLast = (i + delta) % listOfOpenWindows.Length;
                    if (nextOrLast < 0)
                    {
                        nextOrLast = listOfOpenWindows.Length - 1;
                    }

                    var windowHandle = listOfOpenWindows[nextOrLast];

                    if (OpenWindowGetter.IsIconic(windowHandle)) // minimized
                    {
                        OpenWindowGetter.ShowWindowAsync(windowHandle, (int)OpenWindowGetter.ShowWindowEnum.ShowDefault);
                        OpenWindowGetter.ShowWindowAsync(windowHandle, (int)OpenWindowGetter.ShowWindowEnum.Show);
                    }
                    // According to the URL below, the code bellow does not work unless we are being debugged...:
                    // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo 
                    OpenWindowGetter.SetActiveWindow(windowHandle);
                    OpenWindowGetter.SetForegroundWindow(windowHandle);
                }
            }
        }
    }
}
