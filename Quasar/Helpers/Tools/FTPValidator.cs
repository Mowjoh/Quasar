using FluentFTP;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quasar.Helpers.Tools
{
    static class FTPValidator
    {
        /// <summary>
        /// Validates if the provided info can lead to a successful FTP connection
        /// </summary>
        /// <returns>Connection status</returns>
        private static async Task<bool> ValidateFTPConnection()
        {
            string address = Properties.Settings.Default.FTPAddress;
            string username = Properties.Settings.Default.FTPUN;
            string password = Properties.Settings.Default.FTPPW;

            if (validateIP(address.Split(':')[0]) && validatePort(address.Split(':')[1]))
            {
                String errortext = "";
                FtpClient client = new FtpClient(address);

                if (username != "")
                {
                    client.Credentials = new NetworkCredential(username, password);
                }

                try
                {
                    await client.ConnectAsync();
                }
                catch (Exception ex)
                {
                    errortext = ex.Message;
                }


                if (client.IsConnected)
                {
                    Properties.Settings.Default.FTPValid = true;
                    Properties.Settings.Default.Save();
                }
            }

            return true;
        }

        /// <summary>
        /// Validates an IP with regex
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <returns>Match state</returns>
        private static bool validateIP(string IPAddress)
        {
            Regex IP = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

            return IP.IsMatch(IPAddress);
        }

        /// <summary>
        /// Validates a Port value
        /// </summary>
        /// <param name="Port"></param>
        /// <returns>Match state</returns>
        private static bool validatePort(string Port)
        {
            int val;

            bool result = Int32.TryParse(Port, out val);

            if (result)
            {
                result = val > 0 && val < 70000;
            }

            return result;
        }

    }
}
