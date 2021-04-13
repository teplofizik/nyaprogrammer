using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Control.Types
{
    [StructLayout(LayoutKind.Sequential)]
    class LVHITTESTINFO
    {
        public POINT pt;
        public LVHITTESTFLAGS flags;
        public int iItem;
        public int iSubItem;
        // Vista/Win7+
        public int iGroup;
    }
}
