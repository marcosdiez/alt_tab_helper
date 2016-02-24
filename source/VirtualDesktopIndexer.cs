using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    public static class VirtualDesktopIndexer
    {
        public static void VDCheckTimer_Tick()
        {
            /* 
            I really wish this did not have to be activated via a timer.
            But unfortunatelly, well, GetWindowDesktopId crashes if called though the interrupt.
            So I guess this is the way we will do :(
            */
            if (HookManager.workToBeDone)
            {
                HookManager.workToBeDone = false;
                var foregroundWindow = OpenWindowGetter.GetForegroundWindow();
                if (OpenWindowGetter.IsZoomed(foregroundWindow))
                {
                    var currentDesktop = OpenWindowGetter.GetWindowDesktopId(foregroundWindow);

                    var listOfOpenWindows = OpenWindowGetter.GetInterestingOpenWindows();
                    foreach(var thisWindow in listOfOpenWindows)
                    {
                        if(currentDesktop == OpenWindowGetter.GetWindowDesktopId(thisWindow))
                        {
                            OpenWindowGetter.SetActiveWindow(thisWindow);
                            OpenWindowGetter.SetForegroundWindow(thisWindow);
                        }
                    }
                }
            }
        }
    }
}
