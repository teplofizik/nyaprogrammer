using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Control.Column
{
    public class LVColumn
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Какую величину выводим в этом столбце?
        /// </summary>
        public string Tag = "";

        /// <summary>
        /// Вычисленная ширина
        /// </summary>
        public int CalculatedWidth = 0;

        public LVColumn(string Name, string Tag)
        {
            this.Name = Name;
            this.Tag = Tag;
        }

        public void setCalculatedWidth(int Value)
        {
            CalculatedWidth = Value;
        }
    }
}
