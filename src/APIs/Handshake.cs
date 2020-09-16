
namespace VCP.API {
    public class Handshake : APIBase
    {
        public enum ClientRole {
            Sink, Source, Control, Undefined, All
        }

        public override string Type { get => "handshake"; set {} }
        public ClientRole Role {get; set;}
    }

}