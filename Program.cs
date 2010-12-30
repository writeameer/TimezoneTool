using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TimezoneTool
{
    public class Program
    {

        public const int ERROR_ACCESS_DENIED = 0x005;
        public const int CORSEC_E_MISSING_STRONGNAME = -2146233317;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

        public static void Main(string[] args)
        {


            if (args.Count() !=1)
            {
                Console.WriteLine("\nThis tool changes the local time zone to the specified value:\n");
                Console.WriteLine("Usage    : " + System.Diagnostics.Process.GetCurrentProcess().ProcessName +".exe <time zone>" );
                Console.WriteLine("Example  : " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe \"Atlantic Standard Time\"");
                Console.WriteLine("\nType \"" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe list\" to get a list of valid timezones");
                Environment.Exit(1);
            }

            else if (args[0]=="list")
            {
                PrintValidtimeZones();
                Environment.Exit(0);
            }
            var newTimezone = args[0];

            Console.WriteLine("Current Time Zone:{0}", TimeZone.CurrentTimeZone.StandardName);
            ChangeTimeZone(newTimezone);

            Environment.Exit(0);
        }

        public static void ChangeTimeZone(string newTimezone)
        {
            var regTimeZones = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones");

            string subKey = null;
            try
            {
                // Find Timezone in registry
                subKey = regTimeZones.GetSubKeyNames().Where(s => s == newTimezone).First();
            }
            catch (System.InvalidOperationException)
            {
                // List valid available timezones for this machine if 
                // the timezone that was passed in was invalid.


                PrintValidtimeZones();
            }


            var daylightName = (string)regTimeZones.OpenSubKey(subKey).GetValue("Dlt");
            var standardName = (string)regTimeZones.OpenSubKey(subKey).GetValue("Std");
            var tzi = (byte[])regTimeZones.OpenSubKey(subKey).GetValue("TZI");

            var regTzi = new RegistryTimeZoneInformation(tzi);

            var tz = new TimeZoneInformation
            {
                Bias = regTzi.Bias,
                DaylightBias = regTzi.DaylightBias,
                StandardBias = regTzi.StandardBias,
                DaylightDate = regTzi.DaylightDate,
                StandardDate = regTzi.StandardDate,
                DaylightName = daylightName,
                StandardName = standardName
            };

            var enablePrivilage = TokenPrivilegesAccess.EnablePrivilege("SeTimeZonePrivilege");

            if (!enablePrivilage)
            {
                Console.WriteLine("Error: Could not enable privilege: SeTimeZonePrivilege");   
                Environment.Exit(1);
            }
            

            var didSet = SetTimeZoneInformation(ref tz);
            var lastError = Marshal.GetLastWin32Error();
            TokenPrivilegesAccess.DisablePrivilege("SeTimeZonePrivilege");

  
            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", true);
            if (key != null) key.SetValue("TimeZoneKeyName", key.GetValue("StandardName"));

            if (didSet)
            {
                Console.WriteLine("New Time Zone:{0}", TimeZoneInfo.Local.Id );
            }
            else
            {

                switch (lastError)
                {
                    case Program.ERROR_ACCESS_DENIED:
                        Console.WriteLine("Error: Access denied... Try running application as administrator.");
                        break;
                    case Program.CORSEC_E_MISSING_STRONGNAME:
                        Console.WriteLine("Error: Application is not signed ... Right click the project > Signing > Check 'Sign the assembly'.");
                        break;
                    default:
                        Console.WriteLine("Win32Error: " + lastError + "\nHRESULT: " + Marshal.GetHRForLastWin32Error());
                        break;
                }
            }
        }
    
        public static void PrintValidtimeZones()
        {
               
            var timezones = TimeZoneInfo.GetSystemTimeZones();

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Timezone has to be one of the following:");
            Console.WriteLine("----------------------------------------------------------");


            var counter = 0;
            foreach (var timeZoneInfo in timezones)
            {
                counter++;
                Console.WriteLine("{0}. {1}, {2}", counter, timeZoneInfo.StandardName, timeZoneInfo.DisplayName);
            }

            Environment.Exit(1);
        }
    }


}