using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventListener
{
    class Program
    {


        private static string _baseAddress = "http://localhost:9000/";
        private static string _eventRaisedUrl = "http://localhost:8080/SlackEvent/";
        private static string _eventDisplayedUrl = "http://localhost:8080/SlackEventDisplay/";

        private static HttpClient _httpClient = new HttpClient();
        private static volatile string _eventObject;

        static void Main(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("Event Raised");
                    var httpContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes($"Hello World! {new Random().Next()}"));
                    _httpClient.PostAsync(_eventRaisedUrl, httpContent).GetAwaiter().GetResult();
                }
            });



            Task.Run(() =>
            {
                // Create a listener.
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(_eventRaisedUrl);
                listener.Prefixes.Add(_eventDisplayedUrl);
                listener.Start();
                Console.WriteLine("Listening...");
                while (true)
                {
                    
                    HttpListenerContext context = listener.GetContext();
                    
                    
                    if (context.Request.RawUrl == Regex.Split(_eventRaisedUrl, "8080")[1])
                    {
                        Console.WriteLine("Event Handled");
                        HttpListenerResponse response = context.Response;

                        _eventObject = ShowRequestData(context.Request);

                        response.StatusCode = (int) HttpStatusCode.OK;

                        response.Close();

                        //response.RedirectLocation = _eventDisplayedUrl;
                        //// Construct a response.
                        //string responseString = $"<HTML><BODY>{ShowRequestData(context.Request)}</BODY></HTML>";

                        //Console.WriteLine(responseString);

                        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        //// Get a response stream and write the response to it.
                        //response.ContentLength64 = buffer.Length;
                        //System.IO.Stream output = response.OutputStream;
                        //output.Write(buffer, 0, buffer.Length);
                        //// You must close the output stream.
                        //output.Close();
                    }
                    if (context.Request.RawUrl == Regex.Split(_eventDisplayedUrl, "8080")[1])
                    {
                        Console.WriteLine("Event Handled");
                        HttpListenerResponse response = context.Response;

                        response.RedirectLocation = _eventDisplayedUrl;
                        // Construct a response.
                        string responseString = $"<HTML><BODY>{_eventObject}</BODY></HTML>";

                        Console.WriteLine(responseString);

                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        // You must close the output stream.
                        output.Close();
                    }

                }
                
                // Shut down listener
                listener.Stop();
            });

           Console.ReadKey();

            //while (true)
            //{
            //    // Note: The GetContext method blocks while waiting for a request. 
            //    HttpListenerContext context = listener.GetContext();



            //    //HttpListenerRequest request = context.Request;
            //    // Obtain a response object.
            //    HttpListenerResponse response = context.Response;

            //    // Construct a response.
            //    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            //    // Get a response stream and write the response to it.
            //    response.ContentLength64 = buffer.Length;
            //    System.IO.Stream output = response.OutputStream;
            //    output.Write(buffer, 0, buffer.Length);
            //    // You must close the output stream.
            //    output.Close();
            //}



        }

        public static string ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return "No client data was sent with the request.";
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            //if (request.ContentType != null)
            //{
            //    Console.WriteLine("Client data content type {0}", request.ContentType);
            //}
            //Console.WriteLine("Client data content length {0}", request.ContentLength64);

            //Console.WriteLine("Start of client data:");
            // Convert the data to a string and display it on the console.
            string s = reader.ReadToEnd();
            //Console.WriteLine(s);
            //Console.WriteLine("End of client data:");
            body.Close();
            reader.Close();
            return s;
            // If you are finished with the request, it should be closed also.
        }
    }
}
