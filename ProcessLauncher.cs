using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace AltTabHelperV2
{
    class ProcessLauncher
    {
        /// <summary>
        /// Unfortunatelly Windows does not allow AltTabHelperV2 to work properly unless it's running under a debugger.
        /// So This app does the trick
        /// </summary>

        public static bool AmIBeingDebugged()
        {
            return System.Diagnostics.Debugger.IsAttached;
        }

        public static void LaunchProcessWithAttachedDebuggerInNewThread()
        {
              (new Thread(ProcessLauncher.LaunchProcessWithAttachedDebugger)).Start();
        }

        public static void LaunchProcessWithAttachedDebugger()
        {
            var filename = GetFilename();
            Console.WriteLine("Launching " + filename);

            var stop = new ManualResetEvent(false);
            var engine = new MDbgEngine();
            var process = engine.CreateProcess(filename, "", DebugModeFlag.Default, null);
            process.Go();

            process.PostDebugEvent +=
              (sender, e) =>
              {
                  if (e.CallbackType == ManagedCallbackType.OnBreakpoint)
                      process.Go();

                  if (e.CallbackType == ManagedCallbackType.OnProcessExit)
                      stop.Set();
              };


            //var p = Process.Start(filename);
            Console.WriteLine("Launched...");
            stop.WaitOne();
            Application.Exit();
        }


        static string GetFilename()
        {
            var possibleNames = new string[] { System.AppDomain.CurrentDomain.FriendlyName, Process.GetCurrentProcess().ProcessName, "AltTabHelperV2.exe" };
            foreach (var possibleName in possibleNames)
            {
             
                if (File.Exists(possibleName))
                {
                    return possibleName;
                }

                
            }
            return null;

            //var x = System.AppDomain.CurrentDomain.FriendlyName;
            //var z = Process.GetCurrentProcess().ProcessName;

            //return System.AppDomain.CurrentDomain.FriendlyName;
            //var theFile = "AltTabHelperV2.exe";

            //var probablePaths = new string[] { "", "../../../app/bin/Release/", "../../../app/bin/Debug/" };

            //foreach (var probablePath in probablePaths)
            //{
            //    var probableFile = probablePath.Replace("/", "\\") + theFile;
            //    if (File.Exists(probableFile))
            //    {
            //        return probableFile;
            //    }


            //}
            //Console.Write("File not found: " + theFile);
            //return null;
        }
    }
}
