using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace Programmer
{
    class ExternalTool
    {
        static public string Run(string File, string Args, List<string> Stdin, string Dir)
        {
            Console.WriteLine(File + " " + Args);
            Log.WriteLine(File + " " + Args);

            //string ProgramPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string D = Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = File;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Args;
            startInfo.WorkingDirectory = Dir;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    StreamWriter Wr = exeProcess.StandardInput;
                    for (int i = 0; i < Stdin.Count; i++) Wr.WriteLine(Stdin[i]);

                    string output = exeProcess.StandardOutput.ReadToEnd();
                    string error = exeProcess.StandardError.ReadToEnd();
                    exeProcess.WaitForExit();

                    string textout = "";
                    if (output != null)
                    {
                        textout += output;
                        Log.WriteLine(output);
                    }

                    if (error != null)
                    {
                        textout += error;
                        Log.WriteLine(error);
                    }

                    return textout;
                }
            }
            catch(Exception E)
            {
                Log.WriteLine(E.Message);
            }

            return null;
        }
    }
}
