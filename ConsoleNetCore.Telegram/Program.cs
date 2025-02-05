// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

Console.WriteLine("Hello, World!");

var botToken = "8034741741:AAFrztlxEWQ4vYev5AMOMxyJTRJsCJRM5hM";


var client = new TelegramBotClient(new TelegramBotClientOptions(botToken));

client.OnMessage += async (sender, e) =>
{
    string messageText = "Winnie is at your service\n👇 Please click the button below to chat with Customer Service 👇";

    var inlineKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] { InlineKeyboardButton.WithUrl("Winnie", "https://t.me/YourCustomerServiceBot") }
    });

    await client.SendMessage("1081184913", messageText, replyMarkup: inlineKeyboard);

    //await client.SendMessage("1081184913", "This is a testing");
};


client.StartReceiving<UpdateHandler>();

Console.ReadLine();

public class UpdateHandler : IUpdateHandler
{
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
