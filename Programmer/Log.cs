using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Programmer
{
    static class Log
    {
        const string LogFileName = "log.txt";

        public static void WriteLine(string Text)
        {
            using (StreamWriter sw = File.AppendText(LogFileName))
            {
                sw.WriteLine(Text);
            }
        }

        public static void WriteLines(string[] Lines)
        {
            if (Lines == null) return;

            using (StreamWriter sw = File.AppendText(LogFileName))
            {
                foreach(string Line in Lines)
                    sw.WriteLine(Line);
            }
        }
    }
}
