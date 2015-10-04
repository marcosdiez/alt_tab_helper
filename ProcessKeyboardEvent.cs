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

            for (var i = 0 ; i < listOfOpenWindows.Length; i++)
            {
                if(foregroundWindow == listOfOpenWindows[i])
                {
                    var delta = isShiftDown ? -1 : 1;
                    var nextOrLast = (i + delta) % listOfOpenWindows.Length;
                    if (nextOrLast < 0)
                    {
                        nextOrLast = listOfOpenWindows.Length - 1;
                    }

                    var windowHandle = listOfOpenWindows[nextOrLast];
                    if (!OpenWindowGetter.IsZoomed(windowHandle))
                    {
                        OpenWindowGetter.ShowWindowAsync(windowHandle, (int)OpenWindowGetter.ShowWindowEnum.ShowDefault);
                        OpenWindowGetter.ShowWindowAsync(windowHandle, (int)OpenWindowGetter.ShowWindowEnum.Show);
                    }
                    OpenWindowGetter.SetForegroundWindow(windowHandle);
                }
            }
        }

      


    }
}
