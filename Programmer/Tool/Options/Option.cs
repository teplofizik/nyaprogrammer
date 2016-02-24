using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.Options
{
    abstract class Option
    {
        /// <summary>
        /// Название опции
        /// </summary>
        public string Name;
        
        public Option(string N)
        {
            Name = N;
        }

        public override string ToString()
        {
            return (Name != null) ? Name : base.ToString();
        }
    }
}
