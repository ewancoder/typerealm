﻿using System;
using System.IO;

namespace TypeRealm.Server
{
    internal sealed class TcpClientListenerFactory : IClientListenerFactory
    {
        private readonly ILogger _logger;

        public TcpClientListenerFactory(ILogger logger)
        {
            _logger = logger;
        }

        // Connection handler will be called for every new connection in separate thread.
        public IDisposable StartListening(int port, Action<Stream> streamHandler)
        {
            return new TcpClientListener(port, streamHandler, _logger);
        }
    }
}