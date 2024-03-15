using System;
using System.Net.Http;
using System.Threading.Tasks;

public class TypeThreadRequest
{
    private HttpClient _client;

    public async Task SendRequest()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://typethread.com");
        var content = new StringContent("Hello world");
        await _client.PostAsync("/post", content);
    }
}