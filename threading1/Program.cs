using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threading1
{
    class Program
    {
        private static HttpClient _httpClient;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _httpClient = new HttpClient();
            var chatquery = new Chatquery(_httpClient);
            var cancellationTokenSource = new CancellationTokenSource();

            BasicEventloop.EventLoop2(chatquery, cancellationTokenSource.Token);
            //await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
            //await BasicEventloop.EventLoop(chatquery, cancellationTokenSource.Token);
            await Task.Delay(-1,cancellationTokenSource.Token);
        }
    }

    public class Chatquery
    {
        private HttpClient _httpClient;

        public Chatquery(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Task<HttpResponseMessage>> CheckChat()
        {
            return _httpClient.GetAsync("https://www.google.com/");
        }

    }
}
