using PlatformService.Dtos;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;


namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }
    public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platformReadDto),
            Encoding.UTF8,
            "application/json"
        );
        var response = await _client.PostAsync($"{_configuration["CommandService"]}/api/c/Platforms", httpContent);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync POST to command service was OK!");
        }
        else
        {
            Console.WriteLine("--> Sync POST to CommandService was NOT OK");
        }

    }
}