using Params;
using Params.Control.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Params.Control.Collection
{
    public class ParamsListViewItemCollection : IList<NotifiedParameterStorage>
    {
      //  public event EventHandler onCollectionChanged;

        private string mGroupKey = "location";
        private ListView mOwner;
        private IList<NotifiedParameterStorage> mParams = new List<NotifiedParameterStorage>();
        private IList<LVColumn> mColumns;

        public ParamsListViewItemCollection(ListView Owner, IList<LVColumn> Columns)
        {
            mOwner = Owner;
            mColumns = Columns;
        }

        #region IList<ParameterStorage> Interface
        public int IndexOf(NotifiedParameterStorage item)
        {
            return mParams.IndexOf(item);
        }

        public void Insert(int index, NotifiedParameterStorage item)
        {
            item.onStorageUpdate += new EventHandler(item_onStorageUpdate);
            mParams.Insert(index, item);
            mOwner.Items.Insert(index, createListViewItem(item));
        }

        public void RemoveAt(int index)
        {
            mParams.RemoveAt(index);
        }

        public NotifiedParameterStorage this[int index]
        {
            get { return mParams[index]; }
            set
            {
                mParams[index] = value;
                value.onStorageUpdate += new EventHandler(item_onStorageUpdate);
            }
        }

        public void Add(NotifiedParameterStorage item)
        {
            item.onStorageUpdate += new EventHandler(item_onStorageUpdate);
            mParams.Add(item);
            mOwner.Items.Add(createListViewItem(item));
        }

        private void item_onStorageUpdate(object sender, EventArgs E)
        {
            int Index = mParams.IndexOf(sender as NotifiedParameterStorage);

            if (Index >= 0) UpdateItem(Index);
        }

        public void Clear()
        {
            mOwner.Items.Clear();
            mParams.Clear();
        }

        public bool Contains(NotifiedParameterStorage item)
        {
            return mParams.Contains(item);
        }

        public void CopyTo(NotifiedParameterStorage[] array, int arrayIndex)
        {
            mParams.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mParams.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(NotifiedParameterStorage item)
        {
            int Index = mParams.IndexOf(item);
            if(Index >= 0) mOwner.Items.RemoveAt(Index);

            return mParams.Remove(item);
        }

        public IEnumerator<NotifiedParameterStorage> GetEnumerator()
        {
            return mParams.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Cast<NotifiedParameterStorage>().GetEnumerator();
        }
#endregion

        /// <summary>
        /// Создать строку для отображения
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        private ListViewItem createListViewItem(NotifiedParameterStorage S)
        {
            ListViewItem I = new ListViewItem();

            I.Group = getGroup(S);
            // Добавим недостающие
            I.Text = (mColumns.Count > 0) ? getValue(mColumns[0], S) : "";
            if (S.hasParameter("background"))
                I.BackColor = Color.FromArgb(S.getInteger("background"));

            for (int s = 1; s < mColumns.Count; s++) I.SubItems.Add(getValue(mColumns[s], S));

            return I;
        }

        /// <summary>
        /// Получить значение из поля
        /// </summary>
        /// <param name="C"></param>
        /// <param name="S"></param>
        /// <returns></returns>
        private string getValue(LVColumn C, NotifiedParameterStorage S)
        {
            return S.hasParameter(C.Tag) ? S.getString(C.Tag) : "na";
        }

        private ListViewGroup getGroup(NotifiedParameterStorage S)
        {
            string Value = S.getString(mGroupKey) ?? "Default";

            foreach (ListViewGroup G in mOwner.Groups)
            {
                if (Value.CompareTo(G.Header) == 0) return G;
            }

            var NG = new ListViewGroup(Value);
            mOwner.Groups.Add(NG);
            return NG;
        }

        /// <summary>
        /// Задать колонки
        /// </summary>
        /// <param name="Columns"></param>
        public void setColumns(IList<LVColumn> Columns)
        {
            mColumns = Columns;
        }

        /// <summary>
        /// Пересоздать записи
        /// </summary>
        public void RecreateItems()
        {
            for (int i = 0; i < mOwner.Items.Count; i++) UpdateItem(i);
            for (int i = mOwner.Items.Count; i < mOwner.Items.Count; i++) mOwner.Items.Add(createListViewItem(mParams[i]));
        }

        /// <summary>
        /// Обновить содержимое
        /// </summary>
        /// <param name="Index"></param>
        private void UpdateItem(int Index)
        {
            var I = mOwner.Items[Index];
            var P = mParams[Index];

            if (P.hasParameter("background"))
                I.BackColor = Color.FromArgb(P.getInteger("background"));

            // Удалим лишнее
            if (I.SubItems.Count > mColumns.Count)
                for (int s = I.SubItems.Count - 1; s >= mColumns.Count; s--)
                    I.SubItems.RemoveAt(s);

            // Заполним имеющиеся поля
            for (int s = 0; s < I.SubItems.Count; s++) I.SubItems[s].Text = getValue(mColumns[s], P);

            // Добавим недостающие
            for (int s = I.SubItems.Count; s < mColumns.Count; s++) I.SubItems.Add(getValue(mColumns[s], P));
        }
    }
}
