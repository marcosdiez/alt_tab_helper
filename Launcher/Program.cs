using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = getFilename();

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
            Console.WriteLine("Done...");
            stop.WaitOne();
        }

        static string getFilename()
        {
            var theFile = "AltTabHelperV2.exe";

            var probablePaths = new string[] { "", "../../../app/bin/Debug/", "../../../app/bin/Release/" };

            foreach(var probablePath in probablePaths)
            {
                var probableFile = probablePath.Replace("/", "\\") + theFile;
                if (File.Exists(probableFile))
                {
                    return probableFile;
                }


            }
            Console.Write("File not found: " + theFile);
            return null;
        }
    }
}
