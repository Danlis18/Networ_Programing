using _002_ChatScaner;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _002_ChatServer
{
    public partial class Form1 : Form
    {
        Socket listenSocket = null;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //1. Create socket

            int port = 22000;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string ipStr = "10.100.3.248";

            IPAddress ip = IPAddress.Parse(ipStr);

            //Dns.GetHostEntry("localhost");
            //IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ip = entry.AddressList[0];

            IPEndPoint endPoint = new IPEndPoint(ip, port);

            //2. Bind Socket - слухаємо на певному порту
            listenSocket.Bind(endPoint);

            Task.Run(() =>
            {
                ListenThread(listenSocket);
            });
        }

        private void ListenThread(Socket listenSocket)
        {
            //3. Listen - дивиться чи є підключення до цього порту
            listenSocket.Listen(10);
            while (true)
            {

                // block function
                Socket clientSocket = listenSocket.Accept();

                info info = new info()
                {
                    ClientSocket = clientSocket,
                    RemoteEndPoint = clientSocket.RemoteEndPoint.ToString()
                };

                listBox1.Invoke(() =>
                {
                    listBox1.Items.Add(info);
                });

                Task.Run(() =>
                {
                    ReceiveThread(clientSocket);
                });
            }
        }

        private void ReceiveThread(Socket clientSocket)
        {
            int bytes = 0;

            byte[] data = new byte[1024];

            string massage = string.Empty;

            while (true)
            {
                //4. Receive - отримуємо дані від клієнта

                bytes = clientSocket.Receive(data); //- блокуюча функція

                massage = Encoding.ASCII.GetString(data, 0, bytes);

                textBox1.Invoke(() =>
                {
                    textBox1.Text += massage;
                });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string message = textBox2.Text;
            info info = (info)listBox1.SelectedItem;

            byte[] data = Encoding.ASCII.GetBytes(message);

            info.ClientSocket.Send(data);
        }




        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
