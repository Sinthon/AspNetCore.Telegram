namespace AspNetCore.Telegram.Contracts;

public class MessageTriggered
{
    public long ClientId { get; set; }
    public string Message { get; set; } = default!;
}
