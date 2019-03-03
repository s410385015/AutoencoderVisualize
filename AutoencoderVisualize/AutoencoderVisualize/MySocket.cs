using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace AutoencoderVisualize
{

    public class MySocket
    {
        public Socket client;
        private Byte[] buffer = new Byte[256];
        private delegate String StrHandler(String str);
        public bool flag=false;


        public void StartClient(String ip, int port)
        {

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(ip, port);
                flag = true;
            }
            catch
            {
                flag = false;
                MessageBox.Show("連線異常！");
            }
        }
        public void Recv(AsyncCallback EndRecv)
        {
            
            try
            {
                client.BeginReceive(buffer, 0, 256, SocketFlags.None, EndRecv, null);
            }
            catch
            {

            }
        }

        /*
        private void EndRecv(IAsyncResult r)
        {

            try
            {
                int len = client.EndReceive(r);
                String temp = Encoding.UTF8.GetString(buffer, 0, len);
                MessageBox.Show(temp);
               
            }
            catch
            {

            }

        }
        */

        public void Send(String str)
        {
            Byte[] cmd = Encoding.UTF8.GetBytes(str);
            try
            {
                MessageBox.Show("send");
                client.BeginSend(cmd, 0, cmd.Length, SocketFlags.None, EndSend, client);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void EndSend(IAsyncResult r)
        {
            try
            {
                client.EndSend(r);
           
            }
            catch
            {

            }
        }
    }
}
