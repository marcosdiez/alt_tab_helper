using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    class ProcessKeyboardEvent
    {

        public static void EventIsTriggered(bool isShiftDown)
        {
            var listOfOpenWindows = OpenWindowGetter.GetOpenWindows().OrderBy(x => x.ToInt64()).ToArray();

            if (listOfOpenWindows.Count() < 2)
            {
                return;
            }

            var foregroundWindow = OpenWindowGetter.GetForegroundWindow();
            //foreach( var proc in listOfOpenWindows)
            //{
            //    Console.WriteLine(OpenWindowGetter.GetProcessInfo(proc));
            //}

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
                    // this is a very very very ugly hack but it seems to remove all the limitations described here:
                    // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo 
                    for (var z = 0; z < 100; z++)
                    {
                        OpenWindowGetter.SetActiveWindow(windowHandle);
                        OpenWindowGetter.SetForegroundWindow(windowHandle);
                    }
                }
            }
        }
    }
}
