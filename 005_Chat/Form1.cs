using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace _005_Chat
{
    public partial class Form1 : Form
    {
        UdpClient clientSend = null;
        UdpClient clientReceive = null;
        UdpClient clientMessageSend = null;
        UdpClient clientMessageReceive = null;

        int portSend = 47023;
        int portReceive = 47025;
        int portMessageSend = 47020;
        int portMessageReceive = 47021;

        int portTcpPrivate = 47031;
        TcpListener tcpListener = null;
        Dictionary<string, IPEndPoint> users = 
            new Dictionary<string, IPEndPoint>();



        public Form1()
        {
            InitializeComponent();

            string ipStr = "192.168.43.67";
            IPAddress iPAddress = IPAddress.Parse(ipStr);

            clientSend = new UdpClient(new IPEndPoint(iPAddress, portSend));
            clientReceive = new UdpClient(new IPEndPoint(iPAddress, portReceive));

            clientMessageSend = new UdpClient(new IPEndPoint(iPAddress, portMessageSend));
            clientMessageReceive = new UdpClient(new IPEndPoint(iPAddress, portMessageReceive));


            tcpListener = new TcpListener(IPAddress.Any, portTcpPrivate);
            tcpListener.Start();

            Task.Run(() => TcpAcceptLoop());
           


            timer1.Start();


            // get user names
            Task.Run(() =>
            {
                while (true)
                {

                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portReceive);
                    byte[] data = clientReceive.Receive(ref iPEndPoint);

                    //  string message = Encoding.ASCII.GetString(data, 0, data.Length);

                    MyMessage myMessage = Serializer.ByteArrayToObject<MyMessage>(data);


                    listBox1.Invoke(() =>
                    {
                        if (!listBox1.Items.Contains(myMessage.ComputerName))
                            listBox1.Items.Add(myMessage.ComputerName);
                    });

                    if (!myMessage.IsOnline)
                    {

                        listBox1.Invoke(() =>
                        {

                            listBox1.Items.Remove(myMessage.ComputerName);
                        });
                    }

                    if(myMessage.IsOnline)
                    {
                        users[myMessage.ComputerName] = new IPEndPoint(iPEndPoint.Address, portTcpPrivate);
                    }
                    else
                    {
                        users.Remove(myMessage.ComputerName);
                    }

                    // Vitalic-PC   (IPEndpoint)
                    // PC2   (IPEndpoint)



                }


            });


            // get user messages
            Task.Run(() =>
            {
                while (true)
                {

                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portMessageReceive);
                    byte[] data = clientMessageReceive.Receive(ref iPEndPoint);

                    MyMessage myMessage = Serializer.ByteArrayToObject<MyMessage>(data);

                    // string message = Encoding.ASCII.GetString(data, 0, data.Length);

                    textBox1.Invoke(() =>
                    {

                        textBox1.Text += $"{myMessage.ComputerName}: {myMessage.Message}{Environment.NewLine}";
                    });

                }


            });



        }

        private async void TcpAcceptLoop()
        {
            TcpClient tcpClient = null;

            while(true)
            {
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                Task.Run(() => TcpHandleClientAsync(tcpClient));
            }


        }

        private async void TcpHandleClientAsync(TcpClient tcpClient)
        {

            using var ns = tcpClient.GetStream();
            using var sr = new StreamReader(ns, Encoding.UTF8);

            string data;
            while((data = await sr.ReadLineAsync()) != null)
            {
                MyMessage message = JsonSerializer.Deserialize<MyMessage>(data);
                textBox3.Invoke(() =>
                {
                    textBox3.Text += $"(private) {message.ComputerName} : {message.Message}{Environment.NewLine}";
                });
            }

        }

        static string getInternetConnectionIP()
        {
            using (Process route = new Process())
            {
                ProcessStartInfo startInfo = route.StartInfo;
                startInfo.FileName = "route.exe";
                startInfo.Arguments = "print 0.0.0.0";
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                route.Start();

                using (StreamReader reader = route.StandardOutput)
                {
                    string line;
                    do
                    {
                        line = reader.ReadLine();
                    } while (!line.StartsWith("          0.0.0.0"));

                    // the interface is the fourth entry in the line
                    return line.Split(new char[] { ' ' },
                              StringSplitOptions.RemoveEmptyEntries)[3];
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string computerName = SystemInformation.ComputerName;

            MyMessage myMessage = new MyMessage
            {
                ComputerName = computerName,
                IsOnline = true,
            };

            // byte[] data = Encoding.ASCII.GetBytes(computerName);

            byte[] data = Serializer.ObjectByteToArray(myMessage);

            try
            {
                clientSend.Connect(IPAddress.Broadcast, portReceive);
            }
            catch (Exception ex) { }
            clientSend.Send(data, data.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox2.Text;

            MyMessage myMessage = new MyMessage()
            {
                ComputerName = SystemInformation.ComputerName,
                Message = message,
                IsOnline = true
            };

            byte[] data = Serializer.ObjectByteToArray(myMessage);

            // byte[] data = Encoding.ASCII.GetBytes(message);

            try
            {
                clientMessageSend.Connect(IPAddress.Broadcast, portMessageReceive);
            }
            catch (Exception ex) { }
            clientMessageSend.Send(data, data.Length);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();

            MyMessage myMessage = new MyMessage()
            {
                IsOnline = false,
                ComputerName = SystemInformation.ComputerName
            };

            byte[] data = Serializer.ObjectByteToArray(myMessage);

            try
            {
                clientSend.Connect(IPAddress.Broadcast, portReceive);

            }
            catch (Exception ex) { }
            clientSend.Send(data, data.Length);

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem == null)
            {
                MessageBox.Show("Select User");
                return;
            }

            string targetName = listBox1.SelectedItem.ToString();

            IPEndPoint targetEp = null;

            if(!users.TryGetValue(targetName, out targetEp))
            {
                MessageBox.Show("User IP is wrong");
                return;
            }

            MyMessage myMessage = new MyMessage()
            {
                IsOnline = true,
                ComputerName = SystemInformation.ComputerName,
                Message = textBox4.Text
            };

            // ... Check Self



            try
            {
                await SendTcpMessageAsync(targetEp, myMessage);
            }
            catch (Exception ex) {
                MessageBox.Show("Error send");
            }


        }

        private async Task SendTcpMessageAsync(IPEndPoint targetEp, MyMessage myMessage)
        {
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(targetEp.Address, targetEp.Port);

            using var ns = tcpClient.GetStream();

            using StreamWriter sw = new StreamWriter(ns, Encoding.UTF8);

            string data = JsonSerializer.Serialize(myMessage);

            await sw.WriteAsync(data);
        }
    }
}
