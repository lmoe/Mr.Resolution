using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using Swan.Logging;
using System;

namespace Mr._Resolution
{
    class Program
    {
        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o 
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/display", m => m.WithController<DisplayController>())
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }

        static void Main(string[] args)
        {
            var url = "http://*:9696/";

            if (args.Length > 0)
                url = args[0];

            using (var server = CreateWebServer(url))
            {
                server.RunAsync();

                Console.ReadLine();
            }
        }
    }
}
