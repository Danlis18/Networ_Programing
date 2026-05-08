using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace _006_MulticastClient
{
    public partial class Form1 : Form
    {
        private const string MulticastIp = "224.5.6.11";
        private const int ListenPort = 7778;

        // якщо AUTO не працюЇ, впиши сюди св≥й IPv4, наприклад:
        // private const string ManualLocalIp = "192.168.1.25";
        private const string ManualLocalIp = "10.100.3.248";

        private const int HeaderSize = 32;
        private static readonly byte[] Magic = { (byte)'I', (byte)'M', (byte)'G', (byte)'1' };

        private UdpClient client;
        private IPAddress groupAddress;
        private CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<Guid, ImageBuffer> buffers = new Dictionary<Guid, ImageBuffer>();

        public Form1()
        {
            InitializeComponent();

            try
            {
                InitMulticast();
                Task.Run(() => Listener(cts.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Client multicast error");
            }
        }

        private void InitMulticast()
        {
            IPAddress localAddress = GetLocalIPv4();
            groupAddress = IPAddress.Parse(MulticastIp);

            client = new UdpClient(AddressFamily.InterNetwork);
            client.ExclusiveAddressUse = false;

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, ListenPort));

            client.JoinMulticastGroup(groupAddress, localAddress);

            Text = $"Multicast Client | Local IP: {localAddress} | Listening: {ListenPort}";
        }

        private void Listener(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    byte[] packet = client.Receive(ref ep);

                    if (!TryParsePacket(packet, out Guid imageId, out int totalParts, out int partIndex, out byte[] payload))
                        continue;

                    if (totalParts <= 0 || partIndex < 0 || partIndex >= totalParts)
                        continue;

                    ImageBuffer buffer;

                    lock (buffers)
                    {
                        if (!buffers.TryGetValue(imageId, out buffer))
                        {
                            buffer = new ImageBuffer(totalParts);
                            buffers[imageId] = buffer;
                        }
                    }

                    bool imageReady = false;
                    byte[] fullImage = null;

                    lock (buffer)
                    {
                        if (buffer.TotalParts != totalParts)
                            continue;

                        if (buffer.Parts[partIndex] == null)
                        {
                            buffer.Parts[partIndex] = payload;
                            buffer.ReceivedParts++;
                            buffer.LastUpdate = DateTime.Now;
                        }

                        if (buffer.ReceivedParts == buffer.TotalParts)
                        {
                            fullImage = CombineParts(buffer.Parts);
                            imageReady = true;
                        }
                    }

                    if (imageReady)
                    {
                        lock (buffers)
                        {
                            buffers.Remove(imageId);
                        }

                        ShowImage(fullImage);
                    }

                    CleanupOldBuffers();
                }
            }
            catch (ObjectDisposedException)
            {
                // форма закриваЇтьс€
            }
            catch (SocketException)
            {
                // сокет закривс€
            }
            catch (Exception ex)
            {
                BeginInvoke(() =>
                {
                    MessageBox.Show(ex.ToString(), "Listener error");
                });
            }
        }

        private void ShowImage(byte[] imageBytes)
        {
            try
            {
                using MemoryStream ms = new MemoryStream(imageBytes);
                Bitmap bmp = new Bitmap(ms);

                pictureBox1.BeginInvoke(() =>
                {
                    Image old = pictureBox1.Image;
                    pictureBox1.Image = bmp;
                    old?.Dispose();
                });
            }
            catch
            {
                // прийшло щось не схоже на картинку
            }
        }

        private static bool TryParsePacket(
            byte[] packet,
            out Guid imageId,
            out int totalParts,
            out int partIndex,
            out byte[] payload)
        {
            imageId = Guid.Empty;
            totalParts = 0;
            partIndex = 0;
            payload = Array.Empty<byte>();

            if (packet == null || packet.Length < HeaderSize)
                return false;

            for (int i = 0; i < Magic.Length; i++)
            {
                if (packet[i] != Magic[i])
                    return false;
            }

            byte[] guidBytes = new byte[16];
            Buffer.BlockCopy(packet, 4, guidBytes, 0, 16);

            imageId = new Guid(guidBytes);
            totalParts = BitConverter.ToInt32(packet, 20);
            partIndex = BitConverter.ToInt32(packet, 24);
            int payloadLength = BitConverter.ToInt32(packet, 28);

            if (payloadLength < 0 || HeaderSize + payloadLength > packet.Length)
                return false;

            payload = new byte[payloadLength];
            Buffer.BlockCopy(packet, HeaderSize, payload, 0, payloadLength);

            return true;
        }

        private static byte[] CombineParts(byte[][] parts)
        {
            int totalLength = parts.Sum(p => p.Length);
            byte[] result = new byte[totalLength];

            int offset = 0;

            foreach (byte[] part in parts)
            {
                Buffer.BlockCopy(part, 0, result, offset, part.Length);
                offset += part.Length;
            }

            return result;
        }

        private void CleanupOldBuffers()
        {
            lock (buffers)
            {
                var oldKeys = buffers
                    .Where(x => (DateTime.Now - x.Value.LastUpdate).TotalSeconds > 10)
                    .Select(x => x.Key)
                    .ToList();

                foreach (Guid key in oldKeys)
                    buffers.Remove(key);
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
            cts.Cancel();
            client?.Close();
            client?.Dispose();

            base.OnFormClosing(e);
        }

        private class ImageBuffer
        {
            public int TotalParts { get; }
            public byte[][] Parts { get; }
            public int ReceivedParts { get; set; }
            public DateTime LastUpdate { get; set; }

            public ImageBuffer(int totalParts)
            {
                TotalParts = totalParts;
                Parts = new byte[totalParts][];
                ReceivedParts = 0;
                LastUpdate = DateTime.Now;
            }
        }
    }
}