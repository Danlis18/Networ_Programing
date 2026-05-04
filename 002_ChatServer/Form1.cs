using _002_ChatScaner;
using _003_SetrverClient;
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

            button1.Visible = false;
            button2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;

            this.Text = "SERVER";
            this.Size = new Size(950, 560);
            this.BackColor = Color.FromArgb(30, 31, 34);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ліва панель
            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 260;
            sidebar.BackColor = Color.FromArgb(20, 21, 24);
            this.Controls.Add(sidebar);

            Label title = new Label();
            title.Text = "Chat Server";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            title.Location = new Point(25, 25);
            title.AutoSize = true;
            sidebar.Controls.Add(title);

            Button startBtn = new Button();
            startBtn.Text = "START SERVER";
            startBtn.Size = new Size(210, 45);
            startBtn.Location = new Point(25, 75);
            startBtn.BackColor = Color.FromArgb(88, 101, 242);
            startBtn.ForeColor = Color.White;
            startBtn.FlatStyle = FlatStyle.Flat;
            startBtn.FlatAppearance.BorderSize = 0;
            startBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            startBtn.Click += button1_Click;
            sidebar.Controls.Add(startBtn);

            listBox1.Parent = sidebar;
            listBox1.Location = new Point(25, 145);
            listBox1.Size = new Size(210, 340);
            listBox1.BackColor = Color.FromArgb(35, 36, 40);
            listBox1.ForeColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Segoe UI", 10);

            // чат-панель
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int port = 22000;
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                string ipStr = "192.168.56.1";
                IPAddress ip = IPAddress.Parse(ipStr);

                IPEndPoint endPoint = new IPEndPoint(ip, port);

                listenSocket.Bind(endPoint);
                listenSocket.Listen(10);

                textBox1.AppendText("✅ Server started...\r\n");

                Task.Run(() =>
                {
                    ListenThread();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server error");
            }
        }

        private void ListenThread()
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = listenSocket.Accept();

                    info clientInfo = new info()
                    {
                        ClientSocket = clientSocket,
                        RemoteEndPoint = clientSocket.RemoteEndPoint.ToString()
                    };

                    listBox1.Invoke(() =>
                    {
                        listBox1.Items.Add(clientInfo);
                    });

                    textBox1.Invoke(() =>
                    {
                        textBox1.AppendText($"🟢 Client connected: {clientInfo.RemoteEndPoint}\r\n");
                    });

                    Task.Run(() =>
                    {
                        ReceiveThread(clientSocket, clientInfo);
                    });
                }
                catch
                {
                    break;
                }
            }
        }
        private void ReceiveThread(Socket clientSocket, info clientInfo)
        {
            byte[] data = new byte[1024];

            while (true)
            {
                try
                {
                    int bytes = clientSocket.Receive(data);

                    if (bytes == 0)
                    {
                        break;
                    }

                    string json = Encoding.ASCII.GetString(data, 0, bytes);

                    Sms sms = System.Text.Json.JsonSerializer.Deserialize<Sms>(json);

                    textBox1.Invoke(() =>
                    {
                        textBox1.AppendText($"[{sms.Time:HH:mm}] {sms.Nickname}: {sms.Message}\r\n");
                    });
                }
                catch
                {
                    textBox1.Invoke(() =>
                    {
                        textBox1.AppendText("🔴 Client disconnected\r\n");
                    });

                    break;
                }
            }
            listBox1.Invoke(() =>
            {
                listBox1.Items.Remove(clientInfo);
            });

            textBox1.Invoke(() =>
            {
                textBox1.AppendText($"🔴 Client disconnected: {clientInfo.RemoteEndPoint}\r\n");
            });

            clientSocket.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItem == null)
                {
                    MessageBox.Show("Вибери клієнта зі списку.");
                    return;
                }


                string message = textBox2.Text;

                if (string.IsNullOrWhiteSpace(message))
                {
                    return;
                }

                info selectedClient = (info)listBox1.SelectedItem;

                Sms sms = new Sms()
                {
                    Nickname = "Server",
                    Message = message,
                    Time = DateTime.Now
                };

                if (message == "exit")
                {
                    selectedClient.ClientSocket.Shutdown(SocketShutdown.Both);
                    selectedClient.ClientSocket.Close();
                    return;
                }  

                string json = System.Text.Json.JsonSerializer.Serialize(sms);

                byte[] data = Encoding.ASCII.GetBytes(json);
                selectedClient.ClientSocket.Send(data);

                textBox1.AppendText($"[{sms.Time:HH:mm}] Server: {sms.Message}\r\n");
                textBox2.Clear();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }
        }




        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
