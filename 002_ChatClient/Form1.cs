using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _002_ChatClient
{
    public partial class Form1 : Form
    {
        Socket sendSocket = null;
        Button connectBtn = new Button();
        Button disconnectBtn = new Button();
        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;

            this.Text = "CLIENT";
            this.Size = new Size(850, 520);
            this.BackColor = Color.FromArgb(30, 31, 34);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            button1.Visible = false;
            button2.Visible = false;

            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 260;
            sidebar.BackColor = Color.FromArgb(20, 21, 24);
            this.Controls.Add(sidebar);

            Label title = new Label();
            title.Text = "Chat Client";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            title.Location = new Point(25, 25);
            title.AutoSize = true;
            sidebar.Controls.Add(title);

            Label ipLabel = new Label();
            ipLabel.Text = "Server IP";
            ipLabel.ForeColor = Color.LightGray;
            ipLabel.Font = new Font("Segoe UI", 10);
            ipLabel.Location = new Point(25, 80);
            ipLabel.AutoSize = true;
            sidebar.Controls.Add(ipLabel);

            textBox1.Parent = sidebar;
            textBox1.Location = new Point(25, 110);
            textBox1.Size = new Size(210, 30);
            textBox1.BackColor = Color.FromArgb(35, 36, 40);
            textBox1.ForeColor = Color.White;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 11);


            connectBtn.Text = "CONNECT";
            connectBtn.Size = new Size(210, 45);
            connectBtn.Location = new Point(25, 160);
            connectBtn.BackColor = Color.FromArgb(88, 101, 242);
            connectBtn.ForeColor = Color.White;
            connectBtn.FlatStyle = FlatStyle.Flat;
            connectBtn.FlatAppearance.BorderSize = 0;
            connectBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            connectBtn.Click += button2_Click;
            sidebar.Controls.Add(connectBtn);

            disconnectBtn.Text = "DISCONNECT";
            disconnectBtn.Size = new Size(210, 45);
            disconnectBtn.Location = new Point(25, 220);
            disconnectBtn.ForeColor = Color.White;
            disconnectBtn.FlatStyle = FlatStyle.Flat;
            disconnectBtn.FlatAppearance.BorderSize = 0;
            disconnectBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            disconnectBtn.Click += button3_Click;
            sidebar.Controls.Add(disconnectBtn);

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

            textBox3.Parent = main;
            textBox3.Multiline = true;
            textBox3.ReadOnly = true;
            textBox3.Location = new Point(30, 75);
            textBox3.Size = new Size(500, 300);
            textBox3.BackColor = Color.FromArgb(64, 68, 75);
            textBox3.ForeColor = Color.White;
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Segoe UI", 11);

            textBox2.Parent = main;
            textBox2.Location = new Point(30, 395);
            textBox2.Size = new Size(380, 35);
            textBox2.BackColor = Color.FromArgb(64, 68, 75);
            textBox2.ForeColor = Color.White;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 11);

            Button sendBtn = new Button();
            sendBtn.Text = "SEND";
            sendBtn.Size = new Size(110, 35);
            sendBtn.Location = new Point(420, 395);
            sendBtn.BackColor = Color.FromArgb(88, 101, 242);
            sendBtn.ForeColor = Color.White;
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.FlatAppearance.BorderSize = 0;
            sendBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendBtn.Click += button1_Click;
            main.Controls.Add(sendBtn);

            Label nickLabel = new Label();
            nickLabel.Text = "Nickname";
            nickLabel.ForeColor = Color.LightGray;
            nickLabel.Font = new Font("Segoe UI", 10);
            nickLabel.Location = new Point(25, 290);
            nickLabel.AutoSize = true;
            sidebar.Controls.Add(nickLabel);

            TextBox nickBox = new TextBox();
            nickBox.Name = "nickBox";
            nickBox.Location = new Point(25, 320);
            nickBox.Size = new Size(210, 30);
            nickBox.BackColor = Color.FromArgb(35, 36, 40);
            nickBox.ForeColor = Color.White;
            nickBox.BorderStyle = BorderStyle.None;
            nickBox.Font = new Font("Segoe UI", 11);
            sidebar.Controls.Add(nickBox);

            disconnectBtn.Enabled = false;
            disconnectBtn.Text = "DISCONNECT";
            disconnectBtn.BackColor = Color.Gray;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (sendSocket != null && sendSocket.Connected)
            {
                sendSocket.Shutdown(SocketShutdown.Both);
                sendSocket.Close();
            }
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            disconnectBtn.Enabled = false;
            disconnectBtn.Text = "DISCONNECT";
            connectBtn.BackColor = Color.Gray;

            if (sendSocket != null)
            {
                sendSocket.Shutdown(SocketShutdown.Both);
                sendSocket.Close();
                sendSocket = null;
            }

            textBox3.AppendText("🔌 You disconnected\r\n");

            connectBtn.Enabled = true;
            connectBtn.Text = "CONNECT";
            connectBtn.BackColor = Color.FromArgb(88, 101, 242);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            //1. Create socket
            sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string temp = textBox1.Text;

            if (string.IsNullOrWhiteSpace(temp))
            {
                MessageBox.Show("Введи IP або IP:PORT");
                return;
            }

            string ipStr;
            int port = 22000;


            if (temp.Contains(":"))
            {
                var parts = temp.Split(':');

                ipStr = parts[0];

                if (!int.TryParse(parts[1], out port))
                {
                    MessageBox.Show("Неправильний порт");
                    return;
                }
            }
            else
            {
                ipStr = temp;
            }

            //ipStr += port.ToString();

            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            //2. Connect to server
            sendSocket.Connect(endPoint);
            textBox3.AppendText("✅ Connected to server\r\n");

            //4. Receive data from server
            Task.Run(() =>
            {
                ReceiveData(sendSocket);
            });

            connectBtn.Enabled = false;
            connectBtn.Text = "CONNECTED";
            connectBtn.BackColor = Color.Gray;

            disconnectBtn.Enabled = true;
            disconnectBtn.Text = "DISCONNECT";
            disconnectBtn.BackColor = Color.FromArgb(200, 50, 50);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sendSocket == null || !sendSocket.Connected)
            {
                MessageBox.Show("Спочатку підключись до сервера.");
                return;
            }

            TextBox nickBox = this.Controls.Find("nickBox", true).FirstOrDefault() as TextBox;

            string nick = nickBox.Text;
            string message = textBox2.Text;

            if (string.IsNullOrWhiteSpace(nick))
            {
                nick = "Unknown";
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Sms sms = new Sms()
            {
                Nickname = nick,
                Message = message,
                Time = DateTime.Now
            };

            //3. Send data to server
            string json = System.Text.Json.JsonSerializer.Serialize(sms);
            byte[] data = Encoding.ASCII.GetBytes(json);

            sendSocket.Send(data);

            textBox3.AppendText($"[{sms.Time:HH:mm}] You: {sms.Message}\r\n");
            textBox2.Clear();
        }

        private void ReceiveData(Socket receiveSocket)
        {
            byte[] data = new byte[1024];

            while (true)
            {
                int bytesRead = receiveSocket.Receive(data);

                if (bytesRead == 0)
                {
                    break;
                }

                string json = Encoding.ASCII.GetString(data, 0, bytesRead);

                Sms sms = System.Text.Json.JsonSerializer.Deserialize<Sms>(json);

                textBox3.Invoke(() =>
                {
                    textBox3.AppendText($"[{sms.Time:HH:mm}] {sms.Nickname}: {sms.Message}\r\n");
                });

                if (!receiveSocket.Connected)
                {
                    {
                        textBox3.Invoke(() =>
                        {
                            textBox3.AppendText("🔌 Server disconnected.\r\n");
                        });

                        connectBtn.Enabled = true;
                        connectBtn.Text = "CONNECT";
                        connectBtn.BackColor = Color.FromArgb(88, 101, 242);

                        receiveSocket.Close();
                        break;
                    }
                }
            }


        }
    }
}
