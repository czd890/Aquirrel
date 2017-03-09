using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Aquirrel.Internal
{
    public class LocalIp
    {
        static string ipv4=null;
        public static async Task<string> GetLocalIPV4()
        {
            if (ipv4 != null)
                return ipv4;

            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = await Dns.GetHostEntryAsync(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }

        }
    }
}
