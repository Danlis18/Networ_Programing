using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace _005_MultiacastServer
{
    public partial class Form1 : Form
    {
        private const string PictDirectory = "Pictures";

        private const string MulticastIp = "224.5.6.11";
        private const int SendFromPort = 7777;
        private const int ClientListenPort = 7778;
        private const int Ttl = 32;

        // якщо AUTO не працюЇ, впиши сюди св≥й IPv4, наприклад:
        // private const string ManualLocalIp = "192.168.1.30";
        private const string ManualLocalIp = "10.100.3.248";

        private const int MaxPayloadSize = 1200;
        private const int HeaderSize = 32;
        private static readonly byte[] Magic = { (byte)'I', (byte)'M', (byte)'G', (byte)'1' };

        private UdpClient client;
        private IPAddress groupAddress;
        private IPEndPoint remoteEndPoint;

        private FileInfo[] fi;

        public Form1()
        {
            InitializeComponent();

            try
            {
                InitMulticast();
                LoadPictures();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Server multicast error");
            }
        }

        private void InitMulticast()
        {
            IPAddress localAddress = GetLocalIPv4();

            groupAddress = IPAddress.Parse(MulticastIp);
            remoteEndPoint = new IPEndPoint(groupAddress, ClientListenPort);

            client = new UdpClient(AddressFamily.InterNetwork);
            client.ExclusiveAddressUse = false;

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(localAddress, SendFromPort));

            client.Ttl = Ttl;

            // ¬казуЇмо, через €кий адаптер слати multicast
            client.Client.SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.MulticastInterface,
                localAddress.GetAddressBytes());

            // Ќе обов'€зково дл€ в≥дправника, але не заважаЇ
            client.JoinMulticastGroup(groupAddress, localAddress);

            Text = $"Multicast Server | Local IP: {localAddress} | Sending to: {MulticastIp}:{ClientListenPort}";
        }

        private void LoadPictures()
        {
            DirectoryInfo di = new DirectoryInfo(PictDirectory);

            if (!di.Exists)
            {
                MessageBox.Show($"ѕапка '{PictDirectory}' не знайдена.");
                return;
            }

            fi = di.GetFiles()
                .Where(f =>
                    f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    f.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    f.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    f.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            comboBox1.Items.Clear();

            foreach (FileInfo item in fi)
            {
                comboBox1.Items.Add(item.Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string path = Path.Combine(PictDirectory, comboBox1.SelectedItem.ToString());

                using Bitmap original = new Bitmap(path);
                Bitmap copyForScreen = new Bitmap(original);
                Bitmap copyForSending = new Bitmap(original);

                Image old = pictureBox1.Image;
                pictureBox1.Image = copyForScreen;
                old?.Dispose();

                Task.Run(() => SendImage(copyForSending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Image send error");
            }
        }

        private void SendImage(Bitmap bitmap)
        {
            try
            {
                byte[] imageBytes;

                using (bitmap)
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    imageBytes = ms.ToArray();
                }

                Guid imageId = Guid.NewGuid();

                int totalParts = (int)Math.Ceiling((double)imageBytes.Length / MaxPayloadSize);

                for (int partIndex = 0; partIndex < totalParts; partIndex++)
                {
                    int offset = partIndex * MaxPayloadSize;
                    int payloadLength = Math.Min(MaxPayloadSize, imageBytes.Length - offset);

                    byte[] packet = new byte[HeaderSize + payloadLength];

                    // 0..3 magic IMG1
                    Buffer.BlockCopy(Magic, 0, packet, 0, 4);

                    // 4..19 image id
                    byte[] guidBytes = imageId.ToByteArray();
                    Buffer.BlockCopy(guidBytes, 0, packet, 4, 16);

                    // 20..23 total parts
                    Buffer.BlockCopy(BitConverter.GetBytes(totalParts), 0, packet, 20, 4);

                    // 24..27 part index
                    Buffer.BlockCopy(BitConverter.GetBytes(partIndex), 0, packet, 24, 4);

                    // 28..31 payload length
                    Buffer.BlockCopy(BitConverter.GetBytes(payloadLength), 0, packet, 28, 4);

                    // 32.. payload
                    Buffer.BlockCopy(imageBytes, offset, packet, HeaderSize, payloadLength);

                    client.Send(packet, packet.Length, remoteEndPoint);

                    // маленька пауза, щоб мережа не дропала пакети
                    Thread.Sleep(1);
                }

                BeginInvoke(() =>
                {
                    Text = $"Sent image: {imageBytes.Length} bytes, parts: {totalParts}";
                });
            }
            catch (Exception ex)
            {
                BeginInvoke(() =>
                {
                    MessageBox.Show(ex.ToString(), "SendImage error");
                });
            }
        }

        private static IPAddress GetLocalIPv4()
        {
            if (!string.IsNullOrWhiteSpace(ManualLocalIp))
                return IPAddress.Parse(ManualLocalIp);

            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni =>
                    ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .ToList();

            // —початку шукаЇмо адаптер з gateway
            foreach (var ni in interfaces)
            {
                var props = ni.GetIPProperties();

                bool hasGateway = props.GatewayAddresses.Any(g =>
                    g.Address.AddressFamily == AddressFamily.InterNetwork);

                if (!hasGateway)
                    continue;

                var ip = props.UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ip != null)
                    return ip.Address;
            }

            // якщо gateway нема, беремо перший IPv4
            foreach (var ni in interfaces)
            {
                var ip = ni.GetIPProperties().UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ip != null)
                    return ip.Address;
            }

            throw new Exception("Ќе знайдено активний IPv4 адаптер.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            client?.Close();
            client?.Dispose();

            base.OnFormClosing(e);
        }
    }
}