using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using VCP.API;

/// <summary>VirtualControlProtocol</summary>
namespace VCP.API
{
    /// <summary>
    /// Listener class for SinkClient
    /// </summary>
    public abstract class APISinkListener
    {
        public abstract void ReceiveJSON(string jsonString);
    }
}