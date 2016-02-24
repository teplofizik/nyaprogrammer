using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Programmer.Tool;

namespace Programmer.Project
{
    class ScriptEventArgs : EventArgs
    {
        public bool Completed = false; // Успешно?
        public int Index = 0;
        public string Error = "";
        public string Warning = "";
    }

    delegate void ScriptEventHandler(object sender, ScriptEventArgs e);

    class ScriptExec
    {
        private Project P = null;
        public event ScriptEventHandler onActionCompleted;

        private bool Busy = false;
        private int ActionIndex = 0;
        private Script ActiveScript = null;
        private Tool.Tool ActiveTool = null;
        private Tool.Tools Toolset = null;
        private List<Tool.Options.OptionListItem> ActiveOptions = null;
        private string UserValue = "";

        public ScriptExec(Project Pr)
        {
            P = Pr;
        }

        private void FireonActionCompleted(int Index, bool Completed, string Error, string Warning)
        {
            if(onActionCompleted == null) return;

            ScriptEventArgs E = new ScriptEventArgs();
            E.Completed = Completed;
            E.Index = Index;
            E.Error = Error;
            E.Warning = Warning;

            onActionCompleted(this, E);
        }

        private bool IsUserValueCorrect(string Input, string Command)
        {
            switch (Input)
            {
                case "uint64":
                    try
                    {
                        if (UserValue.Length > 12) return false;

                        UInt64 V = UInt64.Parse(UserValue, System.Globalization.NumberStyles.HexNumber);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
            }

            return true;
        }

        private string GetUserValue(string Input)
        {
            switch (Input)
            {
                case "uint64": return UInt64.Parse(UserValue, System.Globalization.NumberStyles.HexNumber).ToString();
                case "int64": return Int64.Parse(UserValue, System.Globalization.NumberStyles.HexNumber).ToString();
                case "int": return Int32.Parse(UserValue).ToString();
                default: return UserValue ?? "";
            }
        }

        private string ParseUserValue(string Input, string Command)
        {
            Command = Command.Replace("{input}", GetUserValue(Input));

            return Command;
        }

        private string ParseExternalToolVars(string Command)
        {
            string Soft = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location)) + "\\Soft";
            Command = Command.Replace("{soft}", Soft);

            return Command;
        }

        private bool RunExternalTool(string Input, string Command)
        {
            if (!IsUserValueCorrect(Input, Command))
                return false;

            Command = ParseUserValue(Input, Command);
            Command = ParseExternalToolVars(Command);

            string Exe = "";
            string Args = "";

            {
                int Separator = Command.IndexOf(".exe");
                if (Separator == -1) return false;

                Exe = Command.Substring(0, Separator + 4);
                Args = Command.Substring(Separator + 5);
            }

            if (!File.Exists(Exe)) Exe = P.Dir + Exe;
            string Output = ExternalTool.Run(Exe, Args, new List<string>(), P.Dir);
            if(Output != null)
            {
                if (Output.Contains("error")) return false;
                if (Output.Contains("ERROR")) return false;
                if (Output.Contains("wrong")) return false;
            }
            return true;
        }

        private bool CompareFile(string A, string B)
        {
            if (!File.Exists(A) || !File.Exists(B)) return false;

            // Проверим-ка их даты изготовления
            var AI = new FileInfo(A);
            var BI = new FileInfo(B);

            if (AI.Length != BI.Length) return false;
            if (AI.LastWriteTimeUtc.CompareTo(BI.LastWriteTimeUtc) != 0) return false;

            return true;
        }

        public void ActionThread()
        {
            Tool.Tool T = ActiveTool;
            Script S = ActiveScript;
            ScriptAction A = ActiveScript.Actions[ActionIndex];
            var Ops = A.Params;

            T.SetActiveOptions(ActiveOptions);
            T.ClearFields();
            if (S.Input != null) T.SetField("{input}", GetUserValue(S.Input));

            Debug.WriteLine(String.Format("{0:d}. {1:s}", ActionIndex, A.Comment));

            string Warning = "";
            string Error = "";
            bool Result = false;
            switch (A.Name)
            {
                case "erase":
                    Result = T.Erase(S.Device, P.Dir);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "option":
                    Result = T.Option(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "wflash":
                    Result = T.ProgramFlash(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "wdata":
                    Result = T.ProgramData(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "vflash":
                    Result = T.VerifyFlash(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "vdata":
                    Result = T.VerifyData(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "lock":
                    Result = T.Lock(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "launch":
                    Result = T.Launch(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "convert":
                    Result = T.Convert(S.Device, P.Dir, A.Value);
                    Error = T.Error;
                    Warning = T.Warning;
                    break;
                case "external":
                    Result = RunExternalTool(S.Input, A.Value);
                    Error = (Result) ? "" : "Wrong command";
                    break;
                case "copy":
                    if (File.Exists(A.Value))
                    {
                        string NewFn = P.Dir + Path.GetFileName(A.Value);

                        if (CompareFile(NewFn, A.Value))
                        {
                            Warning = "Files are equal";
                            Result = true;
                        }
                        else
                        {
                            File.Delete(NewFn);
                            File.Copy(A.Value, NewFn);
                            Result = true;
                        }
                    }
                    else
                        Error = "No source file";
                    break;
                case "copyto":
                    if (File.Exists(A.Value))
                    {
                        string NewFn = P.Dir + Path.GetFileName(A.Value);

                        if (CompareFile(NewFn, A.Value))
                        {
                            Warning = "Files are equal";
                            Result = true;
                        }
                        else
                        {
                            File.Delete(A.Value);
                            File.Copy(NewFn, A.Value);
                            Result = true;
                        }
                    }
                    else
                        Error = "No source file";
                    break;
                default:
                    // Check tool actions
                    if(T.HasToolAction(A.Name))
                    {
                        Result = T.GenericToolAction(A.Name, S.Device, P.Dir, A.Value, Ops); 
                        Error = T.Error;
                        Warning = T.Warning;
                        break;
                    }
                    // Check custom tool actions
                    else if(Toolset != null)
                    {
                        bool Found = false;
                        foreach(var CT in Toolset.CustomToolList)
                        {
                            if (CT.HasToolAction(A.Name))
                            {
                                CT.ClearFields();
                                if (S.Input != null) CT.SetField("{input}", GetUserValue(S.Input));
                                Result = CT.GenericToolAction(A.Name, S.Device, P.Dir, A.Value, Ops);
                                Error = CT.Error;
                                Warning = CT.Warning;
                                Found = true;
                                break;
                            }
                        }

                        if(!Found)
                        {
                            Warning = "Action not supported";
                            Result = true;
                        }
                    }
                    break;
            }

            Debug.WriteLine(String.Format("   {0:s}: {1:s}", (Result) ? "OK" : "FAIL", T.Error));
            Busy = false;
            FireonActionCompleted(ActionIndex, Result, Error, Warning);
        }

        public void RunAction(Script S, int Index, Tool.Tool T, Tool.Tools Tools, List<Tool.Options.OptionListItem> Options)
        {
            if ((S == null) || Busy || (T == null))
            {
                FireonActionCompleted(Index, false, "No script or busy, or no tool", "");
                return;
            }
            if (S.Actions.Count <= Index)
            {
                FireonActionCompleted(Index, false, "Invalid index", "");
                return;
            }

            Thread Thr = new Thread(ActionThread);

            ActiveOptions = Options;
            ActiveTool = T;
            ActionIndex = Index;
            ActiveScript = S;
            Toolset = Tools;
            Busy = true;

            Thr.Start();
        }

        public void SetValue(string Value)
        {
            UserValue = Value;
        }
    }
}
