using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Types
{
    public class IntParameter : Parameter
    {
        /// <summary>
        /// Параметр
        /// </summary>
        public int Value;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public IntParameter() { }

        /// <summary>
        /// Конструктор с указанием имени, значения
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public IntParameter(string Name, int Value)
            : base(Name)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Конструктор с указанием имени, значения и метки временного параметра
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Temp"></param>
        public IntParameter(string Name, int Value, bool Temp)
            : base(Name, Temp)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return String.Format("{0:d}", Value);
        }
    }
}
