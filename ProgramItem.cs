using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AltTabHelperV2
{
    public class ProgramItem
    {
        Process process;

        public Process GetProcess()
        {
            return process;
        }
        public ProgramItem(Process process)
        {
            this.process = process;
        }
        
        public override String ToString()
        {
            return String.Format("{0,-30} - {1,-5} - {2}", process.ProcessName, process.Id, process.MainWindowTitle);
        }

        public void SetFocus()
        {
            var handle = process.MainWindowHandle;

            if ((int)handle == 0)
            {
                ShowWindow(process.Handle, (int)ShowWindowEnum.Restore);
            }
            SetForegroundWindow(process.MainWindowHandle);
        }

        public int GetPid()
        {
            return process.Id;
        }


        public enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint windowStyle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetActiveWindow(IntPtr hWnd);


        // The return value is the handle to the active window attached to the calling thread's message queue. Otherwise, the return value is NULL. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        // Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        // Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hwnd);
    }
}
                                            