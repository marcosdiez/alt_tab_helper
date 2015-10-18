using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltTabHelperV2
{
    using System.Runtime.InteropServices;
    using HWND = IntPtr;

    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter
    {

        // thank you, http://www.tcx.be/blog/2006/list-open-windows/ 

        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_READ = 0x0010;

        public static List<HWND> GetOpenWindows()
        {
            var shellWindow = GetShellWindow();
            var windowList = new List<HWND>();
            var currentProcess = GetCurrentProcessName();
            var builder2 = new StringBuilder(1000);

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
                if (GetWindowTextLength(hWnd) == 0) return true;

                var processName = GetModuleFileName(hWnd, builder2);

                if (processName == currentProcess)
                {
                    windowList.Add(hWnd);
                }
                return true;

            }, 0);

            return windowList;
        }

        public static string GetProcessInfo(HWND hWnd)
        {
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            var length = GetWindowTextLength(hWnd);
            var output = new StringBuilder(1000);
            var output2 = new StringBuilder(1000);
            GetWindowText(hWnd, output, length + 1);

            return String.Format("hWnd: {0} - pid: {1} - exe: {2} - {3}", hWnd.ToInt64(), pid, GetModuleFileName(hWnd, output2), output);


        }
        private static string GetModuleFileName(HWND hWnd, StringBuilder builder2)
        {
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);

            var newHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            GetModuleFileNameEx(newHandle, IntPtr.Zero, builder2, builder2.Capacity);
            CloseHandle(newHandle);
            return builder2.ToString();
        }

        private static string GetCurrentProcessName()
        {
            return GetProcessNameFromPid(GetCurrentPid());
        }

        private static string GetProcessNameFromPid(uint pid)
        {
            var maxSize = 1000;
            var output = new StringBuilder(maxSize);

            var newHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            GetModuleFileNameEx(newHandle, IntPtr.Zero, output, output.Capacity);
            CloseHandle(newHandle);

            return output.ToString();
        }

        private static uint GetCurrentPid()
        {
            var windowInForeground = GetForegroundWindow();
            uint pid;
            var result = GetWindowThreadProcessId(windowInForeground, out pid);
            return pid;
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("psapi.dll")]
        private static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        [DllImport("psapi.dll")]
        private static extern uint GetProcessImageFileName(IntPtr hWnd, StringBuilder lpFileName, int nSize);

        // Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint pid);

        [DllImport("Kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        // Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint windowStyle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetActiveWindow(IntPtr hWnd);

        // The return value is the handle to the active window attached to the calling thread's message queue. Otherwise, the return value is NULL. 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hwnd);

        public enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

    }

}
