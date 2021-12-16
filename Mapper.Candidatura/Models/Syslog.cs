using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mapper.Candidatura.Authentication
{
    public enum Level
    {
        Emergency = 0,
        Alert = 1,
        Critical = 2,
        Error = 3,
        Warning = 4,
        Notice = 5,
        Information = 6,
        Debug = 7,
    }

    public enum Facility
    {
        Kernel = 0,
        User = 1,
        Mail = 2,
        Daemon = 3,
        Auth = 4,
        Syslog = 5,
        Lpr = 6,
        News = 7,
        UUCP = 8,
        Cron = 9,
        Local0 = 10,
        Local1 = 11,
        Local2 = 12,
        Local3 = 13,
        Local4 = 14,
        Local5 = 15,
        Local6 = 16,
        Local7 = 17,
    }

    public class Message
    {
        private int _facility;
        public int Facility
        {
            get { return _facility; }
            set { _facility = value; }
        }
        private int _level;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public Message() { }
        public Message(int facility, int level, string text)
        {
            _facility = facility;
            _level = level;
            _text = text;
        }
    }

    public class UdpClientEx : System.Net.Sockets.UdpClient
    {
        public UdpClientEx() : base() { }
        public UdpClientEx(IPEndPoint ipe) : base(ipe) { }
        ~UdpClientEx()
        {
            if (this.Active) this.Close();
        }

        public bool IsActive
        {
            get { return this.Active; }
        }
    }


    public class Client
    {
        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint ipLocalEndPoint;
        private UdpClientEx udpClient;

        private string _sysLogServerIp = null;
        private int _port = 516;

        public Client()
        {
            ipHostInfo = Dns.GetHostByName(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            ipLocalEndPoint = new IPEndPoint(ipAddress, 0);
            udpClient = new UdpClientEx(ipLocalEndPoint);
        }

        public bool IsActive
        {
            get { return udpClient.IsActive; }
        }

        public void Close()
        {
            if (udpClient.IsActive) udpClient.Close();
        }

        public int Port
        {
            set { _port = value; }
            get { return _port; }
        }

        public string SysLogServerIp
        {
            get { return _sysLogServerIp; }
            set
            {
                if ((_sysLogServerIp == null) && (!IsActive))
                {
                    _sysLogServerIp = value;
                }
            }
        }

        public void Send(Message message)
        {
            if (!udpClient.IsActive)
                udpClient.Connect(_sysLogServerIp, _port);
            if (udpClient.IsActive)
            {
                CultureInfo cEng = new CultureInfo("en-GB");

                int priority = message.Facility * 8 + message.Level;
                string msg = System.String.Format("<{0}>{1} {2} {3}",
                    priority,
                    DateTime.Now.ToString("MMM dd HH:mm:ss", cEng),
                    ipLocalEndPoint.Address,
                    message.Text);
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
                udpClient.Send(bytes, bytes.Length);
            }
            else throw new Exception("Syslog client Socket is not connected. Please set the SysLogServerIp property");
        }

    }

    public class SyslogEvent
    {
        public static void WriteLog(string message, string method)
        {
            Client c = new Client();
            try
            {
                string ipRemoteAddress = HttpContext.Current.Request.ServerVariables[System.Configuration.ConfigurationSettings.AppSettings["RemoteIP"]];
                message = string.Format("Bando Social Housig - {0} - IP({1}) {2}", method, ipRemoteAddress, message);
                c.Port = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["SyslogPort"]);
                c.SysLogServerIp = System.Configuration.ConfigurationSettings.AppSettings["SyslogIP"];
                int facility = (int)Facility.User; // Local5
                int level = (int)Level.Information;  // Information;

                c.Send(new Message(facility, level, message));
            }
            catch (System.Exception ex1)
            {
#if (!DEBUG)
                                   RER.Tools.ApplicationLogger.WebApplicationLogger.LogEvent("Bando Social Housig - SysLog", HttpContext.Current.ApplicationInstance, ex1);
#endif
            }
            finally
            {
                c.Close();
            }
        }
    }
}




