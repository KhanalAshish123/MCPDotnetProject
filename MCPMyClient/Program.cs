using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// LoggerFactory
builder.Services.AddSingleton<ILoggerFactory>(sp =>
{
    return LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    });
});

// MCP Client
builder.Services.AddSingleton<IMcpClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    var clientOptions = new McpClientOptions
    {
        ClientInfo = new() { Name = "demo-client", Version = "1.0.0" }
    };

    var serverConfig = new McpServerConfig
    {
        Id = "demo-server",
        Name = "Demo Server",
        TransportType = TransportTypes.StdIo,
        TransportOptions = new Dictionary<string, string>
        {
            ["command"] = @"D:\FinalMCPComplete\MCPMyServer\bin\Debug\net8.0\MCPMyServer.exe"
        }
    };

    return McpClientFactory.CreateAsync(serverConfig, clientOptions, loggerFactory: loggerFactory).GetAwaiter().GetResult();
});


builder.Services.AddSingleton<IChatClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    var ollamaChatClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2:3b");

    var chatClient = new ChatClientBuilder(ollamaChatClient)
        .UseLogging(loggerFactory)
        .UseFunctionInvocation()
        .Build();

    return (IChatClient)chatClient;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
