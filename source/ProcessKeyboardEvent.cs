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
        public static void ControlEventIsTriggered(bool isShiftDown)
        {
            var output = "";

            output += ("-- AAxCONTROL EVENT TRIGGERED ---");
            var foregroundWindow = OpenWindowGetter.GetForegroundWindow();
            if (OpenWindowGetter.IsZoomed(foregroundWindow))
            {
                output += ("-- Janela em Foco Está maximizada ---");
                var listOfOpenWindows = OpenWindowGetter.GetAllOpenWindows().OrderBy(x => x.ToInt64()).ToArray();
                for (var i = 0; i < listOfOpenWindows.Length; i++)
                {
                    var thisWindow = listOfOpenWindows[i];

                    Console.WriteLine("Chaning the size of " +  OpenWindowGetter.GetProcessInfo(thisWindow));

                    //if (thisWindow != foregroundWindow &&
                    //    !OpenWindowGetter.IsIconic(thisWindow) &&
                    //    !OpenWindowGetter.IsZoomed(thisWindow) 
                    //    )
                    //{
                    //    Console.WriteLine("Chaning the size of " + OpenWindowGetter.GetProcessInfo(thisWindow));
                    //    // the window is not the main, is not maximized nor minimized;
                    //    OpenWindowGetter.SetActiveWindow(thisWindow);
                    //    OpenWindowGetter.SetForegroundWindow(thisWindow);
                    //}
                }
                output += ("-- BBxCONTROL EVENT TRIGGERED ---");
                File.WriteAllText("z:\\tmp\\output.txt", output);
            }
            

        }


        public static void EventIsTriggered(bool isShiftDown)
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
