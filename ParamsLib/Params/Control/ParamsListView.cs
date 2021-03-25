using Params.Control.Column;
using Params.Control.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Control;

namespace Params.Control
{
    public class ParamsListView : AdvancedListView
    {
        /// <summary>
        /// Колонки
        /// </summary>
        private CustomColumnHeaderCollection mColumns;

        /// <summary>
        /// Записи
        /// </summary>
        private ParamsListViewItemCollection mItems;

        public ParamsListView() : base()
        {
            mColumns = new CustomColumnHeaderCollection(Columns);
            mItems = new ParamsListViewItemCollection(this, mColumns);

            ShowGroups = true;
            HideSelection = false;
            MultiSelect = false;
            FullRowSelect = true;
            View = System.Windows.Forms.View.Details;

            //Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        /// <summary>
        /// Выбранный пункт
        /// </summary>
        public NotifiedParameterStorage SelectedNewItem
        {
            get { return (SelectedIndices.Count > 0) ? NewItems[SelectedIndices[0]] : null; }
        }

        /// <summary>
        /// Выбранный пункт
        /// </summary>
        public ListViewItem SelectedItem
        {
            get { return (SelectedItems.Count > 0) ? SelectedItems[0] : null; }
        }

        /// <summary>
        /// Колонки динамической ширины
        /// </summary>
        public IList<LVColumn> NewColumns
        {
            get { return mColumns; }
           // set { if (value != null) { mColumns = value; mItems.setColumns(mColumns); RecalcColumns(); } }
        }

        /// <summary>
        /// Записи
        /// </summary>
        public ParamsListViewItemCollection NewItems
        {
            get { return mItems; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            mColumns.setWidth(ClientSize.Width);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        /// <summary>
        /// Пересчитать колонки
        /// </summary>
        private void RecalcColumns()
        {
            double Weights = mColumns.Sum(I => (I.GetType() == typeof(LVWeightColumn)) ? (I as LVWeightColumn).Weight : 0);
            int Widths = mColumns.Sum(I => (I.GetType() == typeof(LVWidthColumn)) ? (I as LVWidthColumn).Width : 0);
            int FreeWidth = Width - Widths;

            Columns.Clear();
            foreach(var C in NewColumns)
            {
                var CH = new ColumnHeader();
                CH.Text = C.Name;
                CH.Tag = C.Tag;

                if(C.GetType() == typeof(LVWeightColumn))
                    CH.Width = Convert.ToInt32(FreeWidth * (C as LVWeightColumn).Weight / Weights) - 1;
                else if (C.GetType() == typeof(LVWidthColumn))
                    CH.Width = (C as LVWidthColumn).Width;

                Columns.Add(CH);
            }
        }
        
        /// <summary>
        /// Пересоздать записи
        /// </summary>
        private void RecreateItems()
        {
            BeginUpdate();
            mItems.RecreateItems();
            EndUpdate();
        }
    }
}
