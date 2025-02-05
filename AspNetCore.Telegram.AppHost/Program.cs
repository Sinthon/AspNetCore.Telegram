using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin();

var telegram = builder.AddProject<Projects.AspNetCore_Telegram>("aspnetcore-telegram")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
