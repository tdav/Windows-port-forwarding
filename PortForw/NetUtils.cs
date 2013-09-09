using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Net.Utils
{
    public class PortForward
    {
        private readonly Socket MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool LCompress = false;
        private bool RCompress = false;
        private bool IsStop = true;
        
        public void Stop()
        {
            IsStop = false;
        }

        public void Start(IPEndPoint local, bool lCompress, IPEndPoint remote, bool rCompress)
        {          
            MainSocket.Bind(local);
            MainSocket.Listen(PortForw.Properties.Settings.Default.ListenConnection);

            while (IsStop)
            {
                var source = MainSocket.Accept();
                var destination = new PortForward();
                var state = new State(source, destination.MainSocket);                
                destination.Connect(remote, source);
                source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
            }
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var state = new State(MainSocket, destination);            
            MainSocket.Connect(remoteEndpoint);
            MainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        private void OnDataReceive(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);


                    #region compress
                    //    byte[] resData=null;
                    //    byte[] souData = new byte[bytesRead];
                    //    Array.Copy(state.Buffer, souData, bytesRead);

                    //    if (!LCompress && !RCompress)
                    //    {
                    //        resData = state.Buffer;
                    //        Console.WriteLine(String.Format("{0} --> {1}", bytesRead, resData.Length));
                    //    }
                    //    else
                    //    {
                    //        if (RCompress)
                    //            resData = BrainTechLLC.MiniLZO.Compress(souData, bytesRead);

                    //        if (LCompress)
                    //            resData = BrainTechLLC.MiniLZO.Decompress(souData);
                    //    }

                    //    state.DestinationSocket.Send(resData, resData.Length, SocketFlags.None);
                    //    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);

                    //    Console.WriteLine(String.Format("{0} --> {1}", bytesRead, resData.Length));
                    #endregion
                }
            }
            catch
            {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private class State
        {
            public Socket SourceSocket { get; private set; }
            public Socket DestinationSocket { get; private set; }
            public byte[] Buffer { get; private set; }
            public State(Socket source, Socket destination)
            {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[8192];
            }
        }
    }
}
