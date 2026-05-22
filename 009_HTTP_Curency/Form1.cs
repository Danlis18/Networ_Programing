using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace _009_HTTP_Curency
{
    public partial class Form1 : Form
    {
        private ComboBox fromCurrency;
        private ComboBox toCurrency;
        private DateTimePicker datePicker;
        private TextBox amountBox;
        private Label rateValue;
        private Label resultValue;
        private ComboBox fromBox;
        private ComboBox toBox;

        string from = "USD";
        string to = "UAH";
        DateTime date = new DateTime(2009, 2, 9);
        string apiKey = "fxr_live_9f3bedbca8ed6670f3a13671178762ccd448";
        

        HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            CreateInterface();
            await LoadCurrenciesAsync();
        }

        private void CreateInterface()
        {
            this.Text = "Currency Converter";
            this.Size = new Size(760, 470);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(12, 15, 25);
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label title = new Label();
            title.Text = "Currency Converter";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(210, 35);
            this.Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Fast and clean exchange rate interface";
            subtitle.ForeColor = Color.FromArgb(140, 150, 175);
            subtitle.Font = new Font("Segoe UI", 11);
            subtitle.AutoSize = true;
            subtitle.Location = new Point(245, 88);
            this.Controls.Add(subtitle);

            Panel card = new Panel();
            card.Size = new Size(360, 270);
            card.Location = new Point(60, 135);
            card.BackColor = Color.FromArgb(24, 29, 45);
            this.Controls.Add(card);

            Label fromLabel = CreateLabel("From", 35, 35);
            fromBox = CreateComboBox(130, 30);

            Label toLabel = CreateLabel("To", 35, 85);
            toBox = CreateComboBox(130, 80);

            Label dateLabel = CreateLabel("Date", 35, 135);
            datePicker = new DateTimePicker();
            datePicker.Location = new Point(130, 130);
            datePicker.Size = new Size(170, 30);
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "dd.MM.yyyy";
            datePicker.Value = new DateTime(2018, 5, 9);
            datePicker.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Button okButton = new Button();
            okButton.Click += okButton_ClickAsync;
            okButton.Text = "Convert";
            okButton.Size = new Size(170, 38);
            okButton.Location = new Point(130, 185);
            okButton.BackColor = Color.FromArgb(45, 95, 255);
            okButton.ForeColor = Color.White;
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            okButton.Cursor = Cursors.Hand;

            card.Controls.Add(fromLabel);
            card.Controls.Add(fromBox);
            card.Controls.Add(toLabel);
            card.Controls.Add(toBox);
            card.Controls.Add(dateLabel);
            card.Controls.Add(datePicker);
            card.Controls.Add(okButton);

            Panel resultCard = new Panel();
            resultCard.Size = new Size(230, 270);
            resultCard.Location = new Point(455, 135);
            resultCard.BackColor = Color.FromArgb(20, 24, 38);
            this.Controls.Add(resultCard);

            Label resultTitle = new Label();
            resultTitle.Text = "Result";
            resultTitle.ForeColor = Color.FromArgb(145, 160, 255);
            resultTitle.Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            resultTitle.AutoSize = true;
            resultTitle.Location = new Point(25, 30);

            Label rateText = new Label();
            rateText.Text = "Exchange rate";
            rateText.ForeColor = Color.FromArgb(140, 150, 175);
            rateText.Font = new Font("Segoe UI", 10);
            rateText.AutoSize = true;
            rateText.Location = new Point(25, 85);

            rateValue = new Label();
            rateValue.Text = "1.1879";
            rateValue.ForeColor = Color.White;
            rateValue.Font = new Font("Segoe UI Semibold", 32, FontStyle.Bold);
            rateValue.AutoSize = true;
            rateValue.Location = new Point(22, 110);

            Label pairText = new Label();
            pairText.Text = "EUR → USD";
            pairText.ForeColor = Color.FromArgb(140, 150, 175);
            pairText.Font = new Font("Segoe UI", 11);
            pairText.AutoSize = true;
            pairText.Location = new Point(28, 190);

            resultCard.Controls.Add(resultTitle);
            resultCard.Controls.Add(rateText);
            resultCard.Controls.Add(rateValue);
            resultCard.Controls.Add(pairText);
        }
        private Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.ForeColor = Color.White;
            label.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            label.AutoSize = true;
            label.Location = new Point(x, y);
            return label;
        }
        private ComboBox CreateComboBox(int x, int y)
        {
            ComboBox box = new ComboBox();
            box.Location = new Point(x, y);
            box.Size = new Size(170, 30);
            box.DropDownStyle = ComboBoxStyle.DropDownList;
            box.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            return box;
        }


        private async Task LoadCurrenciesAsync()
        {
            try
            {
                fromBox.Items.Clear();
                toBox.Items.Clear();

                fromBox.Items.Add("Loading...");
                toBox.Items.Add("Loading...");
                fromBox.SelectedIndex = 0;
                toBox.SelectedIndex = 0;

                string url = $"https://api.fxratesapi.com/latest?api_key={apiKey}&base=USD&amount=100";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                string json = await client.GetStringAsync(url);

                JObject data = JObject.Parse(json);

                bool success = data["success"] != null && data["success"].Value<bool>();

                if (!success)
                {
                    MessageBox.Show("API повернув помилку:\n" + json);
                    return;
                }

                JObject rates = data["rates"] as JObject;

                if (rates == null)
                {
                    MessageBox.Show("Не знайдено rates. Відповідь API:\n" + json);
                    return;
                }

                string[] codes = rates.Properties()
                                      .Select(p => p.Name)
                                      .OrderBy(code => code)
                                      .ToArray();

                fromBox.Items.Clear();
                toBox.Items.Clear();

                fromBox.Items.AddRange(codes);
                toBox.Items.AddRange(codes);

                fromBox.SelectedItem = codes.Contains("USD") ? "USD" : codes[0];
                toBox.SelectedItem = codes.Contains("UAH") ? "UAH" : codes[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження валют:\n" + ex.Message);

                fromBox.Items.Clear();
                toBox.Items.Clear();

                fromBox.Items.AddRange(new object[] { "USD", "EUR", "UAH", "GBP", "PLN" });
                toBox.Items.AddRange(new object[] { "USD", "EUR", "UAH", "GBP", "PLN" });

                fromBox.SelectedItem = "USD";
                toBox.SelectedItem = "UAH";
            }
        }

        private async void okButton_ClickAsync(object sender, EventArgs e)
        {
            if (fromBox.SelectedItem == null || toBox.SelectedItem == null)
            {
                MessageBox.Show("Оберіть валюти");
                return;
            }


            string fromCurrency = fromBox.SelectedItem.ToString();
            string toCurrency = toBox.SelectedItem.ToString();

            if (fromCurrency == toCurrency)
            {
                rateValue.Text = "1";
                return;
            }

            string selectedDate = datePicker.Value.ToString("yyyy-MM-dd");

            string uri = $"https://api.fxratesapi.com/convert?{selectedDate}&from={fromCurrency}&to={toCurrency}";
            string data = await client.GetStringAsync(uri);

            JObject exchangeData = JObject.Parse(data);

            JObject rates = (JObject)exchangeData["rates"]!;

            decimal rate = rates[toCurrency].Value<decimal>();

            rateValue.Text = rate.ToString("0.####", CultureInfo.InvariantCulture);
        }
    }
}
