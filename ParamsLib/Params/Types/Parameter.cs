using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Params.Types
{
    [XmlInclude(typeof(IntParameter))]
    [XmlInclude(typeof(DoubleParameter))]
    [XmlInclude(typeof(StringParameter))]
    [XmlInclude(typeof(ObjectParameter))]
    public class Parameter
    {
        /// <summary>
        /// Название параметра
        /// </summary>
        public string Name;

        /// <summary>
        /// Временный
        /// </summary>
        public bool Temporary = false;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        /// <param name="N"></param>
        /// <param name="Temp"></param>
        public Parameter() { }

        /// <summary>
        /// Конструктор с указанием названия
        /// </summary>
        /// <param name="N"></param>
        /// <param name="Temp"></param>
        public Parameter(string N)
        {
            Name = N;
        }

        /// <summary>
        /// Конструктор с указанием названия и метки временного параметра
        /// </summary>
        /// <param name="N"></param>
        /// <param name="Temp"></param>
        public Parameter(string N, bool Temp)
        {
            Name = N;
            Temporary = Temp;
        }
    }
}
