using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public class TelegramMessageUpdateHandler(IServiceScopeFactory factory) : IUpdateHandler
{
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Telegram bot error: {exception.Message}");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message || message.From is null)
            return;

        var userId = message.From.Id;
        var userMessage = message.Text ?? string.Empty;

        using var scope = factory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        switch (userMessage.ToLower())
        {
            case "/start":
                await SendStartMessage(botClient, userId, cancellationToken);
                break;

            case "/unbind":
                await UnbindUser(botClient, dbContext, userId, cancellationToken);
                break;

            default:
                await SendDefaultMessage(botClient, userId, cancellationToken);
                break;
        }
    }

    private async Task SendStartMessage(ITelegramBotClient botClient, long userId, CancellationToken cancellationToken)
    {
        const string messageText = "ទទួលបានសារជូនដំណឹងភ្លាមៗតាមរយៈគណនីតាមតេឡេក្រាមរបស់អ្នក។ សូមភ្ជាប់គណនីរបស់អ្នកជាមុនសិន!";

        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("ភ្ជាប់គណនី", $"https://7cbb-116-212-150-208.ngrok-free.app/api/telegram/bind-account?tgid={userId}"));

        await botClient.SendMessage(
            userId,
            messageText,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task UnbindUser(ITelegramBotClient botClient, AppDbContext dbContext, long userId, CancellationToken cancellationToken)
    {
        await dbContext.TelegramClients
            .Where(x => x.ClientId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        const string messageText = "អ្នកបានផ្ដាច់គណនីដោយជោគជ័យ! សារជូនដំណឹងនឹងមិនផ្ញើរជូនតាមតេឡេក្រាមទៀតទេ!";

        await botClient.SendMessage(
            userId,
            messageText,
            cancellationToken: cancellationToken);
    }

    private async Task SendDefaultMessage(ITelegramBotClient botClient, long userId, CancellationToken cancellationToken)
    {
        const string messageText = "ជជែកជាមួយផ្នែកសេវាកម្មអតិថិជន 👇 សូមចុចប៊ូតុងខាងក្រោម 👇";

        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("ផ្នែកសេវាកម្មអតិថិជន", $"https://t.me/sinthan_seng"));

        await botClient.SendMessage(
            userId,
            messageText,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}