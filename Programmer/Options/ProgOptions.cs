using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer.ProgOptions
{
    class ProgOptions
    {        
        public List<ProgOption> OptionList = new List<ProgOption>();

        public ProgOptions()
        {
            
        }

        private string GetOptionValue(string Name)
        {
            foreach (var O in OptionList)
                if (O.Name.CompareTo(Name) == 0) return O.Value;

            return null;
        }

        public string ProjectsDir
        {
            get
            {
                string Val = GetOptionValue("projects"); 
                if(Val != null)
                    return File.Exists(Val) ? Val : null;
                else
                    return null;
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
                    case "option":
                        {
                            string Name = X.GetAttribute("name");
                            string Value = X.GetAttribute("value");

                            OptionList.Add(new ProgOption(Name, Value));
                        }
                        break;
                }
            }

            X.Close();
        }
    }
}
