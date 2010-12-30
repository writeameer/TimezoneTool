using System;
using System.Runtime.InteropServices;

namespace TimezoneTool
{
    public class TokenPrivilegesAccess
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess,
                                                  ref int tokenhandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeValue(string lpsystemname, string lpname,
                                                      [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs,
                                                       [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGE Newstate, int bufferlength,
                                                       int PreivousState, int Returnlength);

        public const int TokenAssignPrimary = 0x00000001;
        public const int TokenDuplicate = 0x00000002;
        public const int TokenImpersonate = 0x00000004;
        public const int TokenQuery = 0x00000008;
        public const int TokenQuerySource = 0x00000010;
        public const int TokenAdjustPrivileges = 0x00000020;
        public const int TokenAdjustGroups = 0x00000040;
        public const int TokenAdjustDefault = 0x00000080;

        public const UInt32 SePrivilegeEnabledByDefault = 0x00000001;
        public const UInt32 SePrivilegeEnabled = 0x00000002;
        public const UInt32 SePrivilegeRemoved = 0x00000004;
        public const UInt32 SePrivilegeUsedForAccess = 0x80000000;

        public static bool EnablePrivilege(string privilege)
        {
            try
            {
                int token = 0;
                int retVal = 0;

                var tp = new TOKEN_PRIVILEGE();
                var ld = new LUID();

                retVal = OpenProcessToken(GetCurrentProcess(), TokenAdjustPrivileges | TokenQuery, ref token);
                retVal = LookupPrivilegeValue(null, privilege, ref ld);
                tp.PrivilegeCount = 1;

                var luidAndAtt = new LUID_AND_ATTRIBUTES {Attributes = SePrivilegeEnabled, Luid = ld};
                tp.Privilege = luidAndAtt;

                retVal = AdjustTokenPrivileges(token, 0, ref tp, 1024, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DisablePrivilege(string privilege)
        {
            try
            {
                var token = 0;
                var retVal = 0;

                var tp = new TOKEN_PRIVILEGE();
                var ld = new LUID();

                retVal = OpenProcessToken(GetCurrentProcess(), TokenAdjustPrivileges | TokenQuery, ref token);
                retVal = LookupPrivilegeValue(null, privilege, ref ld);
                tp.PrivilegeCount = 1;
                // TP.Attributes should be none (not set) to disable privilege
                var luidAndAtt = new LUID_AND_ATTRIBUTES {Luid = ld};
                tp.Privilege = luidAndAtt;

                retVal = AdjustTokenPrivileges(token, 0, ref tp, 1024, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}