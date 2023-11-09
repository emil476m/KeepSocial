using Newtonsoft.Json;

namespace test;

public class Tests
{
    private string apirUrl = "";
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        apirUrl = "http://localhost:5000/";
    }

    [Test]
    [TestCase(2, true)]
    [TestCase(5, false)]
    public async Task Test1(int number, bool result)

    {
        var httpRespnose = await _httpClient.GetAsync(apirUrl + "IsEven" + number);

        httpRespnose.IsSuccessStatusCode.Equals(result);
    }
}