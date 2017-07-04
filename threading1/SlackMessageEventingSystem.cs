using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace threading1
{
    public class SlackMessageEventingSystem
    {
        //public event EventHandler<Task<HttpResponseMessage>> MessageArrived;

        public SlackMessageEventingSystem()
        {
            MessageArrived += OnMessageArrived;
        }

        public event Action<Task<HttpResponseMessage>> MessageArrived;


        private void OnMessageArrived(Task<HttpResponseMessage> message)
        {
            if (MessageArrived != null)
                MessageArrived(message);
        }

        /// <summary>
        /// 
        /// Code modified from here -> https://ayende.com/blog/167299/async-event-loops-in-c
        /// </summary>
        /// <param name="chatquery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void EventLoop(Chatquery chatquery, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).GetAwaiter().GetResult();

                    Task<HttpResponseMessage> message;
                    try
                    {
                        message = chatquery.CheckChat();
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
                    OnMessageArrived(message);
                }
            }, cancellationToken);
        }

        private void NoMessagesInTimeout()
        {
        }
    }
}