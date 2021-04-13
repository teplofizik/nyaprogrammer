using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Control
{
    public class ListViewGroupClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Номер группы
        /// </summary>
        public ListViewGroup Group;

        public ListViewGroupClickedEventArgs(ListViewGroup G)
        {
            Group = G;
        }
    }

    public delegate void ListViewGroupClickedEventHandler(object Sender, ListViewGroupClickedEventArgs E);
}
