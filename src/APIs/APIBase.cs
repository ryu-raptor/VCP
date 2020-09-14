using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    /// <summary>
    /// Base class of API Suite
    /// </summary>
    public class APIBase : JsonCompatible
    {
        /// <summary>
        /// Do Nothing
        /// </summary>
        public APIBase()
        {}

        // Hided
        // Uses only for WebSocket.RelayServer
        private string _apiType = "undefined";

        // Base API type for relaying
        public override string Type {
            get {
                return _apiType;
            }
            set {
                _apiType = value;
            }
        }
    }
}
