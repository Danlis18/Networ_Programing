using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _002_ChatClient
{
    public partial class Form1 : Form
    {
        Socket sendSocket = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //1. Create socket
            sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            int port = 22000;
            string ipStr = textBox1.Text;
            //ipStr += port.ToString();

            IPAddress ip = IPAddress.Parse(ipStr);

            IPEndPoint endPoint = new IPEndPoint(ip, port);

            //2. Connect to server
            sendSocket.Connect(endPoint);

            //4. Receive data from server
            Task.Run(() =>
            {
                ReceiveData(sendSocket);
            });

            button2.Enabled = false;
            button2.Text = "Connected";
            button2.BackColor = Color.Green;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //3. Send data to server
            string message = textBox2.Text;
            byte[] data = Encoding.ASCII.GetBytes(message);
            sendSocket.Send(data);
        }

        private void ReceiveData(Socket receiveSocket)
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int bytesRead = receiveSocket.Receive(data);

                string message = Encoding.ASCII.GetString(data, 0, bytesRead);

                textBox3.Invoke(() =>
                {
                    textBox3.Text += message;
                });
            }
        }

        
    }
}
