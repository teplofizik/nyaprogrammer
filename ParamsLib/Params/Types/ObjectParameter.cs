using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Types
{
    class ObjectParameter: Parameter
    {
        /// <summary>
        /// Параметр
        /// </summary>
        public object Value;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public ObjectParameter() { }

        /// <summary>
        /// Конструктор с указанием имени, значения
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public ObjectParameter(string Name, object Value)
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
        public ObjectParameter(string Name, object Value, bool Temp)
            : base(Name, Temp)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return (Value != null) ? String.Format("{0:s}", Value.ToString()) : "null";
        }
    }
}
