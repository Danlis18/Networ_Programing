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
string uriGeneral = $"https://api.fxratesapi.com/latest?api_key={apiKey}&base={baseCurrency}";

float uahToUsd = 0;
float uahToEur = 0;

HttpClient client = new HttpClient();






using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel(); // stop the bot

// method to handle errors in polling or in your OnMessage/OnUpdate code
async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception); // just dump the exception to the console
}

// method that handle messages received by the bot:
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
                string data = await client.GetStringAsync(uriGeneral);
                Console.WriteLine(data);

                JObject exchangeData = JObject.Parse(data);
                JObject rates = (JObject)exchangeData["rates"]!;
                string baseCode = (string)exchangeData["base"]!;
                string dateRaw = (string)exchangeData["date"]!;

                var buttons = new List<InlineKeyboardButton[]>();

                foreach (var rate in rates)
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
    var usdKeyboard = new InlineKeyboardMarkup(
        new[]
        {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("USD", ""),
                        InlineKeyboardButton.WithCallbackData("="),
                        InlineKeyboardButton.WithUrl($"{uahToUsd}UAH", "https://web.telegram.org"),
                    },
        }
        );

    await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: usdKeyboard);
    break;

case "/eur":
    var eureurKeyboard = new InlineKeyboardMarkup(
        new[]
        {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("EUR", ""),
                        InlineKeyboardButton.WithCallbackData("="),
                        InlineKeyboardButton.WithUrl($"{uahToEur}UAH", "https://web.telegram.org"),
                    },
        }
        );

    await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: eureurKeyboard);
    break;

    //case "/usd":

    //    var replyKeyBoardMarkup = new ReplyKeyboardMarkup(
    //        new[]
    //        {
    //            new []
    //            {
    //                new KeyboardButton("Привіт"),
    //                new KeyboardButton("Як справи?"),
    //            },
    //             new []
    //            {
    //                new KeyboardButton("Контакт") { RequestContact = true },
    //                new KeyboardButton("Локація") { RequestLocation = true },
    //            },
    //        }
    //        );

    //    await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: replyKeyBoardMarkup);



    break;


case "Привіт":

    await bot.SendMessage(chat, "І тобі привіт!!!");
    break;


default:

    await bot.SendPhoto(chat, "https://telegrambots.github.io/book/docs/photo-ara.jpg",
    "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>", ParseMode.Html);



    break;

}




}

// method that handle other types of updates received by the bot:
async Task OnUpdate(Update update)
{

    if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
    {

        await using Stream stream = File.OpenRead(@"C:\\Users\\danil\\Downloads\\Seedance 2_0 - Create a smooth_ high-quality cinematic animation on a pure black background_At the b.mp4");
        switch (query.Data)
        {
            case "Stiсker":
                var message1 = await bot.SendSticker(query.Message!.Chat, "https://api.fstik.app/file/AAMCBQADFQABag-66gfpFlYzGDwE5tUzrcUgEtoAAs8EAAJ82IhWw-qHdDyHB8UBAAdtAAM7BA/sticker.webp");
                var message2 = await bot.SendSticker(query.Message!.Chat, message1.Sticker!.FileId);
                break;

            case "Video":
                await bot.SendVideo(query.Message!.Chat, stream);
                break;
        }


        // await bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");

    }
}
