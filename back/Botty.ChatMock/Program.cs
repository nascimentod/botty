using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddHttpClient();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
    })
    .Build();

var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
var port = 5212;

Console.WriteLine($"Hello there! At which port is the backend exposed ({port} default)?");
var portValue = Console.ReadLine();
if(!string.IsNullOrWhiteSpace(portValue)) {
    port = int.Parse(portValue);
}

var client = httpClientFactory.CreateClient();
Console.Write("Attempting connection...");
var healthCheckResponse = await client.GetAsync($"http://localhost:{port}/healthcheck");

if(!healthCheckResponse.IsSuccessStatusCode)
{
    Console.WriteLine("Could not establish connection. Exiting.");
    return;
}

Console.WriteLine("Connection established!");

while(true)
{
    Console.Write("Message: ");
    var message = Console.ReadLine();

    var outgoingMessage = new OutgoingMessage("ChatMock", message ?? string.Empty);
    var jsonContent = JsonSerializer.Serialize(outgoingMessage);
    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    var response = await client.PostAsync($"http://localhost:{port}/message", stringContent);
    if(!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Error sending message. Exiting.");
    }

    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Response: {responseContent}");
}

record OutgoingMessage(string UserId, string Message) {}