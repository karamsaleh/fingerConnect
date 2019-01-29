using System;
using zkemkeeper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proc_1
{
    class Program
    {
        static DeviceManipulator manipulator = new DeviceManipulator();
        public static ZkemClient objZkeeper;
        public static bool isDeviceConnected = false;


        public static bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    Console.WriteLine("The device is connected !!");
                }
                else
                {
                    Console.WriteLine("The device is diconnected !!");
                    objZkeeper.Disconnect();
                }
            }
        }
        static void Connect()
        {
            try
            {
                string ipAddress = "192.168.12.150";
                string port = "4370";
                if (ipAddress == string.Empty || port == string.Empty)
                    throw new Exception("The Device IP Address and Port is mandotory !!");

                int portNumber = 4370;
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, 1);
                    Console.WriteLine(deviceInfo);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        Console.WriteLine("The device is switched off");
                        break;
                    }

                default:
                    break;
            }

        }

        static void GetLogData()
        {
            try
            {
                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, 1);

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    // To Do
                    // insert into mysql db
                    foreach(MachineInfo m in lstMachineInfo)
                    {
                        Console.WriteLine(m.IndRegID  + ":" + m.DateTimeRecord );
                    }
                    //BindToGridView(lstMachineInfo);
                    //ShowStatusBar(lstMachineInfo.Count + " records found !!", true);
                }
                else
                    Console.WriteLine("No records found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void Main(string[] args)
        {
            Connect();
            Console.ReadLine();
        }
    }
}
