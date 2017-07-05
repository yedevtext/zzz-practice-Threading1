using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;

namespace threading1
{
    class Program : IDisposable
    {
        private static HttpClient _httpClient;

        private static string _baseAddress = "http://localhost:9000/";
        private ClientWebSocket _clientWebSocket;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            try
            {
                _httpClient = new HttpClient();
                _clientWebSocket = new ClientWebSocket();

                var slackMessageEventingSystem = new SlackMessageEventingSystem();
                var chatquery = new Chatquery(_httpClient);
                var cancellationTokenSource = new CancellationTokenSource();
                slackMessageEventingSystem.MessageArrived += SlackMessageEventingSystemOnMessageArrived;

                slackMessageEventingSystem.EventLoop(chatquery, cancellationTokenSource.Token);

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


        private async void SlackMessageEventingSystemOnMessageArrived(HttpResponseMessage httpResponseMessage)
        {
            string message = httpResponseMessage.ToString();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }

        //private void SlackMessageEventingSystemOnMessageArrived(object sender, Task<HttpResponseMessage> task)
        //{
        //    throw new NotImplementedException();
        //}

        //private void SlackMessageEventingSystem_MessageArrived(object sender, Task<HttpResponseMessage> e)
        //{
        //    string message = e;
        //    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        //    await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        //}

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}