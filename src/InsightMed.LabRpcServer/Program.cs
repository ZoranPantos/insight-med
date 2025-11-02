using InsightMed.LabRpcServer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<ILabDbService, LabDbService>();

builder.Services.AddHostedService<RpcServerWorker>();

var host = builder.Build();
await host.RunAsync();