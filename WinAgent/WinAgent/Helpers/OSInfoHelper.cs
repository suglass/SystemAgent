﻿using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinAgent.BaseModel;

namespace WinAgent.Helpers
{
    public static class OSInfoHelper
    {
        public static string getOSDescription()
        {
            string w_strOSinfo = string.Empty;
            OperatingSystem os = Environment.OSVersion;
            // w_strOSinfo += " " + os.Platform.ToString();
            // w_strOSinfo += " " + os.ServicePack.ToString();
            // w_strOSinfo += " " + os.VersionString.ToString();

            w_strOSinfo += $"{RuntimeInformation.OSDescription}";

            return w_strOSinfo;
        }
        public static string getOSPlatform()            // WinNT
        {
            return new ComputerInfo().OSPlatform;
        }
        public static string getOSVersion()             // 6.1.7601
        {
            return new ComputerInfo().OSVersion;
        }
        public static string getOSFullName()            // Mircrosoft Windows 7 Ultimate
        {
            return new ComputerInfo().OSFullName;
        }

        public static string getMachineName()
        {
            return Environment.MachineName;
        }
        public static string getOSbit()
        {
            if (Environment.Is64BitOperatingSystem)
                return "64bit";
            else
                return "32bit";
        }

        private static List<MInstalledApp> GetInstalledApplication(RegistryKey regKey, string registryKey)
        {
            List<MInstalledApp> list = new List<MInstalledApp>();
            using (Microsoft.Win32.RegistryKey key = regKey.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    foreach (string name in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(name))
                        {
                            string displayName = (string)subkey.GetValue("DisplayName");
                            string installLocation = (string)subkey.GetValue("InstallLocation");
                            string version = (string)subkey.GetValue("DisplayVersion");

                            if (!string.IsNullOrEmpty(displayName)) // && !string.IsNullOrEmpty(installLocation)
                            {
                                list.Add(new MInstalledApp()
                                {
                                    DisplayName = displayName.Trim(),
                                    InstallationLocation = installLocation,
                                    DisplayVersion = version
                                });
                            }
                        }
                    }
                }
            }

            return list;
        }

        public static List<MInstalledApp> GetFullListInstalledApplication()
        {
            IEnumerable<MInstalledApp> finalList = new List<MInstalledApp>();

            string registry_key_32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string registry_key_64 = @"SOFTWARE\WoW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            List<MInstalledApp> win32AppsCU = GetInstalledApplication(Registry.CurrentUser, registry_key_32);
            List<MInstalledApp> win32AppsLM = GetInstalledApplication(Registry.LocalMachine, registry_key_32);
            List<MInstalledApp> win64AppsCU = GetInstalledApplication(Registry.CurrentUser, registry_key_64);
            List<MInstalledApp> win64AppsLM = GetInstalledApplication(Registry.LocalMachine, registry_key_64);

            finalList = win32AppsCU.Concat(win32AppsLM).Concat(win64AppsCU).Concat(win64AppsLM);

            finalList = finalList.GroupBy(d => d.DisplayName).Select(d => d.First());

            return finalList.OrderBy(o => o.DisplayName).ToList();
        }
    }
}
