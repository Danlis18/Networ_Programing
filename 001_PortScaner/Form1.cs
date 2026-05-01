using System.Net;
using System.Net.Sockets;

namespace _001_PortScaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Task.Run(() =>
            {


                string ipStr = "10.100.3.248";

                IPAddress ip = IPAddress.Parse(ipStr);

                Socket socket = null;

                int start = 130, end = 140;

                for (int i = start; i < end; i++)
                {
                    try
                    {
                        IPEndPoint endPoint = new IPEndPoint(ip, i);

                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        socket.Connect(endPoint);

                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add($"Port {i} is busy");
                        }));
                    }
                    catch
                    {
                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add($"Port {i} is free");
                        }));
                    }
                    finally
                    {
                        socket?.Close();
                    }
                }
            });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
