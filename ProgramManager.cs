using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    class ProgramManager
    {
        public static void populateListBox(ListBox listBox)
        {
            var theList = GetAppList();

            var items = listBox.Items;
            items.Clear();
            foreach (var item in theList)
            {
                items.Add(item);
            }
        }

        public static List<ProgramItem> GetAppList()
        {
            var theList = new List<ProgramItem>();

            foreach (var proc in Process.GetProcesses())
            {
                if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                {

                    theList.Add(new ProgramItem(proc));
                }
            }
            theList = theList.OrderBy(o => o.ToString()).ToList();
            return theList;
        }

        public static void EventIsTriggered(bool isShiftDown)
        {
            var listOfOpenWindows = OpenWindowGetter.GetOpenWindows().OrderBy(x => x.ToInt64()).ToArray();
            
            if (listOfOpenWindows.Count() < 2)
            {
                return;
            }

            
            var foregroundWindow = ProgramItem.GetForegroundWindow();

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
                    if (!ProgramItem.IsZoomed(windowHandle))
                    {
                        ProgramItem.ShowWindowAsync(windowHandle, (int)ProgramItem.ShowWindowEnum.ShowDefault);
                        ProgramItem.ShowWindowAsync(windowHandle, (int)ProgramItem.ShowWindowEnum.Show);
                    }
                    ProgramItem.SetForegroundWindow(windowHandle);
                }
            }
        }
        
        private static List<ProgramItem> GetAppListFromPid(uint pid)
        {
            
            var processList = Process.GetProcesses();
            var processName = GetProcessNameFromPid(pid);
            List<ProgramItem> theList = GetAllProcessesWithThatName(processList, processName);
            return theList;
        }

        private static List<ProgramItem> GetAllProcessesWithThatName(Process[] processList, string processName)
        {
            var theList = new List<ProgramItem>();

            foreach (var proc in processList)
            {
                if (proc.ProcessName == processName)
                {
                    theList.Add(new ProgramItem(proc));
                }
            }

            theList = theList.OrderBy(o => o.GetPid()).ToList();
            return theList;
        }

        private static string GetProcessNameFromPid(uint pid)
        {
            return OpenWindowGetter.GetProcessNameFromPid(pid);
        }

        public static uint GetCurrentPid()
        {
            var windowInForeground = ProgramItem.GetForegroundWindow();
            uint pid;
            var result = ProgramItem.GetWindowThreadProcessId(windowInForeground, out pid);
            return pid;
        }


    }
}
