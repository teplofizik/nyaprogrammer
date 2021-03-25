using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Params.Control.Column
{
    public class LVWidthColumn : LVColumn
    {
        /// <summary>
        /// Вес колонки для ширины
        /// </summary>
        public int Width;

        public LVWidthColumn(string Name, string Tag, int Width) 
            : base(Name, Tag)
        {
            this.Width = Width;
        }
    }
}
