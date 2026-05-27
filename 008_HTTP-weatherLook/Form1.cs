using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using _008_HTTP_weatherLook;    

namespace _008_HTTP_weatherLook
{
    public partial class Form1 : Form
    {
        private TextBox txtCity;
        private Button btnOk;
        private PictureBox pbWeather;

        private Label lblDate;
        private Label lblTemp;
        private Label lblFeel;
        private Label lblDescription;
        private Label lblSunrise;
        private Label lblSunset;
        private Label lblDuration;
        private Label lblCityTitle;

        private HttpClient httpClient = new HttpClient();
        private string ApiKey = "0844a69f62c606c23b3d7fe28770fcf9";



        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            this.Text = "My Weather";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(900, 420);
            this.BackColor = Color.FromArgb(224, 224, 224);
            this.Font = new Font("Segoe UI", 10);

            Panel topPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(840, 130),
                BackColor = Color.White
            };
            this.Controls.Add(topPanel);

            Label title = new Label
            {
                Text = "MY WEATHER",
                ForeColor = Color.Teal,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            topPanel.Controls.Add(title);

            txtCity = new TextBox
            {
                Location = new Point(540, 25),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 11),
                Text = "Lviv"
            };
            topPanel.Controls.Add(txtCity);

            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(620, 75),
                Size = new Size(120, 45),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            btnOk.FlatAppearance.BorderColor = Color.Red;
            btnOk.FlatAppearance.BorderSize = 3;
            btnOk.Click += async (s, e) => await LoadWeatherAsync();
            topPanel.Controls.Add(btnOk);

            Panel weatherPanel = new Panel
            {
                Location = new Point(45, 165),
                Size = new Size(790, 170),
                BackColor = Color.White
            };
            this.Controls.Add(weatherPanel);

            Label currentTitle = new Label
            {
                Text = "CURRENT WEATHER",
                ForeColor = Color.Teal,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            weatherPanel.Controls.Add(currentTitle);

            lblDate = new Label
            {
                Text = DateTime.Now.ToString("dd.MM.yyyy"),
                ForeColor = Color.Teal,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(650, 20),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblDate);

            lblCityTitle = new Label
            {
                Text = "",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 45),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblCityTitle);

            pbWeather = new PictureBox
            {
                Location = new Point(80, 75),
                Size = new Size(70, 70),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            weatherPanel.Controls.Add(pbWeather);

            lblDescription = new Label
            {
                Text = "Sunny",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(75, 145),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblDescription);

            lblTemp = new Label
            {
                Text = "29°C",
                ForeColor = Color.FromArgb(45, 45, 45),
                Font = new Font("Segoe UI", 34, FontStyle.Regular),
                Location = new Point(290, 65),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblTemp);

            lblFeel = new Label
            {
                Text = "Real Feel 30°",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10),
                Location = new Point(305, 130),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblFeel);

            lblSunrise = new Label
            {
                Text = "Sunrise:  --:--",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(560, 70),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblSunrise);

            lblSunset = new Label
            {
                Text = "Sunset:   --:--",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(560, 105),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblSunset);

            lblDuration = new Label
            {
                Text = "Duration: --:-- hr",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(560, 140),
                AutoSize = true
            };
            weatherPanel.Controls.Add(lblDuration);

            //pbWeather.Image = CreateWeatherIcon(0);
        }

        private async Task LoadWeatherAsync()
        {
            string uri = $"https://api.openweathermap.org/data/2.5/weather?q={txtCity.Text}&appid={ApiKey}&mode=xml&units=metric";
            string conect = await httpClient.GetStringAsync(uri);
            Current weatherData = Serializer.Deserialize<Current>(conect);

            lblTemp.Text = $"{weatherData.Temperature.Value}°C";
            lblFeel.Text = $"Real Feel {weatherData.Feels_like.Value}°";
            lblDescription.Text = weatherData.Weather.Value;
            lblCityTitle.Text = $"{weatherData.City.Name}, {weatherData.City.Country}";
            lblSunrise.Text = $"Sunrise: {DateTime.Parse(weatherData.City.Sun.Rise).ToLocalTime():HH:mm}";
            lblSunset.Text = $"Sunset:   {DateTime.Parse(weatherData.City.Sun.Set).ToLocalTime():HH:mm}";
            lblDuration.Text = $"Duration: {DateTime.Parse(weatherData.City.Sun.Set).ToLocalTime() - DateTime.Parse(weatherData.City.Sun.Rise).ToLocalTime()}";

            string iconCode = weatherData.Weather.Icon;
            string iconUrl = $"https://openweathermap.org/img/wn/{iconCode}@2x.png";
            byte[] imageBytes = await httpClient.GetByteArrayAsync(iconUrl);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                pbWeather.Image = new Bitmap(ms);
            }
        }
    }
}


