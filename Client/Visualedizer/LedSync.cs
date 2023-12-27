using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Ledqualizer
{
    internal class LedSync
    {
        private static LedSync instance;

        private static readonly object lockObject = new object();

        public readonly Config config;

        private ClientWebSocket webSocket;

        public LedSync(Config config) {
            this.config = config;
        }

        public static LedSync GetInstance(Config config)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new LedSync(config);
                    }
                }
            }
            return instance;
        }

        public async Task ConnectAsync()
        {
            webSocket = new ClientWebSocket();
            Uri serverUri = new Uri($"ws://{config.ipAddress}:{config.port}");

            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to the server");
        }

        public async Task SendDataAsync(byte[] data)
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    ArraySegment<byte> dataToSend = new ArraySegment<byte>(data);
                    await webSocket.SendAsync(dataToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sending data failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Connection is not open");
            }
        }

        public async Task DisconnectAsync()
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested closure", CancellationToken.None);
                    Console.WriteLine("Disconnected from the server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to disconnect: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Connection is not open");
            }
        }
    }
}
