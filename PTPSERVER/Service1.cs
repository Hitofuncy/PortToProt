using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTPSERVER
{
    public partial class Service1 : ServiceBase
    {
        DuanKou dk = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string tx = "";
            for (int i = 0, j = args.Length; i < j; i++)
            {
                tx += Environment.NewLine + args[i];
            }
            if (tx == "") tx = "file";
            
            if (args.Length < 4) Environment.Exit(0);
            dk = new DuanKou(args[0], int.Parse(args[1]), args[2], int.Parse(args[3]));
            dk.restart();
        }

        protected override void OnStop()
        {
            dk.stop();
        }
    }
}
