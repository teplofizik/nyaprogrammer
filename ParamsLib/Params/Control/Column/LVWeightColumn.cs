using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Control.Column
{
    public class LVWeightColumn : LVColumn
    {
        /// <summary>
        /// Вес колонки для ширины
        /// </summary>
        public double Weight;

        public LVWeightColumn(string Name, string Tag, double Weight) 
            : base(Name, Tag)
        {
            this.Weight = Weight;
        }
    }
}
