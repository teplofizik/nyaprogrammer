using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.Options
{
    class OptionText : Option
    {
        public string ParamName = "";
        public string Default = "";

        public OptionText(string Name) : base(Name)
        {

        }
    }
}
