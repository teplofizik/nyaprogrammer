using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Types
{
    public class DoubleParameter : Parameter
    {
        /// <summary>
        /// Параметр
        /// </summary>
        public double Value;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public DoubleParameter() { }

        /// <summary>
        /// Конструктор с указанием имени, значения
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public DoubleParameter(string Name, double Value)
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
        public DoubleParameter(string Name, double Value, bool Temp)
            : base(Name, Temp)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return String.Format("{0:00.0}", Value);
        }
    }
}
