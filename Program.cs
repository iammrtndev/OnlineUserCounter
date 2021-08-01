using Fleck;

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace OnlineUserCounter
{
    static class Program
    {
        static void Main()
        {
            var socketsMap = new Dictionary<string, List<IWebSocketConnection>>();
            var server = new WebSocketServer("ws://0.0.0.0");
            //var server = new WebSocketServer("wss://0.0.0.0");
            //server.Certificate = new X509Certificate2("MyCert.pfx", "password");
            server.Start(socket =>
            {
                var protocol = socket.ConnectionInfo.SubProtocol;
                socketsMap.TryAdd(protocol, new List<IWebSocketConnection>());
                var sockets = socketsMap[protocol];
                socket.OnOpen = () =>
                {
                    sockets.Add(socket);
                    SendMessageToSockets(sockets, sockets.Count.ToString());
                };
                socket.OnClose = () =>
                {
                    sockets.Remove(socket);
                    SendMessageToSockets(sockets, sockets.Count.ToString());

                };
            });
            Console.ReadKey();
        }
        static void SendMessageToSockets(IEnumerable<IWebSocketConnection> sockets, string message)
        {
            foreach (var socket in sockets)
            {
                socket.Send(message);
            }
        }
    }
}