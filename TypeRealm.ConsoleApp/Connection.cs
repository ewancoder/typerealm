using System;
using System.IO;
using System.Net.Sockets;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public sealed class Connection : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private readonly Authorize _authorizeMessage;
        private TcpClient _client;

        public Connection(string server, int port, Authorize authorizeMessage)
        {
            _server = server;
            _port = port;
            _authorizeMessage = authorizeMessage;
            _client = new TcpClient();
        }

        public Stream Stream { get; private set; }

        public object ReceiveMessage()
        {
            // TODO: Add pinging and reconnecting if connection is lost.
            return MessageSerializer.Read(Stream);
        }

        public void Send(object message)
        {
            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    // TODO: Add idempotency key.
                    MessageSerializer.Write(Stream, message);
                    return;
                }
                catch
                {
                    if (i == 5)
                        throw;

                    ReconnectAndAuthorize();
                }
            }
        }

        public void ReconnectAndAuthorize()
        {
            DisposeConnection();

            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_server, _port);
                    Stream = _client.GetStream();

                    Send(_authorizeMessage);

                    break;
                }
                catch
                {
                    DisposeConnection();
                }
            }
        }

        public void Dispose()
        {
            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}
