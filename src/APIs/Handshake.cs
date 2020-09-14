
namespace VCP.API {
    public class Handshake : JsonCompatible
    {
        public enum ClientRole {
            Sink, Source, Undefined, All
        }

        public override string Type { get => "handshake"; set {} }
        public ClientRole Role {get; set;}
    }

}