using System.Runtime.InteropServices;

namespace TimezoneTool
{

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SystemTime
    {
        [MarshalAs(UnmanagedType.U2)]
        public short Year;
        [MarshalAs(UnmanagedType.U2)]
        public short Month;
        [MarshalAs(UnmanagedType.U2)]
        public short DayOfWeek;
        [MarshalAs(UnmanagedType.U2)]
        public short Day;
        [MarshalAs(UnmanagedType.U2)]
        public short Hour;
        [MarshalAs(UnmanagedType.U2)]
        public short Minute;
        [MarshalAs(UnmanagedType.U2)]
        public short Second;
        [MarshalAs(UnmanagedType.U2)]
        public short Milliseconds;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TimeZoneInformation
    {
        [MarshalAs(UnmanagedType.I4)]
        public int Bias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string StandardName;
        public SystemTime StandardDate;
        [MarshalAs(UnmanagedType.I4)]
        public int StandardBias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string DaylightName;
        public SystemTime DaylightDate;
        [MarshalAs(UnmanagedType.I4)]
        public int DaylightBias;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LUID
    {
        internal uint LowPart;
        internal uint HighPart;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LUID_AND_ATTRIBUTES
    {
        internal LUID Luid;
        internal uint Attributes;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TOKEN_PRIVILEGE
    {
        internal uint PrivilegeCount;
        internal LUID_AND_ATTRIBUTES Privilege;
    }
}