using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Treadmill.Domain.Services
{
    public interface IUdpService
    {
        DateTime LastMessage { get; }
        Task Serve(Action<string> callback, CancellationToken ct = default);
    }

    public class UdpService : IUdpService
    {
        public DateTime LastMessage { get; private set; } = DateTime.UtcNow;
        private IPEndPoint _remote;
        private readonly UdpClient _client;

        public UdpService(string ip, int port)
        {
            _remote = new IPEndPoint(IPAddress.Parse(ip), port);
            _client = new UdpClient(port) { EnableBroadcast = true };
        }

        public async Task Serve(Action<string> callback, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                var data = _client.Receive(ref _remote);
                LastMessage = DateTime.UtcNow;
                var message = System.Text.Encoding.UTF8.GetString(data);
                callback(message);
            }

            await Task.CompletedTask;
        }
    }
}
