using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace API;

public class HttpClientService
{
    private HttpClient _httpClient;
    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    /*
     * sends a request to a google api to verify a token to see if a person is human
     */
    public async Task<bool> verifyHuman(string token)
    {
        var lookupaddress = "https://www.google.com/recaptcha/api/siteverify";
        var apiResponse = await _httpClient.PostAsync(lookupaddress, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("secret", Environment.GetEnvironmentVariable("resecret")),
            new KeyValuePair<string, string>("response", token)
        }));
        var responsebody = await apiResponse.Content.ReadAsStringAsync();
        var recaptchaResponce = JsonConvert.DeserializeObject<dynamic>(responsebody);
        if (recaptchaResponce.score <= 0.5)
        {
            return false;
        }

        return recaptchaResponce.success;
    }
}

