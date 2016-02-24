using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Programmer
{
    static class Program
    {
        static string OptionsFile = null;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ParseArgs(args);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fMain(OptionsFile));
        }

        static void ParseArgs(string[] args)
        {
            ArgumentParser Parser = new ArgumentParser(args);
            List<Argument> ArgList = Parser.Arguments;

            for (int i = 0; i < ArgList.Count; i++)
            {
                switch (ArgList[i].Name)
                {
                    case "o":
                        {
                            foreach (var A in ArgList[i].Arguments)
                            {
                                if (A.Name.CompareTo("file") == 0)
                                {
                                    if(File.Exists(A.Value))
                                        OptionsFile = A.Value;
                                }
                            }
                            break;
                        }
                }
            }
        }
    }
}
