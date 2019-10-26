using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Quasar_Sys
{
    static class Protoman
    {
        public static bool ActivateCustomProtocol()
        {
            bool Attempt = false;
            string AppPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var customProtocol = Registry.ClassesRoot.OpenSubKey("quasar\\shell\\open\\command", true);
                var customProtocolIcon = Registry.ClassesRoot.OpenSubKey("quasar\\DefaultIcon", true);

                if (customProtocol != null && customProtocolIcon != null)
                {
                    customProtocol.SetValue("", "\"" + AppPath + "\" \"%1\"", RegistryValueKind.String);
                    customProtocol.Close();


                    customProtocolIcon.SetValue("", "\"" + AppPath + "\",1", RegistryValueKind.String);
                    customProtocolIcon.Close();
                }
                else
                {
                    if (customProtocol == null)
                    {
                        customProtocol = Registry.ClassesRoot.CreateSubKey("quasar\\shell\\open\\command", true);
                        customProtocol.SetValue("", "\"" + AppPath + "\" \"%1\"", RegistryValueKind.String);
                        customProtocol.Close();
                    }
                    if (customProtocolIcon == null)
                    {
                        customProtocolIcon = Registry.ClassesRoot.CreateSubKey("quasar\\DefaultIcon", true);
                        customProtocolIcon.SetValue("", "\"" + AppPath + "\",1", RegistryValueKind.String);
                        customProtocolIcon.Close();
                    }
                    Attempt = true;
                }
            }
            return Attempt;
        }
    }
}
