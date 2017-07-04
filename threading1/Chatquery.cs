using System.Net.Http;
using System.Threading.Tasks;

namespace threading1
{
    public class Chatquery
    {
        private readonly HttpClient _httpClient;

        public Chatquery(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> CheckChat()
        {
            return await _httpClient.GetAsync("https://www.google.com/");
        }
    }
}