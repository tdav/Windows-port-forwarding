using Net.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PortForw
{
    partial class pfService : ServiceBase
    {
        public pfService()
        {
            InitializeComponent();
        }

        private int wc = 0;
        private List<PortForward> PortForwardList = new List<PortForward>();
        private List<Task> taskList = new List<Task>();

        public void Start()
        {
            OnStart(null);
        }


        private Task PortForwardStart(PFItem item)
        {
            return Task.Factory.StartNew(() =>
            {
                wc++;
                Console.WriteLine(String.Format("{0} та {1}:{2} --> {3}:{4}", wc, item.LocalIp, item.LocalPort, item.RemoteIp, item.RemotePort));
                PortForward pf = new PortForward();
                pf.Start(new IPEndPoint(IPAddress.Parse(item.LocalIp), item.LocalPort), item.LocalCopmress,
                         new IPEndPoint(IPAddress.Parse(item.RemoteIp), item.RemotePort), item.RemoteCopmress);
                PortForwardList.Add(pf);
            });
        }

        protected override void OnStart(string[] args)
        {
            PFList li = PFList.Load(AppDomain.CurrentDomain.BaseDirectory + "PortList.xml");

            foreach (PFItem item in li)
            {
                taskList.Add(PortForwardStart(item));
            }
        }

        protected override void OnStop()
        {
            foreach (PortForward item in PortForwardList)
            {
                item.Stop();
            }
        }
    }
}
