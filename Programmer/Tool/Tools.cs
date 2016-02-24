using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Programmer.Tool
{
    class Tools
    {
        public List<Tool> ToolList = new List<Tool>();
        public List<Tool> CustomToolList = new List<Tool>();

        public Tools()
        {
            
        }

        private void LoadTool(string FileName, bool Custom)
        {
            Tool T = new Tool();
            if (T.Load(FileName, Custom))
            {
                if(Custom)
                    CustomToolList.Add(T);
                else
                    ToolList.Add(T);
            }
            else
            {
                Log.WriteLine(String.Format("Could not load tool file: {0:s}", FileName));
            }
        }

        public void Load(string Path, string FileName)
        {
            CONF.XmlLoad X = new CONF.XmlLoad();

            if (!X.Load(Path + FileName)) return;
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "tool":
                        {
                            string FN = Path + X.GetAttribute("file");
                            int Disabled = X.GetIntAttribute("disabled");
                            bool Custom = X.GetIntAttribute("custom") != 0;

                            if (Disabled == 0) LoadTool(FN, Custom);
                        }
                        break;
                }
            }

            X.Close();
        }
    }
}
