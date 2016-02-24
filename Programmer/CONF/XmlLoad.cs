using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Programmer.CONF
{
    class XmlLoad
    {
        private XmlReader F;

        public XmlLoad()
        {
 
        }

        public XmlLoad(XmlReader F)
        {
            this.F = F;
        }

        public bool Load(string FileName)
        {
            if (!File.Exists(FileName)) return false;
            if ((new FileInfo(FileName).Length) == 0)
            {
                File.Delete(FileName);
                return false;
            }

            F = XmlReader.Create(FileName);

            try
            {
                F.MoveToContent();
            }
            catch (Exception)
            {
                F.Close();

                File.Delete(FileName);
                return false;
            }

            return true;
        }

        public XmlLoad GetSubtree()
        {
            XmlReader R = F.ReadSubtree();

            // Read extern element
            R.Read();

            return new XmlLoad(R);
        }

        public void Close()
        {
            // F.Skip();
            F.Close();
        }

        public bool Read()
        {
            while (true)
            {
                try
                {
                    if (!F.Read()) return false;
                }
                catch(Exception E)
                {
                    Log.WriteLine("Error while reading xml: " + F.BaseURI);
                    Log.WriteLine(E.Message);
                    return false;
                }
                if (F.NodeType == XmlNodeType.Element) return true;
            }
        }

        public string ElementName
        {
            get { return F.Name;  }
        }

        public bool HasAttribute(string Name)
        {
            return F.GetAttribute(Name) != null;
        }

        public string[] GetAttributeNames()
        {
            var Args = new List<string>();
            if (F.HasAttributes) {
                while (F.MoveToNextAttribute()) {
                    Args.Add(F.Name);
                };
                F.MoveToElement();
            }

            return Args.ToArray();
        }

        public string GetAttribute(string Name)
        {
            return F.GetAttribute(Name);
        }

        public int GetIntAttribute(string Name)
        {
            var A = F.GetAttribute(Name);
            return (A != null) ? Convert.ToInt32(A) : 0;
        }

        public UInt64 GetUInt64Attribute(string Name)
        {
            return Convert.ToUInt64(F.GetAttribute(Name));
        }

        public int[] ReadIntArray()
        {
            int[] Array = new int[4];
            Array[0] = Convert.ToInt32(F.GetAttribute("A"));
            Array[1] = Convert.ToInt32(F.GetAttribute("B"));
            Array[2] = Convert.ToInt32(F.GetAttribute("C"));
            if(F.GetAttribute("D") != null) Array[3] = Convert.ToInt32(F.GetAttribute("D"));
            return Array;
        }

        public string[] ReadStringArray()
        {
            string[] Array = new string[4];
            Array[0] = F.GetAttribute("A");
            Array[1] = F.GetAttribute("B");
            Array[2] = F.GetAttribute("C");
            if (F.GetAttribute("D") != null) Array[3] = F.GetAttribute("D");
            return Array;
        }

        public bool[] ReadBoolArray()
        {
            bool[] Array = new bool[4];
            Array[0] = (Convert.ToUInt32(F.GetAttribute("A")) != 0);
            Array[1] = (Convert.ToUInt32(F.GetAttribute("B")) != 0);
            Array[2] = (Convert.ToUInt32(F.GetAttribute("C")) != 0);
            if (F.GetAttribute("D") != null) Array[3] = (Convert.ToUInt32(F.GetAttribute("D")) != 0);
            return Array;
        }
    }
}
