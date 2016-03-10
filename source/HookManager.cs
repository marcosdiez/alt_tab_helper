using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltTabHelperV2
{

    // most of this code is from http://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
    public partial class HookManager
    {
        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private static HookProc s_KeyboardDelegate;

        private static int s_KeyboardHookHandle = 0;

        public static void EnsureSubscribedToGlobalKeyboardEvents()
        {
            if (s_KeyboardHookHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                s_KeyboardDelegate = KeyboardHookProc;

                s_KeyboardHookHandle = SetWindowsHookEx(
                      WH_KEYBOARD_LL,
                      s_KeyboardDelegate,
                      IntPtr.Zero,
                        // Marshal.GetHINSTANCE(                            Assembly.GetExecutingAssembly().GetModules()[0]),
                        0);


                if (s_KeyboardHookHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }


        

        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            if (nCode >= 0)
            {
                if ((wParam == WM_SYSKEYDOWN)) // raise KeyDown with alt pressed
                {
                    //read structure KeyboardHookStruct at lParam
                    var MyKeyboardHookStruct = (KeyboardHookStruct)
                        Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                    if ((Keys)MyKeyboardHookStruct.VirtualKeyCode == Keys.Oem3) // 0xC0 -> `~
                    {
                        
                        var isDownCtrl = (GetKeyState(VK_CONTROL) & 0x8000) == 0x8000;
                        if (isDownCtrl)
                        {
                            Task.Run((Action)
                                ProcessKeyboardEvent.PutNonMaximizedWindowsInThisDesktopOnTop
                                );
                        }
                        else {
                            var isDownShift = (GetKeyState(VK_SHIFT) & 0x8000) == 0x8000;
                            Task.Run(() =>
                            {
                                ProcessKeyboardEvent.GiveFocusToNextSimilarWindow(isDownShift);
                            });
                        }
                        return -1;
                    }
                }
            }

            //forward to other application
            return CallNextHookEx(s_KeyboardHookHandle, nCode, wParam, lParam);
        }

        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
    }
}
