using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _003_BroadcastChat
{
    public partial class Form1 : Form
    {
        UdpClient clientSend = null;
        UdpClient clientReceive = null;
        UdpClient msgSend = null;
        UdpClient msgReceive = null;
        List<Myessage> rollResults = new List<Myessage>();

        Button rollBtn = new Button();

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
            textBox1.AutoScrollOffset = new Point(0, textBox1.Height);

            textBox2.Parent = main;
            textBox2.Location = new Point(30, 425);
            textBox2.Size = new Size(480, 35);
            textBox2.BackColor = Color.FromArgb(64, 68, 75);
            textBox2.ForeColor = Color.White;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 11);

            Button strBtn = new Button();
            strBtn.Text = "Start Round";
            strBtn.Size = new Size(110, 35);
            strBtn.Location = new Point(520, 425);
            strBtn.BackColor = Color.FromArgb(88, 101, 242);
            strBtn.ForeColor = Color.White;
            strBtn.FlatStyle = FlatStyle.Flat;
            strBtn.FlatAppearance.BorderSize = 0;
            strBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            strBtn.Click += button2_Click;
            main.Controls.Add(strBtn);

            rollBtn.Text = "ROLL";
            rollBtn.Size = new Size(210, 40); 
            rollBtn.Location = new Point(25, 30);
            rollBtn.BackColor = Color.FromArgb(88, 101, 242);
            rollBtn.ForeColor = Color.White;
            rollBtn.FlatStyle = FlatStyle.Flat;
            rollBtn.FlatAppearance.BorderSize = 0;
            rollBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            rollBtn.Click += button3_Click;
            sidebar.Controls.Add(rollBtn);
            rollBtn.Enabled = false;

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


                    if (message.Message == "Round Started")
                    {
                        Invoke(() =>
                        {
                            textBox1.Text += $"--- {message.ComputerName} started a new round ---{Environment.NewLine}";
                            strBtn.Enabled = false;
                            rollBtn.Enabled = true;
                        });
                    }

                    if (message.Message == "Round Stopped")
                    {
                        Invoke(() =>
                        {
                            textBox1.Text += $"--- {message.ComputerName} stopped the round ---{Environment.NewLine}";
                            strBtn.Enabled = true;
                            rollBtn.Enabled = false;
                        });
                    }
                    if (int.TryParse(message.Message, out int rollResult))
                    {
                        //Invoke(() =>
                        //{
                        //    textBox1.Text += $"--- {message.ComputerName} rolled a {rollResult} ---{Environment.NewLine}";
                        //});

                        rollResults.Add(message);
                    }
                }
            }); 
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            rollBtn.Enabled = false;
            int rollResult = randomRoll();

            GeneralMessageSend(sender, rollResult.ToString());
        }

        //Start round
        private void button2_Click(object? sender, EventArgs e)
        {
            timer2.Start();
            textBox1.Clear();

            GeneralMessageSend(sender, "Round Started");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private int randomRoll()
        {
            Random random = new Random();
            int rollResult = random.Next(1, 7);

            return rollResult;
        }


        private void timer1_tick(object sender, EventArgs e)
        {
            GeneralSend(sender, true, "");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GeneralSend(sender, false, "");
        }

        private void GeneralSend(object sender, bool IsOnline, string message)
        {
            Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                IsOnline = IsOnline,
                Message = message
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

        private void GeneralMessageSend(object sender, string message)
        {
            Myessage myMessage = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                Message = message
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

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();

            GeneralMessageSend(sender, "Round Stopped");

            textBox1.Clear();

            string s = rollResults.Average(r => int.Parse(r.Message)).ToString();
            string sMax = rollResults.Max(r => int.Parse(r.Message)).ToString();

            GeneralMessageSend("Roller: ", "Average - " + s + ", Win - " + sMax);


            string roundResultsText = "";

            int rollr = 0;

            rollResults.OrderByDescending(r => int.Parse(r.Message)).ToList().ForEach(r =>
            {
                roundResultsText += $"--- {r.ComputerName} rolled a {r.Message} ---{Environment.NewLine}";
                rollr = Convert.ToInt32(r.Message);
            });



            Myessage result = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName,
                Message = "Result: " + Environment.NewLine + roundResultsText
            };

            byte[] data = Serialazer.ObjectToByteArray(result);

            try
            {
                msgSend.Connect(IPAddress.Broadcast, sgPortReceive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send error");
            }

            msgSend.Send(data, data.Length);

            Thread.Sleep(4000);

            textBox1.Clear();

            result = new Myessage()
            {
                ComputerName = SystemInformation.ComputerName
            };

            rollResults.Where(r => r.ComputerName == result.ComputerName).First().count += rollr;

            MessageBox.Show($"{rollResults.Where(r => r.ComputerName == result.ComputerName).First().count}");

            Thread.Sleep(4000);

            textBox1.Clear();
        }
    }
}
