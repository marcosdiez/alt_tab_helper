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

                var processName = GetProcessNameFromHwnd(hWnd, builder2);

                if (processName == currentProcess)
                {
                    windowList.Add(hWnd);
                }
                return true;

            }, 0);

            return windowList;
        }

        private static string GetProcessNameFromHwnd(HWND hWnd, StringBuilder builder2)
        {
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);

            var newHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            GetModuleFileNameEx(newHandle, IntPtr.Zero, builder2, builder2.Capacity);
            CloseHandle(newHandle);
            return builder2.ToString();
        }

        public static string GetCurrentProcessName()
        {
            return GetProcessNameFromPid(GetCurrentPid());
        }

        public static string GetProcessNameFromPid(uint pid)
        {
            var maxSize = 1000;
            var output = new StringBuilder(maxSize);

            var newHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            GetModuleFileNameEx(newHandle, IntPtr.Zero, output, output.Capacity);
            CloseHandle(newHandle);

            return output.ToString();
        }

        public static uint GetCurrentPid()
        {
            var windowInForeground = GetForegroundWindow();
            uint pid;
            var result = GetWindowThreadProcessId(windowInForeground, out pid);
            return pid;
        }

        public static IDictionary<HWND, string> GetOpenWindows2()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
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

    }

}
