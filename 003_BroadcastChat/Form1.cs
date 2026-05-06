using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _003_BroadcastChat
{
    public partial class Form1 : Form
    {
        UdpClient clientSend = null;
        UdpClient clientReceive = null;
        UdpClient msgSend = null;
        UdpClient msgReceive = null;

        int portSend = 47023;
        int portReceive = 47025;
        int msgPortSend = 47020;
        int sgPortReceive = 47021;

        string ip = "192.168.43.67";

        public Form1()
        {
            InitializeComponent();

            IPAddress localIP = IPAddress.Parse(ip);
            clientSend = new UdpClient(new IPEndPoint(localIP, portSend));
            clientReceive = new UdpClient(new IPEndPoint(localIP, portReceive));

            msgSend = new UdpClient(new IPEndPoint(localIP, msgPortSend));
            msgReceive = new UdpClient(new IPEndPoint(localIP, sgPortReceive));


            this.Text = "BROADCAST";
            this.BackColor = Color.FromArgb(30, 31, 34);
            this.Size = new Size(950, 560);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 260;
            sidebar.BackColor = Color.FromArgb(20, 21, 24);
            this.Controls.Add(sidebar);

            listBox1.Parent = sidebar;
            listBox1.Location = new Point(25, 145);
            listBox1.Size = new Size(210, 340);
            listBox1.BackColor = Color.FromArgb(35, 36, 40);
            listBox1.ForeColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Segoe UI", 10);

            Panel main = new Panel();
            main.Dock = DockStyle.Fill;
            main.BackColor = Color.FromArgb(43, 45, 49);
            this.Controls.Add(main);
            main.BringToFront();

            Label chatTitle = new Label();
            chatTitle.Text = "Messages";
            chatTitle.ForeColor = Color.White;
            chatTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            chatTitle.Location = new Point(30, 25);
            chatTitle.AutoSize = true;
            main.Controls.Add(chatTitle);

            textBox1.Parent = main;
            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.Location = new Point(30, 75);
            textBox1.Size = new Size(600, 330);
            textBox1.BackColor = Color.FromArgb(64, 68, 75);
            textBox1.ForeColor = Color.White;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 11);
            textBox1.ScrollBars = ScrollBars.Vertical;

            textBox2.Parent = main;
            textBox2.Location = new Point(30, 425);
            textBox2.Size = new Size(480, 35);
            textBox2.BackColor = Color.FromArgb(64, 68, 75);
            textBox2.ForeColor = Color.White;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 11);

            Button sendBtn = new Button();
            sendBtn.Text = "SEND";
            sendBtn.Size = new Size(110, 35);
            sendBtn.Location = new Point(520, 425);
            sendBtn.BackColor = Color.FromArgb(88, 101, 242);
            sendBtn.ForeColor = Color.White;
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.FlatAppearance.BorderSize = 0;
            sendBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendBtn.Click += button2_Click;
            main.Controls.Add(sendBtn);

            //Get online computers in local network
            Task.Run(() =>
            {
                while (true)
                {
                    //IPAddress.Any = 0.0.0.0, приймає пакети з будь-якої мережевої інтерфейсу
                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portReceive);

                    byte[] data = clientReceive.Receive(ref iPEndPoint);

                    Myessage message = Serialazer.ByteArrayToObject<Myessage>(data);

                    listBox1.Invoke(() =>
                    {
                        if (!listBox1.Items.Contains(message.ComputerName))
                        {
                            listBox1.Items.Add(message.ComputerName);
                        }
                        if (!message.IsOnline)
                        {
                            listBox1.Items.Remove(message.ComputerName);
                        }
                    });
                }
            });


            //Get messages from other computers in local network    
            Task.Run(() =>
            {
                while (true)
                {
                    //IPAddress.Any = 0.0.0.0, приймає пакети з будь-якої мережевої інтерфейсу
                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, sgPortReceive);

                    byte[] data = msgReceive.Receive(ref iPEndPoint);

                    Myessage message = Serialazer.ByteArrayToObject<Myessage>(data);

                    textBox1.Invoke(() =>
                    {
                        textBox1.Text += $"{message.ComputerName}: {message.Message}{Environment.NewLine}";
                    });
                }
            });
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a message to send.", "Empty Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                Message = textBox2.Text
            };

            byte[] data = Serialazer.ObjectToByteArray(myMessage);

            try
            {
                msgSend.Connect(IPAddress.Broadcast, sgPortReceive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }

            msgSend.Send(data, data.Length);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void timer1_tick(object sender, EventArgs e)
        {

            /*Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                IsOnline = true,
                Message = textBox2.Text
            };

            byte[] data = Serialazer.ObjectToByteArray(myMessage);

            //255.255.255.255 - це спеціальна адреса, яка використовується для відправки пакетів на всі пристрої в локальній мережі (broadcast)

            try
            {
                clientSend.Connect(IPAddress.Broadcast, portReceive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }

            clientSend.Send(data, data.Length);*/

            GeneralCheker(sender, true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                IsOnline = false,
                Message = textBox2.Text
            };

            byte[] data = Serialazer.ObjectToByteArray(myMessage);

            try
            {
                clientSend.Connect(IPAddress.Broadcast, portReceive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }

            clientSend.Send(data, data.Length);*/

            GeneralCheker(sender, false);
        }

        private void GeneralCheker(object sender, bool IsOnline)
        {
            Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                IsOnline = IsOnline,
                Message = textBox2.Text
            };

            byte[] data = Serialazer.ObjectToByteArray(myMessage);

            try
            {
                clientSend.Connect(IPAddress.Broadcast, portReceive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }

            clientSend.Send(data, data.Length);
        }
    }
}
