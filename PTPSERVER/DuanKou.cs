using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTPSERVER
{
    public class DuanKou
    {
        private volatile bool isop = true;

        SynchronizationContext syncContext = null;
        Thread myThread = null;
        Socket serverSocket = null;
        public bool issucc { get; } = true;

        public bool getmyThreadOn()
        {
            if (myThread == null) return false;
            return myThread.ThreadState == ThreadState.Running;
        }

        int localProt { get; set; }
        string localIp { get; set; }
        int TargetPort { get; set; }
        string TargetIp { get; set; }
        public DuanKou(string localIp, string localProt, string TargetIp, string TargetPort)
        {
            try
            {
                syncContext = SynchronizationContext.Current;
                this.localIp = localIp;
                this.localProt = int.Parse(localProt);
                this.TargetIp = TargetIp;
                this.TargetPort = int.Parse(TargetPort);
            }
            catch
            {
                issucc = false;
            }
        }

        public void Run()
        {
            isop = true;
            try
            {
                myThread = new Thread(Listen);
                myThread.Start(serverSocket);
            }
            catch
            {
                isop = false; ;
            }

        }

        private void Listen(object obj)
        {
            Socket serverSocket = (Socket)obj;
            IPAddress ip = IPAddress.Parse(TargetIp);
            while (true)
            {
                Socket tcp1 = null;
                try
                {
                    tcp1 = serverSocket.Accept();
                }
                catch
                {
                    return;
                }

                Socket tcp2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    tcp2.Connect(new IPEndPoint(ip, TargetPort));
                }
                catch
                {

                }


                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                    {
                        tcp1 = tcp2,
                        tcp2 = tcp1
                    });

                    ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                    {
                        tcp1 = tcp1,
                        tcp2 = tcp2
                    });
                    if (isop == false)
                    {
                        throw new NotSupportedException("");
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        public void SwapMsg(object obj)
        {
            thSock mSocket = (thSock)obj;
            while (true)
            {
                if (isop == false)
                {
                    if (mSocket.tcp1.Connected) mSocket.tcp1.Close();
                    if (mSocket.tcp2.Connected) mSocket.tcp2.Close();
                    break;
                }
                try
                {
                    byte[] result = new byte[1024];
                    int num = mSocket.tcp2.Receive(result, result.Length, SocketFlags.None);
                    if (num == 0)
                    {
                        if (mSocket.tcp1.Connected) mSocket.tcp1.Close();
                        if (mSocket.tcp2.Connected) mSocket.tcp2.Close();

                        break;
                    }



                    mSocket.tcp1.Send(result, num, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (mSocket.tcp1.Connected) mSocket.tcp1.Close();
                    if (mSocket.tcp2.Connected) mSocket.tcp2.Close();

                    break;
                }
            }
        }

        public bool stop()
        {
            this.isop = false;
            try
            {
                new Thread(() =>
                {
                    try
                    {
                        new TcpClient(localIp == "0.0.0.0" ? "127.0.0.1" : localIp, localProt);
                    }
                    catch
                    {

                    }

                }).Start();
            }
            catch
            {
                return true;
            }
            if (serverSocket != null)
                serverSocket.Close();
            return true;
        }
        public bool restart()
        {
            IPAddress ip = IPAddress.Parse(localIp);
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;
                serverSocket.Bind(new IPEndPoint(ip, localProt));
                serverSocket.Listen(10000);
            }
            catch
            {
                return false;
            }
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            Run();
            return true;
        }

    }

    public class thSock
    {
        public Socket tcp1 { get; set; }
        public Socket tcp2 { get; set; }
    }
}