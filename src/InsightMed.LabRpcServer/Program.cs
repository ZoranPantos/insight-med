using InsightMed.LabRpcServer;
using InsightMed.LabRpcServer.Options;
using InsightMed.LabRpcServer.Services;
using InsightMed.LabRpcServer.Services.Abstractions;

try
{
    Console.Title = "InsightMed.LabRpcServer";
}
catch { }

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<ILabDbService, LabDbService>();
builder.Services.AddTransient<IParameterValueRandomizerService, ParameterValueRandomizerService>();

builder.Services.AddHostedService<RpcServerWorker>();

var host = builder.Build();
await host.RunAsync();