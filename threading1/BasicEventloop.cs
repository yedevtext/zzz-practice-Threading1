using System;
using System.Threading;
using System.Threading.Tasks;

namespace threading1
{
    class BasicEventloop
    {
        /// <summary>
        /// 
        /// Code modified from here -> https://ayende.com/blog/167299/async-event-loops-in-c
        /// </summary>
        /// <param name="chatquery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task EventLoop(Chatquery chatquery, CancellationToken cancellationToken)
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


        /// <summary>
        /// 
        /// Code modified from here -> https://ayende.com/blog/167299/async-event-loops-in-c
        /// </summary>
        /// <param name="chatquery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static void EventLoop2(Chatquery chatquery, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
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
            }, cancellationToken);
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
}