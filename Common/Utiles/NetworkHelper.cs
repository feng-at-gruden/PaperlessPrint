using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace Common.Utiles
{
    public static class NetworkHelper
    {

        public static String GetLocalIP()
        {
            String hostname = Dns.GetHostName();
            IPAddress[] ipList = Dns.GetHostAddresses(hostname);
            foreach (IPAddress ip in ipList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

        public static String GetWifiIP()
        {
            string ipstr = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                bool type = (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);   //判断是否是无线网卡  
                if (type)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();   //IP配置信息  
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        ipstr = ip.UnicastAddresses[0].Address.ToString();  //IP地址  
                    }
                }
            }
            return ipstr;
        }


    }
}
