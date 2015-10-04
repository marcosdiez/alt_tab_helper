using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AltTabHelperV2
{
    public partial class Hook
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
                // wParam == WM_KEYDOWN || 
                if ((wParam == WM_SYSKEYDOWN)) // raise KeyDown with alt pressed
                {
                    //read structure KeyboardHookStruct at lParam
                    KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                    if ((Keys)MyKeyboardHookStruct.VirtualKeyCode == Keys.Oem3) // 0xC0 -> `~
                    {
                        bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);

                        ProgramManager.EventIsTriggered(isDownShift);



                        return -1;
                    }
                }

                //// raise KeyPress
                //if (s_KeyPress != null && wParam == WM_KEYDOWN)
                //{
                //    bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                //    bool isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                //    byte[] keyState = new byte[256];
                //    GetKeyboardState(keyState);
                //    byte[] inBuffer = new byte[2];
                //    if (ToAscii(MyKeyboardHookStruct.VirtualKeyCode,
                //              MyKeyboardHookStruct.ScanCode,
                //              keyState,
                //              inBuffer,
                //              MyKeyboardHookStruct.Flags) == 1)
                //    {
                //        char key = (char)inBuffer[0];
                //        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                //        KeyPressEventArgs e = new KeyPressEventArgs(key);
                //        s_KeyPress.Invoke(null, e);
                //        handled = handled || e.Handled;
                //    }
                //}

                //// raise KeyUp
                //if (s_KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                //{
                //    Keys keyData = (Keys)MyKeyboardHookStruct.VirtualKeyCode;
                //    KeyEventArgs e = new KeyEventArgs(keyData);
                //    s_KeyUp.Invoke(null, e);
                //    handled = handled || e.Handled;
                //}

            }

            ////if event handled in application do not handoff to other listeners
            //if (handled)
            //    return -1;

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
