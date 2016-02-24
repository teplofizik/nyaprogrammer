using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Project
{
    class Projects
    {
        public List<Project> ProjectList = new List<Project>();
        public string ProjectPath = "";

        public Projects(string ProjectPath)
        {
            this.ProjectPath = ProjectPath;
        }

        private void AddProject(CONF.XmlLoad X)
        {
            Project P = new Project(ProjectPath);
            
            string File = X.GetAttribute("path");
            if (P.Load(ProjectPath + "\\" + File))
            {
                ProjectList.Add(P);
            }
            else
            {
                Log.WriteLine(String.Format("Could not load project file: {0:s}", File));
            }
        }

        public void Load(string FileName)
        {
            CONF.XmlLoad X = new CONF.XmlLoad();

            if (!X.Load(FileName)) return;

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "project": AddProject(X); break;
                }
            }

            X.Close();
        }
    }
}
