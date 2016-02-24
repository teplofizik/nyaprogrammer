using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Input
{
    class InputType
    {
        /// <summary>
        /// Название типа
        /// </summary>
        public string Name;

        /// <summary>
        /// Проверка значения согласно типу
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public Func<string, bool> Check;

        /// <summary>
        /// Преобразование значения в выходной формат
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public Func<string, string> Convert;

        /// <summary>
        /// Инкремент
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public Func<string, string> Increment;

        public InputType(string N, Func<string, bool> Chk, Func<string, string> Cnv, Func<string, string> Inc)
        {
            Name = N;
            Check = Chk;
            Convert = Cnv;
            Increment = Inc;
        }
    }
}
