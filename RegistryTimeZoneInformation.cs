using System;
using System.Runtime.InteropServices;

namespace TimezoneTool
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RegistryTimeZoneInformation
    {
        [MarshalAs(UnmanagedType.I4)]
        public int Bias;
        [MarshalAs(UnmanagedType.I4)]
        public int StandardBias;
        [MarshalAs(UnmanagedType.I4)]
        public int DaylightBias;
        public SystemTime StandardDate;
        public SystemTime DaylightDate;

        public RegistryTimeZoneInformation(TimeZoneInformation tzi)
        {
            Bias = tzi.Bias;
            StandardDate = tzi.StandardDate;
            StandardBias = tzi.StandardBias;
            DaylightDate = tzi.DaylightDate;
            DaylightBias = tzi.DaylightBias;
        }

        public RegistryTimeZoneInformation(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length != 0x2c))
            {
                throw new ArgumentException("Argument_InvalidREG_TZI_FORMAT");
            }
            Bias = BitConverter.ToInt32(bytes, 0);
            StandardBias = BitConverter.ToInt32(bytes, 4);
            DaylightBias = BitConverter.ToInt32(bytes, 8);
            StandardDate.Year = BitConverter.ToInt16(bytes, 12);
            StandardDate.Month = BitConverter.ToInt16(bytes, 14);
            StandardDate.DayOfWeek = BitConverter.ToInt16(bytes, 0x10);
            StandardDate.Day = BitConverter.ToInt16(bytes, 0x12);
            StandardDate.Hour = BitConverter.ToInt16(bytes, 20);
            StandardDate.Minute = BitConverter.ToInt16(bytes, 0x16);
            StandardDate.Second = BitConverter.ToInt16(bytes, 0x18);
            StandardDate.Milliseconds = BitConverter.ToInt16(bytes, 0x1a);
            DaylightDate.Year = BitConverter.ToInt16(bytes, 0x1c);
            DaylightDate.Month = BitConverter.ToInt16(bytes, 30);
            DaylightDate.DayOfWeek = BitConverter.ToInt16(bytes, 0x20);
            DaylightDate.Day = BitConverter.ToInt16(bytes, 0x22);
            DaylightDate.Hour = BitConverter.ToInt16(bytes, 0x24);
            DaylightDate.Minute = BitConverter.ToInt16(bytes, 0x26);
            DaylightDate.Second = BitConverter.ToInt16(bytes, 40);
            DaylightDate.Milliseconds = BitConverter.ToInt16(bytes, 0x2a);
        }
    }
}