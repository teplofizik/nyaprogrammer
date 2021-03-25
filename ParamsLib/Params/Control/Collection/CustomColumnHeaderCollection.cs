using Params.Control.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Params.Control.Collection
{
    class CustomColumnHeaderCollection : IList<LVColumn>
    {
        private List<LVColumn> mHeaders = new List<LVColumn>();
        private ListView.ColumnHeaderCollection mColumns;

        private int mWidth = 0;

        public CustomColumnHeaderCollection(ListView.ColumnHeaderCollection Columns)
        {
            mColumns = Columns;
        }

        /// <summary>
        /// Установить ширину
        /// </summary>
        /// <param name="Value"></param>
        public void setWidth(int Value)
        {
            mWidth = Value;
            RecalcWidth();
            ResizeColumns();
        }

        /// <summary>
        /// Отмасштабировать столбцы
        /// </summary>
        private void ResizeColumns()
        {
            for (int i = 0; i < mColumns.Count; i++)
                mColumns[i].Width = mHeaders[i].CalculatedWidth;
        }

        #region IList<LVColumn> interface
        public int IndexOf(LVColumn item)
        {
            return mHeaders.IndexOf(item);
        }

        public void Insert(int index, LVColumn item)
        {
            mHeaders.Insert(index, item);
            RecalcWidth();
            ResizeColumns();
            mColumns.Insert(index, getColumn(item));
        }

        public void RemoveAt(int index)
        {
            mHeaders.RemoveAt(index);
            mColumns.RemoveAt(index);
            RecalcWidth();
            ResizeColumns();
        }

        public LVColumn this[int index]
        {
            get { return mHeaders[index]; }
            set { mHeaders[index] = value; }
        }

        public void Add(LVColumn item)
        {
            mHeaders.Add(item);
            RecalcWidth();
            ResizeColumns();

            mColumns.Add(getColumn(item));
        }

        public void Clear()
        {
            mColumns.Clear();
            mHeaders.Clear();
        }

        public bool Contains(LVColumn item)
        {
            return mHeaders.Contains(item);
        }

        public void CopyTo(LVColumn[] array, int arrayIndex)
        {
            mHeaders.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mHeaders.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(LVColumn item)
        {
            int Index = mHeaders.IndexOf(item);
            if (Index >= 0) mColumns.RemoveAt(Index);

            var Result = mHeaders.Remove(item);

            RecalcWidth();
            ResizeColumns();

            return Result;
        }

        public IEnumerator<LVColumn> GetEnumerator()
        {
            return mHeaders.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Cast<LVColumn>().GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Вычислить ширину столбцов
        /// </summary>
        private void RecalcWidth()
        {
            double Weights = mHeaders.Sum(I => (I.GetType() == typeof(LVWeightColumn)) ? (I as LVWeightColumn).Weight : 0);
            int Widths = mHeaders.Sum(I => (I.GetType() == typeof(LVWidthColumn)) ? (I as LVWidthColumn).Width : 0);
            int FreeWidth = mWidth - Widths;

            foreach (var C in mHeaders)
            {
                if (C.GetType() == typeof(LVWeightColumn))
                    C.setCalculatedWidth(Convert.ToInt32(FreeWidth * (C as LVWeightColumn).Weight / Weights) - 1);
                else if (C.GetType() == typeof(LVWidthColumn))
                    C.setCalculatedWidth((C as LVWidthColumn).Width);
            }
        }

        private ColumnHeader getColumn(LVColumn C)
        {
            ColumnHeader CH = new ColumnHeader();

            CH.Text = C.Name;
            CH.Tag = C.Tag;
            CH.Width = C.CalculatedWidth;

            return CH;
        }
    }
}
