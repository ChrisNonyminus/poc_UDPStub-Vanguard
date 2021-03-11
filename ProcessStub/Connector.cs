using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTCV.Common;
using RTCV.NetCore;
using RTCV.UI;
//thx https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
namespace UDPStub
{
    public class UDPSocket
    {
        public Socket _socket;
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        public EndPoint epTo;
        private AsyncCallback recv = null;
        private AsyncCallback snd = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }
        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            //_socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            //{
            //    State so = (State)ar.AsyncState;
            //    int bytes = _socket.EndSend(ar);
            //    Console.WriteLine("SEND: {0}, {1}", bytes, text);
            //}, state);
            int n = _socket.SendTo(data, 0, data.Length, SocketFlags.None, this.epTo);
            Console.WriteLine("SENT: \"{0} , {1}\" to {2}", n, text, epTo.ToString());
        }
        public String Receive()
        {
            String returnvalue = "";
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
                returnvalue = Encoding.ASCII.GetString(so.buffer, 0, bytes);
            }, state);
            return returnvalue;

        }
    }
    public class Connector
    {
        public static UDPSocket c;
        public static UDPSocket s;
        public static void InitializeConnector()
        {
            s = new UDPSocket();
            s.Server(S.GET<StubForm>().localIP, 42042);
            c = new UDPSocket();
            c.Client(S.GET<StubForm>().tbClientAddr.Text, 42042);
            c.epTo = new IPEndPoint(IPAddress.Parse(S.GET<StubForm>().tbClientAddr.Text), 42042);
            Console.WriteLine("Connected to {0}", c.epTo.ToString());
            c.Send("Windows: Howdy!");
        }
        public static void SendMessage(String text)
        {
            c.Send(text);
        }
        public static String RecMessage()
        {
            return c.Receive();
        }
    }
}
