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

            await EventLoop(chatquery, cancellationTokenSource.Token);

        }

        /// <summary>
        /// 
        /// Code modified from here -> https://ayende.com/blog/167299/async-event-loops-in-c
        /// </summary>
        /// <param name="chatquery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task EventLoop(Chatquery chatquery, CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                object msg;
                try
                {
                    msg = await chatquery.CheckChat();
                }
                catch (TimeoutException)
                {
                    NoMessagesInTimeout();
                    continue;
                }
                catch (Exception e)
                {
                    break;
                }
                ProcessMessage(msg);
            }
        }

        private static void ProcessMessage(object msg)
        {
            Console.WriteLine("Processing message...");
        }

        private static void NoMessagesInTimeout()
        {
            Console.WriteLine("No message to process...");
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
