using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceProcess;

using Net.Utils;

namespace PortForw
{
    class Program
    {
        static void Main(string[] args)
        {
            var srv = new pfService();
            ServiceBase[] sb = new ServiceBase[] { srv };
            if (Environment.UserInteractive)
            {
                Console.CancelKeyPress += (x, y) => srv.Stop();
                srv.Start();

                Console.ReadKey();
                srv.Stop();
            }
            else
            {
                ServiceBase.Run(srv);
            }
        }
    }
}
//PortForw.exe 127.0.0.1 80 127.0.0.1 12345
