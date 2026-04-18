using System.Net.WebSockets;

namespace Ledqualizer
{
    internal enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Faulted
    }

    internal sealed class DeviceSession
    {
        private readonly DeviceConfig device;
        private readonly Action<string, ConnectionState, string?> statusCallback;
        private ClientWebSocket? webSocket;

        public DeviceSession(DeviceConfig device, Action<string, ConnectionState, string?> statusCallback)
        {
            this.device = device;
            this.statusCallback = statusCallback;
        }

        public ConnectionState Status { get; private set; } = ConnectionState.Disconnected;

        public async Task<bool> ConnectAsync(CancellationToken token)
        {
            SetStatus(ConnectionState.Connecting, null);
            webSocket = new ClientWebSocket();

            try
            {
                Uri serverUri = new Uri($"ws://{device.Host}:{device.Port}");
                await webSocket.ConnectAsync(serverUri, token).ConfigureAwait(false);
                SetStatus(ConnectionState.Connected, null);
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                DisposeSocket();
                SetStatus(ConnectionState.Faulted, ex.Message);
                return false;
            }
        }

        public async Task<bool> SendFrameAsync(byte[] frame, CancellationToken token)
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
            {
                return false;
            }

            try
            {
                await webSocket.SendAsync(new ArraySegment<byte>(frame), WebSocketMessageType.Binary, true, token).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                DisposeSocket();
                SetStatus(ConnectionState.Faulted, ex.Message);
                return false;
            }
        }

        public async Task DisconnectAsync(CancellationToken token)
        {
            try
            {
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested closure", token).ConfigureAwait(false);
                }
            }
            catch
            {
                // Best effort disconnect.
            }
            finally
            {
                DisposeSocket();
                SetStatus(ConnectionState.Disconnected, null);
            }
        }

        private void SetStatus(ConnectionState state, string? detail)
        {
            Status = state;
            statusCallback(device.Id, state, detail);
        }

        private void DisposeSocket()
        {
            webSocket?.Dispose();
            webSocket = null;
        }
    }

    internal sealed class DeviceTarget
    {
        public DeviceTarget(DeviceConfig config, DeviceSession session)
        {
            Config = config;
            Session = session;
        }

        public DeviceConfig Config { get; }
        public DeviceSession Session { get; }
    }

    internal interface ISceneRunner
    {
        Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token);
    }

    internal sealed class RunController
    {
        private readonly object syncRoot = new();
        private CancellationTokenSource? runCts;
        private Task? runTask;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

        public event Action<string, ConnectionState, string?>? DeviceStatusChanged;

        public bool IsRunning
        {
            get
            {
                lock (syncRoot)
                {
                    return runTask != null && !runTask.IsCompleted;
                }
            }
        }

        public async Task StartAsync(IReadOnlyList<DeviceConfig> devices, ISceneRunner runner)
        {
            await StopAsync().ConfigureAwait(false);

            var cts = new CancellationTokenSource();
            Task newRunTask = Task.Run(() => RunInternalAsync(devices, runner, cts.Token));

            lock (syncRoot)
            {
                runCts = cts;
                runTask = newRunTask;
            }
        }

        public async Task StopAsync()
        {
            CancellationTokenSource? cts;
            Task? task;

            lock (syncRoot)
            {
                cts = runCts;
                task = runTask;
                runCts = null;
                runTask = null;
            }

            if (cts == null || task == null)
            {
                return;
            }

            try
            {
                cts.Cancel();
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                cts.Dispose();
            }
        }

        private async Task RunInternalAsync(IReadOnlyList<DeviceConfig> devices, ISceneRunner runner, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var activeTargets = new List<DeviceTarget>();

                try
                {
                    foreach (DeviceConfig device in devices)
                    {
                        var session = new DeviceSession(device, OnDeviceStatusChanged);
                        bool connected = await session.ConnectAsync(token).ConfigureAwait(false);
                        if (connected)
                        {
                            activeTargets.Add(new DeviceTarget(device, session));
                        }
                    }

                    if (activeTargets.Count > 0)
                    {
                        await runner.RunAsync(activeTargets, token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                finally
                {
                    foreach (DeviceTarget target in activeTargets)
                    {
                        await target.Session.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                }

                if (!token.IsCancellationRequested)
                {
                    await Task.Delay(RetryDelay, token).ConfigureAwait(false);
                }
            }
        }

        private void OnDeviceStatusChanged(string deviceId, ConnectionState state, string? detail)
        {
            DeviceStatusChanged?.Invoke(deviceId, state, detail);
        }
    }
}
