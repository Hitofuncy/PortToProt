using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IP
{
    public class DuanKou
    {
        static public volatile bool isop = true;

        Thread myThread = null;
        Socket serverSocket = null;
        int i = 0;

        public bool getmyThreadOn(){
            return myThread.ThreadState == ThreadState.Running;
        }

        int localProt { get; set; }
        string localIp { get; set; }
        int TargetPort { get; set; }
        string TargetIp { get; set; }
        public DuanKou(string localIp, int localProt, string TargetIp, int TargetPort){
            this.localIp = localIp;
            this.localProt = localProt;
            this.TargetIp = TargetIp;
            this.TargetPort = TargetPort;

            IPAddress ip = IPAddress.Parse(localIp);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;
            serverSocket.Bind(new IPEndPoint(ip, localProt));
            serverSocket.Listen(10000);
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
        }

        public void Run() {
            myThread = new Thread(Listen);
            myThread.Start(serverSocket);
        }

        private void Listen(object obj){
            Socket serverSocket = (Socket)obj;
            IPAddress ip = IPAddress.Parse(TargetIp);
            while (isop){
                Socket tcp1 = serverSocket.Accept();
                Socket tcp2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcp2.Connect(new IPEndPoint(ip, TargetPort));

                ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock{
                    tcp1 = tcp2,
                    tcp2 = tcp1
                });

                ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock{
                    tcp1 = tcp1,
                    tcp2 = tcp2
                });
            }
        }

        public void SwapMsg(object obj){
            thSock mSocket = (thSock)obj;
            while (true) {
                try {
                    byte[] result = new byte[1024];
                    int num = mSocket.tcp2.Receive(result, result.Length, SocketFlags.None);
                    if (num == 0) {
                        if (mSocket.tcp1.Connected) mSocket.tcp1.Close();
                        if (mSocket.tcp2.Connected) mSocket.tcp2.Close();
                        
                        break;
                    }
                    mSocket.tcp1.Send(result, num, SocketFlags.None);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    if (mSocket.tcp1.Connected) mSocket.tcp1.Close();
                    if (mSocket.tcp2.Connected) mSocket.tcp2.Close();
                    
                    break;
                }
            }
        }

    }

    public class thSock{
        public Socket tcp1 { get; set; }
        public Socket tcp2 { get; set; }
    }
}
