using Control.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Control
{
    public class AdvancedListView : ListView
    {
        private const int LVM_FIRST = 0x1000;                    // ListView messages
        private const int LVM_SETGROUPINFO = (LVM_FIRST + 147);  // Setinfo on Group
        private const int LVM_SUBITEMHITTEST = (LVM_FIRST + 57); // ListView SendMessage to check for a sub-item hit test
        private const int LVM_HITTEST = (LVM_FIRST + 18);        // ListView SendMessage to check for an item hit test
        private const int LVM_GETGROUPSTATE = LVM_FIRST + 92;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        public event ListViewGroupClickedEventHandler onGroupClicked;

        private delegate void CallBackSetGroupState
		    (ListViewGroup lstvwgrp, ListViewGroupState state);
        private delegate void CallbackSetGroupString(ListViewGroup lstvwgrp, string value);

        [DllImport("User32.dll")]
        private static extern int SendMessage  (IntPtr hWnd, int Msg, int wParam, LVGROUP lParam);

        [DllImport("User32.dll")]
        private static extern int SendMessage (IntPtr hWnd, int msg, int wParam, LVHITTESTINFO lParam);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);     

        private static int? GetGroupID(ListViewGroup lstvwgrp)
        {
            int? rtnval = null;
            Type GrpTp = lstvwgrp.GetType();
            if (GrpTp != null)
            {
                PropertyInfo pi = GrpTp.GetProperty("ID", BindingFlags.NonPublic |
                                BindingFlags.Instance);
                if (pi != null)
                {
                    object tmprtnval = pi.GetValue(lstvwgrp, null);
                    if (tmprtnval != null)
                    {
                        rtnval = tmprtnval as int?;
                    }
                }
            }
            return rtnval;
        }

        public int? getGroupID(ListViewGroup G)
        {
            return GetGroupID(G);
        }

        private static ListViewGroupState getGrpState(ListViewGroup lstvwgrp, ListViewGroupState mask)
        {
            if (Environment.OSVersion.Version.Major < 6)   //Only Vista and forward 
                return 0;
            if (lstvwgrp == null || lstvwgrp.ListView == null) return 0;

            {
                int? GrpId = GetGroupID(lstvwgrp);

                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);

                if (GrpId.HasValue)
                    return (ListViewGroupState)SendMessage(lstvwgrp.ListView.Handle, LVM_GETGROUPSTATE, GrpId.Value, Convert.ToInt32(mask));
                else
                    return 0;
            }
        }

        private static void setGrpState(ListViewGroup lstvwgrp, ListViewGroupState state)
        {
            if (Environment.OSVersion.Version.Major < 6)   //Only Vista and forward 
                // allows collapse of ListViewGroups
                return;
            if (lstvwgrp == null || lstvwgrp.ListView == null)
                return;
            if (lstvwgrp.ListView.InvokeRequired)
                lstvwgrp.ListView.Invoke(new CallBackSetGroupState(setGrpState),
                                lstvwgrp, state);
            else
            {
                int? GrpId = GetGroupID(lstvwgrp);
                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
                LVGROUP group = new LVGROUP();
                group.CbSize = Marshal.SizeOf(group);
                group.State = state;
                group.Mask = ListViewGroupMask.State;
                if (GrpId != null)
                {
                    group.IGroupId = GrpId.Value;
                    SendMessage(lstvwgrp.ListView.Handle,
                    LVM_SETGROUPINFO, GrpId.Value, group);
                    SendMessage(lstvwgrp.ListView.Handle,
                    LVM_SETGROUPINFO, GrpId.Value, group);
                }
                else
                {
                    group.IGroupId = gIndex;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                    //SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                }
                lstvwgrp.ListView.Refresh();
            }
        }

        private static void setGrpFooter(ListViewGroup lstvwgrp, string footer)
        {
            if (Environment.OSVersion.Version.Major < 6)   //Only Vista and forward 
                //allows footer on ListViewGroups
                return;
            if (lstvwgrp == null || lstvwgrp.ListView == null)
                return;
            if (lstvwgrp.ListView.InvokeRequired)
                lstvwgrp.ListView.Invoke(new CallbackSetGroupString(setGrpFooter),
                                lstvwgrp, footer);
            else
            {
                int? GrpId = GetGroupID(lstvwgrp);
                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
                LVGROUP group = new LVGROUP();
                group.CbSize = Marshal.SizeOf(group);
                group.PszFooter = footer;
                group.Mask = ListViewGroupMask.Footer;
                if (GrpId != null)
                {
                    group.IGroupId = GrpId.Value;
                    SendMessage(lstvwgrp.ListView.Handle,
                    LVM_SETGROUPINFO, GrpId.Value, group);
                }
                else
                {
                    group.IGroupId = gIndex;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                }
            }
        }

        public void SetGroupState(ListViewGroupState state)
        {
            foreach (ListViewGroup lvg in this.Groups)
                setGrpState(lvg, state);
        }

        public void SetGroupFooter(ListViewGroup lvg, string footerText)
        {
            setGrpFooter(lvg, footerText);
        }

        /// <summary>
        /// convert the IntPtr LParam to an Point.
        /// </summary>
        private static Point LParamToPoint(IntPtr lparam)
        {
            return new Point(lparam.ToInt32() & 0xFFFF, lparam.ToInt32() >> 16);
        }

        private LVHITTESTINFO SubitemHitTest(ref Message m)
        {
            if ((m.Msg == WM_LBUTTONDOWN) || (m.Msg == WM_LBUTTONUP))
            {
                Point hitPoint = LParamToPoint(m.LParam);
                LVHITTESTINFO lvHitTestInfo = new LVHITTESTINFO();
                lvHitTestInfo.pt.x = hitPoint.X;
                lvHitTestInfo.pt.y = hitPoint.Y;

                if (SendMessage(Handle, LVM_SUBITEMHITTEST, -1, lvHitTestInfo) != -1)
                    return lvHitTestInfo;
                else
                    return null;
            }
            else
                return null;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                LVHITTESTINFO lvHitTestInfo = SubitemHitTest(ref m);

                if (lvHitTestInfo != null)
                {
                    if ((lvHitTestInfo.flags & LVHITTESTFLAGS.LVHT_EX_GROUP_HEADER) != 0)
                        return;
                }
            }
            else if (m.Msg == WM_LBUTTONUP)
            {
                LVHITTESTINFO lvHitTestInfo = SubitemHitTest(ref m);

                if (lvHitTestInfo != null)
                {
                    // if the ListViewGroup was clicked and something is 
                    if ((lvHitTestInfo.flags & LVHITTESTFLAGS.LVHT_EX_GROUP_HEADER) != 0)
                    {
                        foreach (ListViewGroup G in Groups)
                        {
                            int? ID = GetGroupID(G);
                            if (ID.HasValue)
                            {
                                if (ID.Value == lvHitTestInfo.iItem)
                                {
                                    var S = getGrpState(G, ListViewGroupState.Collapsed | ListViewGroupState.Normal);
                                    if (onGroupClicked != null) onGroupClicked(this, new ListViewGroupClickedEventArgs(G));
                                    break;
                                }
                            }
                        }
                        return;
                    }
                }

                base.DefWndProc(ref m);
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Свёрнута ли группа
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        public bool isGroupCollapsed(ListViewGroup G)
        {
            return (Convert.ToInt32(getGrpState(G, ListViewGroupState.Collapsed)) & Convert.ToInt32(ListViewGroupState.Collapsed)) != 0;
        }

        public void CollapseGroup(ListViewGroup G)
        {
            setGrpState(G, ListViewGroupState.Collapsed);
        }

        public void ExpandGroup(ListViewGroup G)
        {
            setGrpState(G, ListViewGroupState.Normal);
        }
    }
}
