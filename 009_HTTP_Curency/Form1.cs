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
        private ComboBox fromBox;
        private ComboBox toBox;
        private DateTimePicker datePicker;

        private Label rateValue;
        private Label pairText;
        private Label dateText;

        private Button okButton;

        private readonly HttpClient client = new HttpClient();

        private string apiKey = "fxr_live_f9bc67c95d236202afd7bcdc7187eac2e208";
        private string baseCurrency = "USD";

        private JObject loadedRates;
        private string loadedDate = "";

        public Form1()
        {
            InitializeComponent();

            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Clear();
            CreateInterface();
            this.Shown += async (s, e) => await LoadCurrenciesAsync();
        }

        private void CreateInterface()
        {
            this.Controls.Clear();

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
            datePicker.Value = DateTime.Now;
            datePicker.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            datePicker.Enabled = false;

            okButton = new Button();
            okButton.Text = "Convert";
            okButton.Size = new Size(170, 38);
            okButton.Location = new Point(130, 185);
            okButton.BackColor = Color.FromArgb(45, 95, 255);
            okButton.ForeColor = Color.White;
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            okButton.Cursor = Cursors.Hand;
            okButton.Click += okButton_ClickAsync;

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

            Label rateLabel = new Label();
            rateLabel.Text = "Exchange rate";
            rateLabel.ForeColor = Color.FromArgb(140, 150, 175);
            rateLabel.Font = new Font("Segoe UI", 10);
            rateLabel.AutoSize = true;
            rateLabel.Location = new Point(25, 85);

            rateValue = new Label();
            rateValue.Text = "Loading...";
            rateValue.ForeColor = Color.White;
            rateValue.Font = new Font("Segoe UI Semibold", 26, FontStyle.Bold);
            rateValue.AutoSize = true;
            rateValue.Location = new Point(22, 110);

            pairText = new Label();
            pairText.Text = "USD → UAH";
            pairText.ForeColor = Color.FromArgb(140, 150, 175);
            pairText.Font = new Font("Segoe UI", 11);
            pairText.AutoSize = true;
            pairText.Location = new Point(28, 190);

            dateText = new Label();
            dateText.Text = "Date: -";
            dateText.ForeColor = Color.FromArgb(100, 110, 135);
            dateText.Font = new Font("Segoe UI", 9);
            dateText.AutoSize = true;
            dateText.Location = new Point(28, 220);

            resultCard.Controls.Add(resultTitle);
            resultCard.Controls.Add(rateLabel);
            resultCard.Controls.Add(rateValue);
            resultCard.Controls.Add(pairText);
            resultCard.Controls.Add(dateText);
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
                okButton.Enabled = false;

                string uri = $"https://api.fxratesapi.com/latest?apikey={apiKey}&base={baseCurrency}";

                string json = await client.GetStringAsync(uri);

                JObject exchangeData = JObject.Parse(json);

                bool success = exchangeData["success"] != null &&
                               exchangeData["success"].Value<bool>();

                loadedRates = exchangeData["rates"] as JObject;
                loadedDate = exchangeData["date"].ToString();

                string[] codes = loadedRates.Properties()
                                            .Select(p => p.Name)
                                            .OrderBy(code => code)
                                            .ToArray();

                fromBox.Items.Clear();
                toBox.Items.Clear();

                fromBox.Items.AddRange(codes);
                toBox.Items.AddRange(codes);

                fromBox.SelectedItem = codes.Contains("USD") ? "USD" : codes[0];
                toBox.SelectedItem = codes.Contains("UAH") ? "UAH" : codes[0];

                dateText.Text = "Date: " + loadedDate;

                ConvertCurrency();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження валют:\n" + ex.Message);
            }
            finally
            {
                okButton.Enabled = true;
            }
        }
        private decimal GetCurrencyRate(string currencyCode)
        {
            if (currencyCode == baseCurrency)
            {
                return 1;
            }

            if (loadedRates == null || loadedRates[currencyCode] == null)
            {
                throw new Exception("Не знайдено валюту: " + currencyCode);
            }

            return loadedRates[currencyCode].Value<decimal>();
        }
        private void ConvertCurrency()
        {
            try
            {
                if (loadedRates == null)
                {
                    MessageBox.Show("Курси ще не завантажені");
                    return;
                }

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
                    pairText.Text = $"1 {fromCurrency} = 1 {toCurrency}";
                    return;
                }

                decimal fromRate = GetCurrencyRate(fromCurrency);
                decimal toRate = GetCurrencyRate(toCurrency);

                decimal resultRate = toRate / fromRate;

                string resultText = resultRate.ToString("0.####", CultureInfo.InvariantCulture);

                rateValue.Text = resultText;
                pairText.Text = $"1 {fromCurrency} = {resultText} {toCurrency}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка конвертації:\n" + ex.Message);
            }
        }
        private void okButton_ClickAsync(object sender, EventArgs e)
        {
            ConvertCurrency();
        }
    }
}