using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TelegramClient> TelegramClients => Set<TelegramClient>();
}

public static class DbMigrator
{
    public static void EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}

public class TelegramClient
{
    public Guid Id { get; set; }
    public long ClientId { get; set; }
}