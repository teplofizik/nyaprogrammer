using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer.Tool
{
    class Tool
    {
        // List of supported controller architectures
        private List<string> ControllerTypes = new List<string>();

        // Название программатора
        private string ToolName = "";

        // Путь к программе
        public string ToolPath = "";

        public List<Input.Field> Fields = new List<Input.Field>();

        private ToolAction taErase = null;
        private ToolAction taProgramOption = null;
        private ToolAction taProgramFlash = null;
        private ToolAction taVerifyFlash = null;
        private ToolAction taProgramData = null;
        private ToolAction taVerifyData = null;
        private ToolAction taLock = null;
        private ToolAction taLaunch = null;
        private ToolAction taConvert = null;

        private List<ToolAction> taCustom = new List<ToolAction>();

        public List<Options.Option> Ops = new List<Options.Option>();
        private List<Options.OptionListItem> SelectedOptions = null;

        public string Warning = "";
        public string Error = "";

        public Tool()
        {

        }

        public void SetActiveOptions(List<Options.OptionListItem> O)
        {
            SelectedOptions = O;
        }

        public void ClearFields()
        {
            Fields.Clear();
        }

        public void SetField(string Name, string Value)
        {
            Fields.Add(new Input.Field(Name, Value));
        }

        private void LoadSupportedTypes(CONF.XmlLoad X)
        {
             while (X.Read())
             {
                 switch (X.ElementName)
                 {
                     case "type": ControllerTypes.Add(X.GetAttribute("value")); break;
                 }
             }

             X.Close();
        }

        private ToolAction LoadAction(CONF.XmlLoad X)
        {
            ToolAction A = new ToolAction();
            
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "command": A.Command = X.GetAttribute("value"); break;
                    case "toolpath": A.CustomToolPath = X.GetAttribute("value"); break;
                    case "params":
                        {
                            CONF.XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                var Name = Subtree.ElementName;
                                var Def = Subtree.GetAttribute("default") ?? "";

                                A.Defaults.setString(Name, Def);
                            }
                            Subtree.Close();
                        }
                        break;
                    case "error":
                        {
                            CONF.XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "string": A.ErrorMask.Add(Subtree.GetAttribute("value")); break;
                                }
                            }
                            Subtree.Close();
                        }
                        break;
                    case "internal":
                        {
                            CONF.XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "string": A.Write.Add(Subtree.GetAttribute("value")); break;
                                }
                            }
                            Subtree.Close();
                        }
                        break;
                }
            }

            return A;
        }

        private void LoadCustomActions(CONF.XmlLoad X)
        {
            while (X.Read())
            {
                ToolAction A = LoadAction(X.GetSubtree());
                A.Name = X.ElementName;

                taCustom.Add(A);
            }

            X.Close();
        }

        private void LoadActions(CONF.XmlLoad X)
        {
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "erase": taErase = LoadAction(X.GetSubtree()); break;
                    case "woption": taProgramOption = LoadAction(X.GetSubtree()); break;
                    case "wflash": taProgramFlash = LoadAction(X.GetSubtree()); break;
                    case "wdata": taProgramData = LoadAction(X.GetSubtree()); break;
                    case "vflash": taVerifyFlash = LoadAction(X.GetSubtree()); break;
                    case "vdata": taVerifyData = LoadAction(X.GetSubtree()); break;
                    case "lock": taLock = LoadAction(X.GetSubtree()); break;
                    case "launch": taLaunch = LoadAction(X.GetSubtree()); break;
                    case "convert": taConvert = LoadAction(X.GetSubtree()); break;
                    default:
                        {
                            ToolAction A = LoadAction(X.GetSubtree());
                            A.Name = X.ElementName;

                            taCustom.Add(A);
                        }
                        break;
                }
            }

            X.Close();
        }

        private void LoadOption(Options.OptionList O, CONF.XmlLoad Base, CONF.XmlLoad X)
        {
            var I = new Options.OptionListItem(Base.GetAttribute("label"));
            I.Default = (Base.GetAttribute("default") != null);

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "value":
                        string N = X.GetAttribute("name");
                        string V = X.GetAttribute("val");

                        if(N != null) I.setString(N, V);
                        break;
                }
            }

            X.Close();
            O.AddItem(I);
        }

        private Options.OptionList LoadList(string Name, CONF.XmlLoad X)
        {
            var O = new Options.OptionList(Name);
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "option": LoadOption(O, X, X.GetSubtree()); break;
                }
            }

            X.Close();

            return O;
        }


        private Options.OptionText LoadText(string Name, CONF.XmlLoad X)
        {
            var O = new Options.OptionText(Name);

            var Def = X.GetAttribute("default");
            if (Def != null) O.Default = Def;

            var N = X.GetAttribute("name");
            if (N != null) O.ParamName = N;

            return O;
        }

        private void LoadOptions(CONF.XmlLoad X)
        {
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "list": Ops.Add(LoadList(X.GetAttribute("label"), X.GetSubtree())); break;
                    case "text": Ops.Add(LoadText(X.GetAttribute("label"), X)); break;
                }
            }

            X.Close();
        }

        public bool Load(string FileName, bool Custom)
        {
            CONF.XmlLoad X = new CONF.XmlLoad();

            if (!X.Load(FileName)) return false;

            while (X.Read())
            {
                if (Custom)
                {
                    switch (X.ElementName)
                    {
                        case "name": ToolName = X.GetAttribute("value"); break;
                        case "path": ToolPath = X.GetAttribute("value"); break;
                        case "actions": LoadCustomActions(X.GetSubtree()); break;
                    }
                }
                else
                {
                    switch (X.ElementName)
                    {
                        case "name": ToolName = X.GetAttribute("value"); break;
                        case "path": ToolPath = X.GetAttribute("value"); break;
                        case "supported": LoadSupportedTypes(X.GetSubtree()); break;
                        case "actions": LoadActions(X.GetSubtree()); break;
                        case "options": LoadOptions(X.GetSubtree()); break;
                    }
                }
            }

            X.Close();

            return true;
        }

        // Is architecture supported by this tool?
        public bool IsArchitectureSupported(List<string> Architecture)
        {
            foreach(var A in Architecture)
                if(ControllerTypes.Contains(A)) 
                    return true;

            return false;
        }

        private bool ActionNotSupported()
        {
            Warning = "Action not supported by tool.";

            return true;
        }

        private bool IsHexOption(string Option)
        {
            for (int i = 0; i < Option.Length; i++)
            {
                char C = Option[i];

                if (
                    ((C >= '0') && (C <= '9')) ||
                    ((C >= 'a') && (C <= 'f')) ||
                    ((C >= 'A') && (C <= 'F'))
                    ) continue;

                return false;
            }

            return true;
        }

        private string ParseExternalToolVars(string Command)
        {
            string Soft = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location)) + "\\Soft";
            Command = Command.Replace("{soft}", Soft);

            foreach(var F in Fields)
                Command = Command.Replace(F.Name, F.Value);

            return Command;
        }

        private string ReplaceToolOption(string Text)
        {
            var Temp = Text;
            if (SelectedOptions != null)
            {
                foreach(var O in SelectedOptions)
                {
                    foreach(var S in O.getNames())
                    {
                        var Arg ="{" + S + "}";

                        if(Temp.IndexOf(Arg) >= 0)
                            Temp = Temp.Replace(Arg, O.getString(S));
                    }
                }
            }
            return Temp;
        }

        private string ReplaceActionOption(string Text, ToolAction A, Options.OptionListItem Ops)
        {
            if (Ops == null) return Text;

            var Temp = Text;

            foreach(var P in A.Defaults.Params)
            {
                var N = P.Name;
                var V = Ops.getString(N);
                if (V == null) V = P.ToString();

                string Mask = "{" + N + "}";
                Temp = Temp.Replace(Mask, V);
            }

            return Temp;
        }

        private string ReplaceOptionByte(string Text, string Option)
        {
            if (Option == null) Option = "";
            string Temp = Text.Replace("{option}", Option);

            if(IsHexOption(Option))
            {
                for (int i = 0; i < Option.Length / 2; i++)
                {
                    string Mask = "{" + String.Format("option{0:d}", i) + "}";
                    string Hex = Option.Substring(i * 2, 2);

                    Temp = Temp.Replace(Mask, Hex);
                }
            }

            return Temp;
        }

        private string GetMessage(string Text, List<string> Mask)
        {
            foreach (string M in Mask)
            {
                int Index = Text.IndexOf(M);
                if (Index >= 0)
                {
                    var T = Text.Split(new char[] { '\n', '\r' });

                    foreach (var Line in T)
                    {
                        if(Line.IndexOf(M) >= 0)
                            return Line;
                    }

                    return Text;
                }
            }
            return null;
        }

        private bool Exec(ToolAction A, string Command, List<string> Write, string Dir)
        {
            string P = (A.CustomToolPath != null) ? A.CustomToolPath : ToolPath;
            string Output = ExternalTool.Run(P, Command, Write, Dir);

            if (Output == null) return false;
            
            var E = GetMessage(Output, A.ErrorMask);
            if (E != null)
            {
                Error = E;
                return false;
            }

            var W = GetMessage(Output, A.WarningMask);
            if (W != null) Warning = W;

            return true;
        }

        private bool GenericToolAction(ToolAction A, string Device, string Dir, string Options, Options.OptionListItem Ops)
        {
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = Command.Replace("{filename}", Options);
            Command = Command.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(Options));
            Command = Command.Replace("{dir}", Dir);
            Command = ReplaceActionOption(Command, A, Ops);
            Command = ReplaceOptionByte(Command, Options);
            Command = ReplaceToolOption(Command);
            Command = ParseExternalToolVars(Command);

            
            List<string> Write = new List<string>();
            /*for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                T = T.Replace("{filename}", Options);
                T = T.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(Options));
                T = T.Replace("{dir}", Dir);
                T = ReplaceOptionByte(T, Options);
                Write.Add(T);
            }*/

            return Exec(A, Command, Write, Dir);
        }

        public bool GenericToolAction(string Name, string Device, string Dir, string Options, Options.OptionListItem Ops)
        {
            ToolAction Act = null;

            foreach (var A in taCustom)
                if (A.Name.CompareTo(Name) == 0)
                {
                    Act = A;
                    break;
                }

            return GenericToolAction(Act, Device, Dir, Options, Ops);
        }

        // Program lock bytes
        public bool Lock(string Device, string Dir, string Options)
        {
            return GenericToolAction(taLock, Device, Dir, Options, null);
        }

        // Launch device (from bootloader)
        public bool Launch(string Device, string Dir, string Options)
        {
            return GenericToolAction(taLaunch, Device, Dir, Options, null);
        }

        // Convert firmware
        public bool Convert(string Device, string Dir, string Options)
        {
            return GenericToolAction(taConvert, Device, Dir, Options, null);
        }

        // Program option/fuse bytes
        public bool Option(string Device, string Dir, string Options)
        {
            return GenericToolAction(taProgramOption, Device, Dir, Options, null);
        }

        // Program flash
        public bool ProgramFlash(string Device, string Dir, string FileName)
        {
            ToolAction A = taProgramFlash;
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = Command.Replace("{filename}", FileName);
            Command = Command.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
            Command = Command.Replace("{dir}", Dir);
            Command = ParseExternalToolVars(Command);
            Command = ReplaceToolOption(Command);

            List<string> Write = new List<string>();
            for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                T = T.Replace("{filename}", FileName);
                T = T.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
                T = T.Replace("{dir}", Dir);
                Write.Add(T);
            }

            return Exec(A, Command, Write, Dir);
        }

        // Program data (eeprom)
        public bool ProgramData(string Device, string Dir, string FileName)
        {
            ToolAction A = taProgramData;
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = Command.Replace("{filename}", FileName);
            Command = Command.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
            Command = Command.Replace("{dir}", Dir);
            Command = ParseExternalToolVars(Command);
            Command = ReplaceToolOption(Command);

            List<string> Write = new List<string>();
            for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                T = T.Replace("{filename}", FileName);
                T = T.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
                T = T.Replace("{dir}", Dir);
                Write.Add(T);
            }

            return Exec(A, Command, Write, Dir);
        }

        // Verify flash
        public bool VerifyFlash(string Device, string Dir, string FileName)
        {
            ToolAction A = taVerifyFlash;
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = Command.Replace("{filename}", FileName);
            Command = Command.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
            Command = Command.Replace("{dir}", Dir);
            Command = ParseExternalToolVars(Command);
            Command = ReplaceToolOption(Command);

            List<string> Write = new List<string>();
            for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                T = T.Replace("{filename}", FileName);
                T = T.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
                T = T.Replace("{dir}", Dir);
                Write.Add(T);
            }

            return Exec(A, Command, Write, Dir);
        }

        // Verify data (eeprom)
        public bool VerifyData(string Device, string Dir, string FileName)
        {
            ToolAction A = taVerifyData;
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = Command.Replace("{filename}", FileName);
            Command = Command.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
            Command = Command.Replace("{dir}", Dir);
            Command = ParseExternalToolVars(Command);
            Command = ReplaceToolOption(Command);

            List<string> Write = new List<string>();
            for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                T = T.Replace("{filename}", FileName);
                T = T.Replace("{filenamewe}", Path.GetFileNameWithoutExtension(FileName));
                T = T.Replace("{dir}", Dir);
                Write.Add(T);
            }

            return Exec(A, Command, Write, Dir);
        }

        // Erase controller
        public bool Erase(string Device, string Dir)
        {
            ToolAction A = taErase;
            if (A == null) return ActionNotSupported();
            Error = "";
            Warning = "";

            string Command = A.Command;
            Command = Command.Replace("{device}", Device);
            Command = ParseExternalToolVars(Command);
            Command = ReplaceToolOption(Command);

            List<string> Write = new List<string>();
            for (int i = 0; i < A.Write.Count; i++)
            {
                string T = A.Write[i];
                T = T.Replace("{device}", Device);
                Write.Add(T);
            }

            return Exec(A, Command, Write, Dir);
        }

        public bool HasToolAction(string Name)
        {
            foreach (var A in taCustom)
                if (A.Name.CompareTo(Name) == 0) return true;

            return false;
        }

        public override string ToString()
        {
            return ToolName;
        }
    }
}
