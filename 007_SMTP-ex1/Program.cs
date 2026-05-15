using System.Net;
using System.Net.Mail;

async void SendEmailAsync()
{
    try
    {
        string smtpAddress = "smtp.gmail.com";
        int port = 587;
        string userName = "danilolisnicuk21@gmail.com";
        string appPassword = "xdvk atza gora ntiy";

        string emailFrom = "danilolisnicuk21@gmail.com";
        string emailTo = "danilolisnicuk9@gmail.com";

        string subject = "Hello, its subject";
        string body = "<b style='color: red; background-color: orange'>Hi, today is a nice day!!!</b>";

        string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;'>

    <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f8; padding:30px 0;'>
        <tr>
            <td align='center'>

                <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.08);'>

                    <tr>
                        <td style='background-color:#2563eb; padding:24px; text-align:center; color:#ffffff;'>
                            <h1 style='margin:0; font-size:24px;'>Вітаємо!</h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:32px; color:#333333;'>
                            <h2 style='margin-top:0; color:#111827;'>Ваш запит успішно оброблено</h2>

                            <p style='font-size:16px; line-height:1.6;'>
                                Дякуємо, що скористалися нашим сервісом.
                            </p>

                            <div style='background-color:#f9fafb; border-left:4px solid #2563eb; padding:16px; margin:24px 0;'>
                                <p style='margin:0; font-size:15px; color:#374151;'>
                                    <strong>Статус:</strong> Успішно<br>
                                    <strong>Дата:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}
                                </p>
                            </div>

                            <p style='font-size:16px; line-height:1.6;'>
                                Якщо у вас виникли питання, просто дайте відповідь на цей лист.
                            </p>

                            <div style='text-align:center; margin-top:30px;'>
                                <a href='https://example.com'
                                   style='background-color:#2563eb; color:#ffffff; text-decoration:none; padding:14px 24px; border-radius:8px; display:inline-block; font-weight:bold;'>
                                    Перейти на сайт
                                </a>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style='background-color:#f3f4f6; padding:18px; text-align:center; color:#6b7280; font-size:13px;'>
                            © 2026 Your Company. Усі права захищено.
                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";

        MailAddress from = new MailAddress(emailFrom);
        MailAddress to = new MailAddress(emailTo);
        MailMessage message = new MailMessage(from, to);

        message.IsBodyHtml = true;//для того, чтобы в теле письма можно было использовать html теги

        message.Subject = subject;
        message.Body = htmlBody;


        string filePath = @"C:\Users\danil\Downloads\Gen-4_5 - PromptСтвори суперреалістичне динамічне відео на основі цього зображення слот-машини_ Каме.gen-4_5 - promptствори суперреалістичне динамічне.mp4";
        message.Attachments.Add(new Attachment(filePath));

        SmtpClient smtpClient = new SmtpClient(smtpAddress, port);

        smtpClient.Credentials = new NetworkCredential(userName, appPassword);
        smtpClient.EnableSsl = true;
        await smtpClient.SendMailAsync(message);

        Console.WriteLine("Message Send Successfully!");

    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

SendEmailAsync();

Console.WriteLine("Main not blocked!!!");
Console.ReadKey();