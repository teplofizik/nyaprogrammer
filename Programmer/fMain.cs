using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Net;

namespace Programmer
{
    public partial class fMain : Form
    {
        string ProgramPath = "";

        // Project
        private Input.InputParser mInput = new Input.InputParser();
        private Project.Project CurrentProject = null;
        private Project.Projects Pr = null;
        private Project.Script CurrentScript = null;
        private Project.ScriptExec ScriptExec = null;
        private bool Runned = false;

        // Tool
        private Tool.Tools Toolset = null;

        // Options
        private ProgOptions.ProgOptions Opts;

        // UI
        private CheckBox[] Checks;
        private List<Button[]> Scripts = new List<Button[]>();
        private ComboBox[] ToolOptionsCB;
        private TextBox[] ToolOptionsTB;

        // Action
        private bool Cancel = false;

        public fMain(string Options)
        {
            InitializeComponent();

            Init(Options);
        }

        private void Init(string Options)
        {
            ToolOptionsCB = new ComboBox[] { cbOption1, cbOption2, cbOption3, cbOption4 };
            ToolOptionsTB = new TextBox[] { tOption1, tOption2, tOption3, tOption4 };

            Checks = new CheckBox[] { ck1, ck2, ck3, ck4, ck5, ck6, ck7, ck8, ck9, ck10 };

            Scripts.Add(new Button[] { b0_0, b0_1, b0_2, b0_3, b0_4 });
            Scripts.Add(new Button[] { b1_0, b1_1, b1_2, b1_3, b1_4 });
            Scripts.Add(new Button[] { b2_0, b2_1, b2_2, b2_3, b2_4 });
            Scripts.Add(new Button[] { b3_0, b3_1, b3_2, b3_3, b3_4 });
            Scripts.Add(new Button[] { b4_0, b4_1, b4_2, b4_3, b4_4 });
            Scripts.Add(new Button[] { b5_0, b5_1, b5_2, b5_3, b5_4 });

            Opts = new ProgOptions.ProgOptions();
            Opts.Load("", Options ?? "options.xml");

            ProgramPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string ProjectDir = Opts.ProjectsDir ?? ProgramPath + "\\Projects\\projects.xml";

            Pr = new Project.Projects(Path.GetDirectoryName(Path.GetFullPath(ProjectDir)));
            Pr.Load(ProjectDir);

            Toolset = new Tool.Tools();
            Toolset.Load(ProgramPath + "\\Tools\\", "tools.xml");

            Text = "Nya Programmer Tool - " + ProjectDir;
        }

        private void CreateProjectList()
        {
            cbProjects.Items.Clear();
            for (int i = 0; i < Pr.ProjectList.Count; i++)
            {
                Project.Project P = Pr.ProjectList[i];
                // string Text = String.Format("[{0:s}] {1:s}", P.Type, P.Name);
                // if(P.Description != null) Text += ". " + P.Description;
                string Text = P.Name;
                string Type = (P.Type.Count > 0) ? P.Type[0] : "---";

                if (P.Description != null) Text += ". " + P.Description;
                Text += " [" + Type + "]";

                cbProjects.Items.Add(Text);
            }

            if (cbProjects.Items.Count > 0) cbProjects.SelectedIndex = 0;
        }

        private void CreateToolList(Project.Project P)
        {
            cbTool.Items.Clear();
            for (int i = 0; i < Toolset.ToolList.Count; i++)
            {
                Tool.Tool T = Toolset.ToolList[i];
                if (!T.IsArchitectureSupported(P.Type)) continue;

                cbTool.Items.Add(T);
            }

            if (cbTool.Items.Count > 0) cbTool.SelectedIndex = 0;
        }

        private void AddScriptButton(int Cathegory, Project.Script S)
        {
            Button[] BList = Scripts[Cathegory];

            for (int i = 0; i < BList.Length; i++)
            {
                Button B = BList[i];
                if (B.Tag != null) continue;

                B.Tag = S;
                B.Text = S.Name;
                B.Visible = true;

                break;
            }
        }

        private void RefreshButtonColor()
        {
            for (int i = 0; i < Scripts.Count; i++)
            {
                for (int j = 0; j < Scripts[i].Length; j++)
                {
                    Button B = Scripts[i][j];

                    B.ForeColor = (B.Tag == CurrentScript) ? Color.Green : SystemColors.ControlText;
                }
            }
        }

        private void RefreshButtonAvail(bool InProcess)
        {
            for (int i = 0; i < Scripts.Count; i++)
            {
                for (int j = 0; j < Scripts[i].Length; j++)
                {
                    Button B = Scripts[i][j];

                    if (B.Tag != null)
                    {
                        var S = B.Tag as Project.Script;

                        B.Enabled = (S.Autorun) ? !InProcess : true;
                    }
                }
            }
        }

        private void RefreshVersions(Project.Project P)
        {
            var V = P.Versions;

            cbVersion.Items.Clear();
            cbVersion.Items.Add("All");
            cbVersion.Items.AddRange(V);
            cbVersion.SelectedIndex = 0;
            if ((V.Contains(P.DefaultVersion) && (P.DefaultVersion.CompareTo("Default") != 0)))
            {
                for (int i = 0; i < V.Length; i++)
                {
                    if (V[i].CompareTo(P.DefaultVersion) == 0)
                    {
                        cbVersion.SelectedIndex = i + 1;
                        break;
                    }
                }
            }
            cbVersion.Visible = (V.Length > 1);
        }

        private void RefreshButtons(Project.Project P, bool SelectDefault)
        {
            for (int i = 0; i < Scripts.Count; i++)
            {
                for(int j = 0; j < Scripts[i].Length; j++)
                {
                    Button B = Scripts[i][j];

                    B.Visible = false;
                    B.Text = "";
                    B.Tag = null;
                }
            }

            if (P == null) return;

            for (int i = 0; i < P.Scripts.Count; i++)
            {
                Project.Script S = P.Scripts[i];
                int Cat = S.Cathegory;
                if (Cat > 5) continue;

                if (cbVersion.SelectedIndex > 0)
                {
                    // Фильтр по версии
                    if ((S.Version.CompareTo("any") == 0) ||
                        (S.Version.CompareTo(cbVersion.SelectedItem as string) == 0))
                        AddScriptButton(Cat, S);
                }
                else
                {
                    if(!S.Old)
                        AddScriptButton(Cat, S);
                }

                if (SelectDefault)
                {
                    if (S.DefaultScript)
                    {
                        CurrentScript = S;
                        RefreshSteps(S);
                    }
                }
            }
        }

        private void RefreshSteps(Project.Script S)
        {
            lScript.Text = "Скрипт не выбран";
            bStart.Enabled = false;
            tSerial.Visible = false;
            bPlus.Visible = false;

            for (int i = 0; i < Checks.Length; i++)
            {
                Checks[i].Visible = false;
                Checks[i].Tag = false;
                Checks[i].Checked = false;

                Checks[i].ForeColor = Color.Black;
                Checks[i].Text = "";
            }

            if (S == null) return;

            tSerial.Visible = (S.Input != null);
            bPlus.Visible = tSerial.Visible;
            if (S.Default != null)
            {
                if (S.Default.CompareTo(tSerial.Tag) != 0)
                {
                    tSerial.Tag = S.Default;
                    tSerial.Text = S.Default;
                }
            }

            bStart.Enabled = true;
            lScript.Text = S.Name;
            for (int i = 0; i < S.Actions.Count; i++)
            {
                Checks[i].Visible = true;
                Checks[i].Text = S.Actions[i].Comment;
            }

            Array.ForEach(ToolOptionsCB, CB => CB.Enabled = CB.Visible );
        }

        private void UpdateStatus(bool ActiveProcess)
        {
            Runned = ActiveProcess;

            bStart.Text = (ActiveProcess) ? "Отмена" : "Старт";
            cbTool.Enabled = !ActiveProcess;

            RefreshButtonAvail(ActiveProcess);

            if(!ActiveProcess) Array.ForEach(ToolOptionsCB, CB => CB.Enabled = CB.Visible);
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            CreateProjectList();
            RefreshSteps(null);
            RefreshButtons(null, false);
            RefreshButtonColor();
        }

        private void bOpenProject_Click(object sender, EventArgs e)
        {
            CurrentProject = Pr.ProjectList[cbProjects.SelectedIndex];

            RefreshVersions(CurrentProject);
            RefreshSteps(null);
            RefreshButtons(CurrentProject, true);
            CreateToolList(CurrentProject);
            RefreshButtonColor();
            
            ScriptExec = new Project.ScriptExec(CurrentProject);
            ScriptExec.onActionCompleted += new Project.ScriptEventHandler(ScriptExec_onActionCompleted);
        }

        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSteps(null);
            RefreshButtons(CurrentProject, true);
            CreateToolList(CurrentProject);
            RefreshButtonColor();
        }

        private void bScript_Click(object sender, EventArgs e)
        {
            Button B = sender as Button;
            if (B.Tag == null) return;

            CurrentScript = B.Tag as Project.Script;
            RefreshSteps(CurrentScript);
            RefreshButtonColor();

            if (CurrentScript.Autorun) bStart.PerformClick();
        }

        private List<Tool.Options.OptionListItem> getOptions()
        {
            var R = new List<Tool.Options.OptionListItem>();
            Array.ForEach(ToolOptionsCB, CB => { if (CB.Visible && CB.SelectedItem != null) R.Add(CB.SelectedItem as Tool.Options.OptionListItem); });

            foreach(var TB in ToolOptionsTB)
            {
                if (TB.Visible)
                {
                    var O = TB.Tag as Tool.Options.OptionText;
                    var I = new Tool.Options.OptionListItem(O.Name);
                    I.setString(O.ParamName, TB.Text);

                    R.Add(I);
                }
            }
            return R;
        }

        private void RunAction(int Index)
        {
            Tool.Tool T = (cbTool.SelectedIndex >= 0) ? cbTool.Items[cbTool.SelectedIndex] as Tool.Tool : new Tool.Tool();
            ScriptExec.RunAction(CurrentScript, Index, T, Toolset, getOptions());

            Checks[Index].ForeColor = Color.LightBlue;
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            if (Runned)
            {
                // Отмена
                Cancel = true;

                Log.WriteLine("Cancelled.");
            }
            else
            {
                Cancel = false;
                Log.WriteLine("Project:" + CurrentProject.Name);
                Log.WriteLine("Script:" + CurrentScript.Name);
                
                Array.ForEach(ToolOptionsCB, C => { C.Enabled = false; } );

                for (int i = 0; i < Checks.Length; i++)
                {
                    Checks[i].Tag = false;
                    Checks[i].Checked = false;
                    Checks[i].ForeColor = Color.Black;
                }

                SetErrorText("", Color.Red);
                if (CurrentScript.Input != null)
                {
                    if (!mInput.Check(CurrentScript.Input, tSerial.Text))
                    {
                        SetErrorText("Error: invalid input value", Color.Red);
                        return;
                    }

                    ScriptExec.SetValue(mInput.Convert(CurrentScript.Input, tSerial.Text));
                }
                RunAction(0);
                UpdateStatus(true);
            }
        }

        private void SetErrorText(string Text, Color C)
        {
            lError.Text = Text;
            lError.ForeColor = C;
        }

        private void ScriptExec_onActionCompleted(object sender, Project.ScriptEventArgs e)
        {
            bool Ok = e.Completed;
            int Index = e.Index;

            try
            {
                BeginInvoke((MethodInvoker)delegate()
                {
                    if (Ok)
                    {
                        Checks[Index].Tag = true;
                        Checks[Index].Checked = true;

                        if (e.Warning.Length > 0)
                        {
                            Checks[Index].ForeColor = Color.Orange;
                            SetErrorText(e.Warning, Color.Orange);
                        }
                        else
                            Checks[Index].ForeColor = Color.Green;
                        if (Index < CurrentScript.Actions.Count - 1)
                        {
                            // Следующий
                            if (!Cancel)
                            {
                                RunAction(Index + 1);
                            }
                            else
                            {
                                UpdateStatus(false);
                                SetErrorText("Отменено", Color.Red);
                                Cancel = false;
                            }
                        }
                        else
                        {
                            UpdateStatus(false);
                        }
                    }
                    else
                    {
                        Checks[Index].ForeColor = Color.Red;
                        UpdateStatus(false);
                        SetErrorText(e.Error, Color.Red);
                    }
                });
            }
            catch(Exception E)
            {
                SetErrorText(E.Message, Color.Red);
            }
        }

        private void ck_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox Ch = sender as CheckBox;
            bool Checked = (bool)Ch.Tag;

            //Ch.Tag = true;
            Ch.Checked = Checked;
        }

        private void cbTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTool.SelectedItem == null)
            {
                Array.ForEach(ToolOptionsCB, C => { C.Visible = false; C.Enabled = false; });
            }
            else
            {
                var Opts = (cbTool.SelectedItem as Tool.Tool).Ops;

                Array.ForEach(ToolOptionsCB, C => { C.Visible = false; C.Enabled = false; C.Items.Clear(); });
                Array.ForEach(ToolOptionsTB, C => { C.Visible = false; C.Enabled = false;  });

                for(int i = 0; (i < Opts.Count) && (i < ToolOptionsCB.Length); i++)
                {
                    if(Opts[i].GetType() == typeof(Tool.Options.OptionList))
                    {
                        var CB = ToolOptionsCB[i];

                        CB.Enabled = true;
                        CB.Visible = true;
                        CB.Items.Clear();

                        var O = Opts[i] as Tool.Options.OptionList;
                        CB.Items.AddRange(O.Items);

                        for(int j = 0; j < O.Items.Length; j++)
                        {
                            if(O.Items[j].Default)
                            {
                                CB.SelectedIndex = j;
                                break;
                            }
                        }

                        if ((CB.Items.Count > 0) && (CB.SelectedIndex < 0)) CB.SelectedIndex = 0;
                    }
                    else if (Opts[i].GetType() == typeof(Tool.Options.OptionText))
                    {
                        var O = Opts[i] as Tool.Options.OptionText;
                        var TB = ToolOptionsTB[i];

                        TB.Enabled = true;
                        TB.Visible = true;
                        TB.Text = O.Default;
                        TB.Tag = O;
                    }
                }
            }
        }

        private void tSerial_TextChanged(object sender, EventArgs e)
        {
            bPlus.Enabled = mInput.HasIncrement(CurrentScript.Input);
        }

        private void bPlus_Click(object sender, EventArgs e)
        {
            if(CurrentScript == null) return;

            SetErrorText("", Color.Red);
            if (mInput.HasIncrement(CurrentScript.Input))
            {
                if (!mInput.Check(CurrentScript.Input, tSerial.Text))
                    SetErrorText("Error: invalid input value", Color.Red);
                else
                {
                    tSerial.Text = mInput.Increment(CurrentScript.Input, tSerial.Text);

                    for (int i = 0; i < Checks.Length; i++)
                    {
                        Checks[i].Tag = false;
                        Checks[i].Checked = false;
                        Checks[i].ForeColor = Color.Black;
                    }
                }
            }
        }
    }
}
