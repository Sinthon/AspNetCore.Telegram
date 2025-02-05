using AspNetCore.Telegram.Contracts;
using MassTransit;
using Telegram.Bot;

namespace AspNetCore.Telegram.Consumers;

public class MessageTriggeredConsumer(ITelegramBotClient botClient) : IConsumer<MessageTriggered>
{
    public async Task Consume(ConsumeContext<MessageTriggered> context)
    {
        var message = context.Message;

        Console.WriteLine($"Message triggered for client {message.ClientId}: {message.Message}");

        await botClient.SendMessage(message.ClientId, message.Message);
    }
}
