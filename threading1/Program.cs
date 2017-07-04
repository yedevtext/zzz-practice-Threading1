using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace threading1
{
    class Program : IDisposable
    {
        private static HttpClient _httpClient;

        private static string _baseAddress = "http://localhost:9000/";
        private IDisposable _webApp;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            try
            {
                _httpClient = new HttpClient();
                _webApp = WebApp.Start<WebAppStartup>(_baseAddress);
                var slackMessageEventingSystem = new SlackMessageEventingSystem();
                var chatquery = new Chatquery(_httpClient);
                var cancellationTokenSource = new CancellationTokenSource();
                slackMessageEventingSystem.MessageArrived += SlackMessageEventingSystem_MessageArrived;

                await slackMessageEventingSystem.EventLoop(chatquery, cancellationTokenSource.Token);
                
                //BasicEventloop.EventLoop2(chatquery, cancellationTokenSource.Token);
                ////await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
                ////await BasicEventloop.EventLoop(chatquery, cancellationTokenSource.Token);
                /// 
                
                await Task.Delay(-1, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
            }
            finally
            {
                Dispose();
            }
        }

        private void SlackMessageEventingSystem_MessageArrived(object sender, Task<HttpResponseMessage> e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _webApp?.Dispose();
            _httpClient?.Dispose();
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