using AspNetCore.Telegram.Components;
using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using AspNetCore.Telegram.Consumers;
using AspNetCore.Telegram.Endpoints;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

// register aspire services
builder.AddServiceDefaults();

// register ef core db context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("data source=bot.db"));

// register masstransit  
builder.Services.AddMassTransit(busConfig =>
{
    busConfig.SetKebabCaseEndpointNameFormatter();
    busConfig.AddConsumer<MessageDispatchedConsumer>();
    busConfig.AddConsumer<MessageTriggeredConsumer>();
    busConfig.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration
            .GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

// register telegram services
builder.Services.AddSingleton<IUpdateHandler, TelegramMessageUpdateHandler>();
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var botToken = builder.Configuration.GetValue<string>("TelegramBot:Token")!;
    return new TelegramBotClient(botToken);
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DiagnosticHeaders.DefaultListenerName));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Map aspire endpoints
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.EnsureDatabaseCreated();
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using IServiceScope scope = app.Services.CreateScope();

ITelegramBotClient botClient = scope.ServiceProvider
    .GetRequiredService<ITelegramBotClient>();

IUpdateHandler updateHandler = scope.ServiceProvider
    .GetRequiredService<IUpdateHandler>();

// start listening for telegram updates
botClient.StartReceiving(updateHandler);

// Map telegram endpoints
app.MapTelegramEndpoints();

app.Run();
