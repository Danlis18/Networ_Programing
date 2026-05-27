using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

const string token = "8875642959:AAFEDGYXcQfwTpsF2_ho61IXidRB5I7pvjg";
string apiKey = "fxr_live_f9bc67c95d236202afd7bcdc7187eac2e208";
string baseCurrency = "USD";

float uahToUsd = 0;
float uahToEur = 0;

HttpClient client = new HttpClient();

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();


async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception); 
}

async Task OnMessage(Message msg, UpdateType type)
{

    Chat chat = msg.Chat;

    switch (msg.Text)
    {
        case "/help":
            string helpCommands = @"Список команд:
                /current — отримати актуальний курс валют;
                /usd — курс долара;
                /eur — курс євро;
                /help — список доступних команд.";

            await bot.SendMessage(chat, helpCommands);
            break;


        case "/current":
            {
                string uriGeneral = $"https://api.fxratesapi.com/latest?api_key={apiKey}&base={baseCurrency}";

                string dataAll = await client.GetStringAsync(uriGeneral);
                Console.WriteLine(dataAll);

                JObject exchangeDataAll = JObject.Parse(dataAll);
                JObject ratesAll = (JObject)exchangeDataAll["rates"]!;
                string baseCode = (string)exchangeDataAll["base"]!;
                string dateRaw = (string)exchangeDataAll["date"]!;

                var buttons = new List<InlineKeyboardButton[]>();

                foreach (var rate in ratesAll)
                {
                    string currency = rate.Key;
                    decimal value = Convert.ToDecimal(rate.Value);

                    Console.WriteLine($"1 {baseCode} = {value:0.####} {currency}");

                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"{currency}: {value:0.####}", $"currency_{currency}")
                    });
                }

                var currentKeyboard = new InlineKeyboardMarkup(buttons);

                await bot.SendMessage(chat, "Актуальні курси валют до 100$", replyMarkup: currentKeyboard);
                break;
            }

        case "/usd":
            string uriUSD = $"https://api.fxratesapi.com/latest?api_key={apiKey}&base=USD";
            string data = await client.GetStringAsync(uriUSD);
            JObject exchangeData = JObject.Parse(data);
            JObject rates = (JObject)exchangeData["rates"]!;
            uahToUsd = Convert.ToSingle(rates["UAH"]!);
            var usdKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                        {
                            InlineKeyboardButton.WithCallbackData("USD", "usd"),
                            InlineKeyboardButton.WithCallbackData("=", "equal"),
                            InlineKeyboardButton.WithCallbackData($"{uahToUsd:F2} UAH", "usd_to_uah"),
                    },
                }
            );

            await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: usdKeyboard);
            break;

        case "/eur":
            string uriEUR = $"https://api.fxratesapi.com/latest?api_key={apiKey}&base=EUR";
            string dataEUR = await client.GetStringAsync(uriEUR);
            JObject exchangeDatEUR = JObject.Parse(dataEUR);
            JObject ratesEUR = (JObject)exchangeDatEUR["rates"]!;
            uahToEur = Convert.ToSingle(ratesEUR["UAH"]!);
            var eureurKeyboard = new InlineKeyboardMarkup(
        new[]
        {
                    new[]
                        {
                            InlineKeyboardButton.WithCallbackData("EUR", "eur"),
                            InlineKeyboardButton.WithCallbackData("=", "equal"),
                            InlineKeyboardButton.WithCallbackData($"{uahToEur:F2} UAH", "eur_to_uah"),
                    },
        }
        );

            await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: eureurKeyboard);
            break;
        default:

            await bot.SendPhoto(chat, "https://telegrambots.github.io/book/docs/photo-ara.jpg",
            "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>", ParseMode.Html);
            break;
    }
}