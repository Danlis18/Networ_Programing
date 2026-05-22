using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

const string token = "8430347585:AAGYW8KwePKLBanoSrtXqR6_tg3aavKOdtA";

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
        case "/start":
            string commands = @"Список команд:
                /start - запуск бота
                /inline - меню
                /keyboard - вивід клавіатури";

            await bot.SendMessage(chat, commands);
            break;


        case "/inline":
            var inlineKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Google", "https://www.google.com/"),
                        InlineKeyboardButton.WithUrl("Telegram", "https://web.telegram.org"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Sticker"),
                        InlineKeyboardButton.WithCallbackData("Video"),
                    }
                }
                );

            await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: inlineKeyboard);



            break;

        case "/keyboard":

            var replyKeyBoardMarkup = new ReplyKeyboardMarkup(
                new[]
                {
                    new []
                    {
                        new KeyboardButton("Привіт"),
                        new KeyboardButton("Як справи?"),
                    },
                     new []
                    {
                        new KeyboardButton("Контакт") { RequestContact = true },
                        new KeyboardButton("Локація") { RequestLocation = true },
                    },
                }
                );

            await bot.SendMessage(chat, "Виберіть пункт: ", replyMarkup: replyKeyBoardMarkup);



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
