using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Control.Types
{
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), 
	Description("LVGROUP StructureUsed to set and retrieve groups.")]
public struct LVGROUP
{
    /// <summary>
    /// Size of this structure, in bytes.
    /// </summary>
    [Description("Size of this structure, in bytes.")]
    public int CbSize;
    /// <summary>
    /// Mask that specifies which members of the structure are valid input. 
    /// One or more of the following values:LVGF_NONENo other items are valid.
    /// </summary>
    [Description("Mask that specifies which members of the structure are valid input. "+
	    "One or more of the following values:LVGF_NONE No other items are valid.")]
    public ListViewGroupMask Mask;
    /// <summary>
    /// Pointer to a null-terminated string that contains the header text 
    /// when item information is being set. If group information is being retrieved, 
    /// this member specifies the address of the buffer that receives the header text.
    /// </summary>
    [Description("Pointer to a null-terminated string that contains the header text "+
	    "when item information is being set. If group information is being retrieved, "+
	    "this member specifies the address of the buffer that "+
	    "receives the header text.")]
    [MarshalAs(UnmanagedType.LPWStr)]
    public string PszHeader;
    /// <summary>
    /// Size in TCHARs of the buffer pointed to by the pszHeader member. 
    /// If the structure is not receiving information about a group, 
    /// this member is ignored.
    /// </summary>
    [Description("Size in TCHARs of the buffer pointed to by the pszHeader member. "+
	"If the structure is not receiving information about a group, "+
	"this member is ignored.")]
    public int CchHeader;
    /// <summary>
    /// Pointer to a null-terminated string that contains the footer text 
    /// when item information is being set. If group information is being retrieved, 
    /// this member specifies the address of the buffer that receives the footer text.
    /// </summary>
    [Description("Pointer to a null-terminated string that contains the "+
	    "footer text when item information is being set. "+
	    "If group information is being retrieved, this member specifies the "+
	    "address of the buffer that receives the footer text.")]
    [MarshalAs(UnmanagedType.LPWStr)]
    public string PszFooter;
    /// <summary>
    /// Size in TCHARs of the buffer pointed to by the pszFooter member. 
    /// If the structure is not receiving information about a group, 
    /// this member is ignored.
    /// </summary>
    [Description("Size in TCHARs of the buffer pointed to by the pszFooter member. "+
	    "If the structure is not receiving information about a group, "+
	    "this member is ignored.")]
    public int CchFooter;
    /// <summary>
    /// ID of the group.
    /// </summary>
    [Description("ID of the group.")]
    public int IGroupId;
    /// <summary>
    /// Mask used with LVM_GETGROUPINFO (Microsoft Windows XP and Windows Vista) 
    /// and LVM_SETGROUPINFO (Windows Vista only) to specify which flags in the 
    /// state value are being retrieved or set.
    /// </summary>
    [Description("Mask used with LVM_GETGROUPINFO "+
	    "(Microsoft Windows XP and Windows Vista) and LVM_SETGROUPINFO "+
	    "(Windows Vista only) to specify which flags in the state value are "+
	    "being retrieved or set.")]
    public int StateMask;
    /// <summary>
    /// Flag that can have one of the following values:LVGS_NORMALGroups are expanded, 
    /// the group name is displayed, and all items in the group are displayed.
    /// </summary>
    [Description("Flag that can have one of the following values:LVGS_NORMAL "+
	    "Groups are expanded, the group name is displayed, and all items "+
	    "in the group are displayed.")]
    public ListViewGroupState State;
    /// <summary>
    /// Indicates the alignment of the header or footer text for the group. 
    /// It can have one or more of the following values. Use one of the header flags. 
    /// Footer flags are optional. 
    /// Windows XP: Footer flags are reserved.LVGA_FOOTER_CENTERReserved.
    /// </summary>
    [Description("Indicates the alignment of the header or footer text for the group. "+
	    "It can have one or more of the following values. Use one of the header flags. "+
	    "Footer flags are optional. Windows XP: "+
	    "Footer flags are reserved.LVGA_FOOTER_CENTERReserved.")]
    public uint UAlign;
    /// <summary>
    /// Windows Vista. Pointer to a null-terminated string that contains the 
    /// subtitle text when item information is being set. If group information 
    /// is being retrieved, this member specifies the address of the buffer that 
    /// receives the subtitle text. This element is drawn under the header text.
    /// </summary>
    [Description("Windows Vista. Pointer to a null-terminated string that "+
    	"contains the subtitle text when item information is being set. "+
    	"If group information is being retrieved, this member specifies the "+
    	"address of the buffer that receives the subtitle text. "+
    	"This element is drawn under the header text.")]
    public IntPtr PszSubtitle;
    /// <summary>
    /// Windows Vista. Size, in TCHARs, of the buffer pointed to by the 
    /// pszSubtitle member. If the structure is not receiving information 
    /// about a group, this member is ignored.
    /// </summary>
    [Description("Windows Vista. Size, in TCHARs, of the buffer pointed "+
    	"to by the pszSubtitle member. If the structure is not receiving information "+
    	"about a group, this member is ignored.")]
    public uint CchSubtitle;
    /// <summary>
    /// Windows Vista. Pointer to a null-terminated string that contains the text 
    /// for a task link when item information is being set. If group information 
    /// is being retrieved, this member specifies the address of the buffer 
    /// that receives the task text. This item is drawn right-aligned opposite 
    /// the header text. When clicked by the user, 
    /// the task link generates an LVN_LINKCLICK notification.
    /// </summary>
    [Description("Windows Vista. Pointer to a null-terminated string that "+
    	"contains the text for a task link when item information is being set. "+
    	"If group information is being retrieved, this member specifies the address "+
    	"of the buffer that receives the task text. This item is drawn right-aligned "+
    	"opposite the header text. When clicked by the user, the task link "+
    	"generates an LVN_LINKCLICK notification.")]
    [MarshalAs(UnmanagedType.LPWStr)]
    public string PszTask;
    /// <summary>
    /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszTask member. 
    /// If the structure is not receiving information about a group, 
    /// this member is ignored.
    /// </summary>
    [Description("Windows Vista. Size in TCHARs of the buffer pointed to "+
    	"by the pszTask member. If the structure is not receiving information "+
    	"about a group, this member is ignored.")]
    public uint CchTask;
    /// <summary>
    /// Windows Vista. Pointer to a null-terminated string that contains the 
    /// top description text when item information is being set. 
    /// If group information is being retrieved, this member specifies the address 
    /// of the buffer that receives the top description text. 
    /// This item is drawn opposite the title image when there is a title image, 
    /// no extended image, and uAlign==LVGA_HEADER_CENTER.
    /// </summary>
    [Description("Windows Vista. Pointer to a null-terminated string that "+
    	"contains the top description text when item information is being set. "+
    	"If group information is being retrieved, this member specifies the address "+
    	"of the buffer that receives the top description text. "+
    	"This item is drawn opposite the title image when there is a title image, "+
    	"no extended image, and uAlign==LVGA_HEADER_CENTER.")]
    [MarshalAs(UnmanagedType.LPWStr)]
    public string PszDescriptionTop;
    /// <summary>
    /// Windows Vista. Size in TCHARs of the buffer pointed to by the 
    /// pszDescriptionTop member. If the structure is not receiving information 
    /// about a group, this member is ignored.
    /// </summary>
    [Description("Windows Vista. Size in TCHARs of the buffer pointed to "+
    	"by the pszDescriptionTop member. If the structure is not receiving "+
    	"information about a group, this member is ignored.")]
    public uint CchDescriptionTop;
    /// <summary>
    /// Windows Vista. Pointer to a null-terminated string that contains the 
    /// bottom description text when item information is being set. 
    /// If group information is being retrieved, this member specifies the address 
    /// of the buffer that receives the bottom description text. 
    /// This item is drawn under the top description text when there is a title image, 
    /// no extended image, and uAlign==LVGA_HEADER_CENTER.
    /// </summary>
    [Description("Windows Vista. Pointer to a null-terminated string that "+
    	"contains the bottom description text when item information is being set. "+
    	"If group information is being retrieved, this member specifies the address "+
    	"of the buffer that receives the bottom description text. "+
    	"This item is drawn under the top description text when there is a title image, "+
    	"no extended image, and uAlign==LVGA_HEADER_CENTER.")]
    [MarshalAs(UnmanagedType.LPWStr)]
    public string PszDescriptionBottom;
    /// <summary>
    /// Windows Vista. Size in TCHARs of the buffer pointed to by the 
    /// pszDescriptionBottom member. If the structure is not receiving 
    /// information about a group, this member is ignored.
    /// </summary>
    [Description("Windows Vista. Size in TCHARs of the buffer pointed "+
    	"to by the pszDescriptionBottom member. If the structure is not receiving "+
    	"information about a group, this member is ignored.")]
    public uint CchDescriptionBottom;
    /// <summary>
    /// Windows Vista. Index of the title image in the control imagelist.
    /// </summary>
    [Description("Windows Vista. Index of the title image in the control imagelist.")]
    public int ITitleImage;
    /// <summary>
    /// Windows Vista. Index of the extended image in the control imagelist.
    /// </summary>
    [Description("Windows Vista. Index of the extended image in the control imagelist.")]
    public int IExtendedImage;
    /// <summary>
    /// Windows Vista. Read-only.
    /// </summary>
    [Description("Windows Vista. Read-only.")]
    public int IFirstItem;
    /// <summary>
    /// Windows Vista. Read-only in non-owner data mode.
    /// </summary>
    [Description("Windows Vista. Read-only in non-owner data mode.")]
    public IntPtr CItems;
    /// <summary>
    /// Windows Vista. NULL if group is not a subset. 
    /// Pointer to a null-terminated string 
    /// that contains the subset title text when item information is being set. 
    /// If group information is being retrieved, this member specifies the address 
    /// of the buffer that receives the subset title text.
    /// </summary>
    [Description("Windows Vista. NULL if group is not a subset. "+
    	"Pointer to a null-terminated string that contains the subset title text "+
    	"when item information is being set. If group information is being retrieved, "+
    	"this member specifies the address of the buffer that "+
	    "receives the subset title text.")]
    public IntPtr PszSubsetTitle;
    /// <summary>
    /// Windows Vista. Size in TCHARs of the buffer pointed to by the 
    /// pszSubsetTitle member. 
    /// If the structure is not receiving information about a group, 
    /// this member is ignored.
    /// </summary>
    [Description("Windows Vista. Size in TCHARs of the buffer pointed to "+
    	"by the pszSubsetTitle member. If the structure is not receiving "+
    	"information about a group, this member is ignored.")]
    public IntPtr CchSubsetTitle;
}
}
