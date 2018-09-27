using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Aquirrel.Tools
{
    /// <summary>
    /// 获取当前机器的本机ipv4
    /// </summary>
    public class LocalIp
    {
        static string ipv4 = null;
        public static string GetLocalIPV4()
        {
            if (ipv4 != null)
                return ipv4;

            try
            {
                return ipv4 = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                     .Select(p => p.GetIPProperties())
                     .SelectMany(p => p.UnicastAddresses)
                     .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                     .FirstOrDefault()?.Address.ToString();
            }
            catch
            {
                return "";
            }

        }
    }
}
