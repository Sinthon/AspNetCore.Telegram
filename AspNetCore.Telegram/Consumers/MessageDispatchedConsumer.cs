using AspNetCore.Telegram.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Telegram.Consumers;

public class MessageDispatchedConsumer(AppDbContext dbContext) : IConsumer<MessageDispatched>
{
    public async Task Consume(ConsumeContext<MessageDispatched> context)
    {
        var messages = await dbContext.TelegramClients
            .AsNoTracking()
            .ToListAsync();

        foreach (var message in messages)
        {
            await context.Publish<MessageTriggered>(new
            {
                message.ClientId,
                Message = "Hello, World!"
            });
        }
    }
}
