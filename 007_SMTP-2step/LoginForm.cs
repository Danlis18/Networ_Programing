using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace _007_SMTP_2step
{
    public partial class LoginForm : Form
    {
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private TextBox codeTextBox;
        private Button loginButton;
        private Button verifyButton;
        private Label titleLabel;

        string smtpAddress = "smtp.gmail.com";
        int port = 587;
        string userName = "danilolisnicuk21@gmail.com";
        string appPassword = "xdvk atza gora ntiy";

        string emailFrom = "danilolisnicuk21@gmail.com";
        string emailTo = "danilolisnicuk9@gmail.com";

        string password = "12345";

        string subject = "Hello, its subject";

        Random random = new Random();
        int code;

        public LoginForm()
        {
            InitializeComponent();

            this.Text = "Login";
            this.Size = new Size(420, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            Panel mainPanel = new Panel();
            mainPanel.Size = new Size(340, 500);
            mainPanel.Location = new Point(33, 20);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            titleLabel = new Label();
            titleLabel.Text = "Увійти в акаунт";
            titleLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(31, 41, 55);
            titleLabel.AutoSize = false;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(300, 50);
            titleLabel.Location = new Point(20, 25);

            Label emailLabel = new Label();
            emailLabel.Text = "Email";
            emailLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            emailLabel.ForeColor = Color.FromArgb(75, 85, 99);
            emailLabel.Location = new Point(35, 95);
            emailLabel.AutoSize = true;

            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 11);
            emailTextBox.Size = new Size(270, 32);
            emailTextBox.Location = new Point(35, 120);
            emailTextBox.PlaceholderText = "example@gmail.com";

            Label passwordLabel = new Label();
            passwordLabel.Text = "Пароль";
            passwordLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            passwordLabel.ForeColor = Color.FromArgb(75, 85, 99);
            passwordLabel.Location = new Point(35, 165);
            passwordLabel.AutoSize = true;

            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 11);
            passwordTextBox.Size = new Size(270, 32);
            passwordTextBox.Location = new Point(35, 190);
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.PlaceholderText = "Введіть пароль";

            loginButton = new Button();
            loginButton.Text = "Продовжити";
            loginButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            loginButton.Size = new Size(270, 42);
            loginButton.Location = new Point(35, 245);
            loginButton.BackColor = Color.FromArgb(37, 99, 235);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;

            Label dividerLabel = new Label();
            dividerLabel.Text = "2-Step Verification";
            dividerLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dividerLabel.ForeColor = Color.FromArgb(31, 41, 55);
            dividerLabel.AutoSize = false;
            dividerLabel.TextAlign = ContentAlignment.MiddleCenter;
            dividerLabel.Size = new Size(270, 30);
            dividerLabel.Location = new Point(35, 310);

            Label codeLabel = new Label();
            codeLabel.Text = "Введіть код підтвердження";
            codeLabel.Font = new Font("Segoe UI", 10);
            codeLabel.ForeColor = Color.FromArgb(75, 85, 99);
            codeLabel.Location = new Point(35, 350);
            codeLabel.AutoSize = true;

            codeTextBox = new TextBox();
            codeTextBox.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            codeTextBox.Size = new Size(270, 38);
            codeTextBox.Location = new Point(35, 375);
            codeTextBox.TextAlign = HorizontalAlignment.Center;
            codeTextBox.MaxLength = 6;
            codeTextBox.PlaceholderText = "000000";

            verifyButton = new Button();
            verifyButton.Text = "Підтвердити код";
            verifyButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            verifyButton.Size = new Size(270, 42);
            verifyButton.Location = new Point(35, 425);
            verifyButton.BackColor = Color.FromArgb(16, 185, 129);
            verifyButton.ForeColor = Color.White;
            verifyButton.FlatStyle = FlatStyle.Flat;
            verifyButton.FlatAppearance.BorderSize = 0;
            verifyButton.Cursor = Cursors.Hand;
            verifyButton.Click += VerifyButton_Click;

            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(loginButton);
            mainPanel.Controls.Add(dividerLabel);
            mainPanel.Controls.Add(codeLabel);
            mainPanel.Controls.Add(codeTextBox);
            mainPanel.Controls.Add(verifyButton);

            this.Controls.Add(mainPanel);
        }


        private void LoginButton_Click(object sender, EventArgs e)
        {
            

            code = GenerateCode();

            if (passwordTextBox.Text == password && emailTextBox.Text == emailTo)
            {
                SendEmailAsync(code);
            }
            else
            {
                MessageBox.Show("Неправильний логін або пароль. Спробуйте ще раз.");
            }
        }
        private void VerifyButton_Click(object sender, EventArgs e)
        {
            if (codeTextBox.Text == "")
            {
                MessageBox.Show("Будь ласка, введіть код підтвердження.");
                return;
            }
            if (codeTextBox.Text == codeTextBox.PlaceholderText)
            {
                MessageBox.Show("Будь ласка, згенеруйте код підтвердження.");
                return;
            }
            if (codeTextBox.Text == code.ToString())
            {
                MessageBox.Show("Ви успішно увійшли в акаунт!");
            }
            else
            {
                MessageBox.Show("Невірний код підтвердження. Спробуйте ще раз.");
            }
        }

        static string GetHtmlBody(string code, string companyName)
        {
            string safeCode = WebUtility.HtmlEncode(code);
            string safeCompanyName = WebUtility.HtmlEncode(companyName);

            return $@"
<!DOCTYPE html>
<html lang='uk'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>2-Step Verification</title>
</head>
<body style='margin:0; padding:0; background-color:#0b0f14; font-family:Segoe UI, Arial, sans-serif; color:#e5e7eb;'>

    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#0b0f14; padding:40px 0;'>
        <tr>
            <td align='center'>

                <table width='640' cellpadding='0' cellspacing='0' border='0' style='background-color:#111827; border:1px solid #1f2937; border-radius:20px; overflow:hidden; box-shadow:0 10px 40px rgba(0,0,0,0.45);'>

                    <tr>
                        <td align='center' style='padding:36px 30px; background:linear-gradient(135deg, #111827, #0f172a); border-bottom:1px solid #1f2937;'>
                            <div style='font-size:14px; letter-spacing:3px; text-transform:uppercase; color:#9ca3af; margin-bottom:10px;'>
                                Secure Access
                            </div>

                            <h1 style='margin:0; font-size:30px; color:#f9fafb; font-weight:700;'>
                                2-Step Verification
                            </h1>

                            <p style='margin:12px 0 0 0; font-size:15px; color:#9ca3af;'>
                                Підтвердження безпечного входу у ваш акаунт
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:42px 42px 28px 42px;'>

                            <h2 style='margin:0 0 16px 0; font-size:24px; color:#f3f4f6;'>
                                Ваш код підтвердження
                            </h2>

                            <p style='margin:0 0 24px 0; font-size:16px; line-height:1.7; color:#d1d5db;'>
                                Щоб завершити вхід, введіть цей одноразовий код у формі підтвердження.
                            </p>

                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='margin:28px 0;'>
                                <tr>
                                    <td align='center'>
                                        <div style='display:inline-block; background-color:#0f172a; border:1px solid #374151; border-radius:16px; padding:22px 34px;'>
                                            <span style='font-size:34px; font-weight:700; letter-spacing:10px; color:#fbbf24;'>
                                                {safeCode}
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#0b1220; border:1px solid #1f2937; border-left:4px solid #fbbf24; border-radius:12px; margin:22px 0 28px 0;'>
                                <tr>
                                    <td style='padding:18px 20px; color:#d1d5db; font-size:15px; line-height:1.7;'>
                                        <strong style='color:#f9fafb;'>Важливо:</strong><br>
                                        • Код дійсний протягом <strong style='color:#fbbf24;'>10 хвилин</strong><br>
                                        • Не передавайте його іншим особам<br>
                                        • Якщо це були не ви — змініть пароль
                                    </td>
                                </tr>
                            </table>

                            <p style='margin:0 0 18px 0; font-size:15px; line-height:1.7; color:#9ca3af;'>
                                Якщо ви не намагалися увійти в акаунт, просто проігноруйте цей лист.
                            </p>

                        </td>
                    </tr>

                    <tr>
                        <td style='padding:24px 42px 36px 42px; border-top:1px solid #1f2937;'>
                            <p style='margin:0; font-size:13px; line-height:1.7; color:#6b7280; text-align:center;'>
                                © 2026 {safeCompanyName} · Secure Authentication System<br>
                                Цей лист створено автоматично, будь ласка, не відповідайте на нього.
                            </p>
                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
        }
        int GenerateCode()
        {
            code = 0;
            for (int i = 0; i < 6; i++)
            {
                code = code * 10 + random.Next(0, 10);
            }
            return code;
        }
        async void SendEmailAsync(int code)
        {
            try
            {
                MailAddress from = new MailAddress(emailFrom);
                MailAddress to = new MailAddress(emailTo);
                MailMessage message = new MailMessage(from, to);

                message.IsBodyHtml = true;

                message.Subject = subject;

                message.Body = GetHtmlBody(code.ToString(), "Danlis Team");
                message.IsBodyHtml = true;
                //message.Attachments.Add(new Attachment(filePath));

                SmtpClient smtpClient = new SmtpClient(smtpAddress, port);

                smtpClient.Credentials = new NetworkCredential(userName, appPassword);
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(message);

                MessageBox.Show("Message Send Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
