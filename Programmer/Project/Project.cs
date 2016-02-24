using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Programmer.Project
{
    class ScriptAction
    {
        public string Name = "";

        public string Value = "";
        public string Comment = "";

        public Tool.Options.OptionListItem Params;

        public ScriptAction(string N, string V, string C, Tool.Options.OptionListItem P)
        {
            Name = N;
            Value = V;
            Comment = C;

            Params = P;
        }
    }

    class Script
    {
        // Script name
        public string Name = "";

        // Script device
        public string Device = "";

        // Script cathegory
        public int Cathegory = 0;

        // Id default script
        public bool DefaultScript = false;

        /// <summary>
        /// Старая версия?
        /// </summary>
        public bool Old = false;

        // Need input?
        public string Input = null;
        public string Default = null;

        public bool Autorun = false;

        public string Version = "Default";

        // Actions
        public List<ScriptAction> Actions = new List<ScriptAction>();

        public bool CheckScriptVersion(string Version)
        {
            return Version.CompareTo(this.Version) == 0;
        }
    }

    class Project
    {
        // Controller type
        public List<string> Type = new List<string>();

        // Project name
        public string Name = "";

        // Project description
        public string Description = null;

        // Project dir
        public string Dir = "";

        // Default version
        public string DefaultVersion = "Default";

        // Scripts
        public List<Script> Scripts = new List<Script>();

        /// <summary>
        /// Список версий
        /// </summary>
        public string[] Versions
        {
            get
            {
                var Res = new List<string>();

                foreach(var S in Scripts)
                {
                    if (S.Version.CompareTo("any") == 0) continue;
                    if (!Res.Contains(S.Version))
                        Res.Add(S.Version);
                }
                
                return Res.ToArray();
            }
        }

        public Project(string Dir)
        {
            this.Dir = Dir;
        }

        private void LoadActions(Script S, CONF.XmlLoad X)
        {
            while (X.Read())
            {
                string Value = X.GetAttribute("value");
                string Comment = X.GetAttribute("comment");
                Tool.Options.OptionListItem Params = new Tool.Options.OptionListItem("values");

                string[] Args = X.GetAttributeNames();
                for (int i = 0; i < Args.Length; i++)
                {
                    var N = Args[i];
                    if ((N.CompareTo("value") == 0) || (N.CompareTo("comment") == 0)) continue;

                    Params.setString(N, X.GetAttribute(N));
                }

                switch (X.ElementName)
                {
                    case "erase": S.Actions.Add(new ScriptAction("erase", Value, Comment, Params)); break;
                    case "option": S.Actions.Add(new ScriptAction("option", Value, Comment, Params)); break;
                    case "wflash": S.Actions.Add(new ScriptAction("wflash", Value, Comment, Params)); break;
                    case "wdata": S.Actions.Add(new ScriptAction("wdata", Value, Comment, Params)); break;
                    case "vflash": S.Actions.Add(new ScriptAction("vflash", Value, Comment, Params)); break;
                    case "vdata": S.Actions.Add(new ScriptAction("vdata", Value, Comment, Params)); break;
                    case "external": S.Actions.Add(new ScriptAction("external", Value, Comment, Params)); break;
                    case "lock": S.Actions.Add(new ScriptAction("lock", Value, Comment, Params)); break;
                    case "launch": S.Actions.Add(new ScriptAction("launch", Value, Comment, Params)); break;
                    case "copy": S.Actions.Add(new ScriptAction("copy", Value, Comment, Params)); break;
                    case "copyto": S.Actions.Add(new ScriptAction("copyto", Value, Comment, Params)); break;
                    case "convert": S.Actions.Add(new ScriptAction("convert", Value, Comment, Params)); break;

                    default: S.Actions.Add(new ScriptAction(X.ElementName, Value, Comment, Params)); break;
                }
            }

            X.Close();
        }

        private void LoadScript(CONF.XmlLoad X)
        {
            Script S = new Script();
            Scripts.Add(S);

            if (X.HasAttribute("version"))
                S.Version = X.GetAttribute("version");

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "name": S.Name = X.GetAttribute("value"); break;
                    case "device": S.Device = X.GetAttribute("value"); break;
                    case "cathegory": S.Cathegory = X.GetIntAttribute("value"); break;
                    case "input":
                        S.Input = X.GetAttribute("value");
                        S.Default = X.GetAttribute("default");
                        break;
                    case "steps": LoadActions(S, X.GetSubtree()); break;
                    case "default": S.DefaultScript = true; break;
                    case "autorun": S.Autorun = true; break;
                    case "old": S.Old = true; break;
                }
            }

            X.Close();
        }

        public bool Load(string FileName)
        {
            CONF.XmlLoad X = new CONF.XmlLoad();

            if (!X.Load(FileName)) return false;

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "name": Name = X.GetAttribute("value"); break;
                    case "type": Type.Add(X.GetAttribute("value")); break;
                    case "description": Description = X.GetAttribute("value"); break;
                    case "version": DefaultVersion = X.GetAttribute("value"); break;
                    case "script": LoadScript(X.GetSubtree()); break;
                }
            }

            X.Close();

            Dir = Path.GetDirectoryName(FileName) + "\\";

            return true;
        }
    }
}
