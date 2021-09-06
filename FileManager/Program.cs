using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace FileManager
{
    class Program
    {
        public static bool wasCleared = false;
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:

                    // CODE HERE WHEN CLOSED
                    AddUpdateAppSettings("Path", FileManager.DefaultPath);
                    AddUpdateAppSettings("PageNum", FileManager.PageNumber.ToString());

                    Environment.Exit(0);
                    return false;

                default:
                    return false;
            }
        }

        // local drivers
        private static string[] drives = System.Environment.GetLogicalDrives();
        private static string  defaultPath = string.Empty;
        private static int pageNumber = 1;

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(Handler, true); // Register the handle 
            SetConsoleUtf8();
            SetConsoleSize();
            SetBackgroundColorConsole();

            LoadAppSettings();

            // load default drive or args (c:\)
            if (defaultPath==string.Empty)
                defaultPath = SetDefaultDrivePath(args);

            FileManager fileManager = new FileManager(defaultPath, pageNumber);
        }

        // app console configuration
        #region Set default console size
        private static void SetConsoleSize()
        {
            Console.SetWindowSize(110, 60);
        }
        #endregion

        #region Set utf encoding console
        private static void SetConsoleUtf8()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        #endregion

        #region set default drive
        private static string SetDefaultDrivePath(string [] args)
        {
            bool rootDriveExist = false;
            string anotherDrive = string.Empty;

            foreach (var drive in drives)
            {
                if (args[0].ToString().ToLower() == drive.ToString().ToLower())
                {
                    rootDriveExist = true;
                    break;
                }

                DriveInfo GetInfo = new DriveInfo(drive);

                if (GetInfo.DriveType == DriveType.CDRom || GetInfo.DriveType == DriveType.Network)
                {
                    continue;
                }
                else
                {
                    anotherDrive = drive.ToString();
                }
            }




            if (rootDriveExist)
            {
                return args[0];
            }
            else
            {
                return anotherDrive;
            }
        }
        #endregion

        #region  set default background color

        private static void SetBackgroundColorConsole()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion


        #region Load Settings
        private static void LoadAppSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count != 0)
                {
                    defaultPath = appSettings["Path"];
                    pageNumber = Convert.ToInt32(appSettings["PageNum"]);
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }         
        }
        #endregion

        #region Save settings
        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        #endregion
    }
}
