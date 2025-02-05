using AspNetCore.Telegram.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace AspNetCore.Telegram.Endpoints;

public static class TelegramEndpoints
{
    public static void MapTelegramEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/telegram")
            .WithTags("Telegram");

        app.MapGet("place-order", async (IPublishEndpoint publishEndpoint, CancellationToken ctn) =>
        {
            await publishEndpoint.Publish(new MessageDispatched(), ctn);
            return Results.Ok();
        });

        group.MapGet("/unbind-account", UnBindAccount);
        group.MapPost("/unbind-account", UnBindAccount);

        group.MapGet("/bind-account", BindAccount);
        group.MapPost("/bind-account", BindAccount);
    }

    private static async Task<IResult> BindAccount(long tgid, ITelegramBotClient botClient, AppDbContext context, CancellationToken ctn)
    {
        if (!await context.TelegramClients.AnyAsync(x => x.ClientId == tgid, ctn))
        {
            context.TelegramClients.Add(new TelegramClient
            {
                ClientId = tgid
            });
            await context.SaveChangesAsync(ctn);
        }

        var messageText = "សូមអបអរសាទរអ្នកបានភ្ជាប់គណនីដោយជោគជ័យ។";
        await botClient.SendMessage(tgid, messageText);

        return Results.LocalRedirect("/telegram/account/bind-success");
    }

    private static async Task<IResult> UnBindAccount(long tgid, ITelegramBotClient botClient, AppDbContext context, CancellationToken ctn)
    {
        await context.TelegramClients
            .Where(x => x.ClientId == tgid)
            .ExecuteDeleteAsync(ctn);

        string messageText = "អ្នកបានផ្ដាច់គណនីដោយជោគជ័យ! សារជូនដំណឹងនឹងមិនផ្ញើរជូនតាមតេឡេក្រាមទៀតទេ!";
        await botClient.SendMessage(tgid, messageText);

        return Results.Ok();
    }
}
