using System;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using VCP.API;
using System.Linq;

/// <summary>VirtualControlProtocol</summary>
namespace VCP.WebSocket
{
    /// VCP.WebSocket.RelayServer
    /// リレーサーバーの通信仕様は仕様書を参照
    public class RelayServer : WebSocketBehavior
    {
        public static bool DebugMode {get; set;}
        protected static LinkedList<RelayServer> Connections = new LinkedList<RelayServer>();

        private Handshake.ClientRole Role {get; set;} = Handshake.ClientRole.Undefined;

        protected override void OnOpen()
        {
            Connections.AddLast(this);
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Connections.Remove(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            // リクエストヘッダを見てパケットを転送するか，ルートを構築するかを決定する
            if (e.IsText) {
                if (DebugMode) {
                    Console.WriteLine($"text received: {e.Data}");
                }

                var api = JsonCompatible.FromJson<APIBase>(e.Data);
                switch (api.Type)
                {
                    case "handshake":
                    {
                        var hs = JsonCompatible.FromJson<Handshake>(e.Data);
                        Role = hs.Role;

                        if (DebugMode) {
                            Console.WriteLine($"handshaked: {Role}");
                        }
                    }
                    break;
                    default:
                    Relay(e);
                    break;
                }
            }
        }

        void Relay(MessageEventArgs e) {
            // filter then relay
            foreach(var c in Connections.Where((c) => c.Role == Handshake.ClientRole.Sink)) {
                c.Send(e.Data);
            }
        }
    }

    /// VCP.WebSocket.SourceClient
    /// 主に情報を発信するクライアント
    public class SourceClient : Client
    {
        public SourceClient(string url) : base(url, new Handshake() {Role = Handshake.ClientRole.Source})
        {
        }

        public void Send(JsonCompatible api)
        {
            socket.Send(api.ToJson());
        }

        protected override void OnMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    // Dictionary<string, string->void> でイベントハンドラ実装，登録
    // 型が実装窓口を用意する，OR，GetAPINameを利用して，型とファサードを利用する


    /// <summary>
    /// Dispatcher for SinkClient
    /// </summary>
    public class SinkClient : Client
    {
        Dictionary<string, List<ISinkProcessor>> processorMap;
        /// <summary>
        /// 同期的実行のためのキュー
        /// </summary>
        Queue<Action> processQueue;

        /// <summary>
        /// メインスレッドで同期的に実行するか?
        /// </summary>
        /// <value></value>
        public bool ProcessSync {
            get => _processSync;
            set {
                _processSync = value;
            }
        }

        bool _processSync;

        public SinkClient(string url) : base(url, new Handshake() { Role = Handshake.ClientRole.Sink })
        {
            processorMap = new Dictionary<string, List<ISinkProcessor>>();
            processQueue = new Queue<Action>();
        }

        public void AddProcessor(ISinkProcessor processor)
        {
            var apiname = processor.GetSupportedAPIName();
            if (processorMap.ContainsKey(apiname)) {
                processorMap[apiname].Add(processor);
            }
            else {
                processorMap.Add(apiname, new List<ISinkProcessor>());
                // no recursion
                processorMap[apiname].Add(processor);
            }
        }

        protected override void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText) {
                string typeName = "";

                // 頭から読み出し，type プロパティを探す
                var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(
                    Encoding.UTF8.GetBytes(e.Data)));
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonTokenType.PropertyName
                        && jsonReader.GetString() == "type")
                    {
                        if (jsonReader.Read()
                            && jsonReader.TokenType == JsonTokenType.String)
                        {
                            typeName = jsonReader.GetString();
                            break;
                        }
                        else {
                            break;
                        }
                    }
                }

                // パースが2回行われるという無駄がある
                // ここでディスパッチ
                if (processorMap.TryGetValue(typeName, out var processors)) {
                    foreach (var p in processors)
                    {
                        if (ProcessSync) {
                            processQueue.Enqueue(() => p.ProcessJson(e.Data));
                        }
                        else {
                            p.ProcessJson(e.Data);
                        }
                    }
                }
            }
            else if (e.IsBinary) {
                // MessagePack の解析
                // Union Type でスイッチ+ビルド
                // 実行時型を取得し，それでスイッチ
                // ProcessNative でさらにキャストするのでますます遅くなる
                // 速さよりサイズといったところ
                var apibase = new APIBase(); //MessagePackSerializer.Deserialize<APIBase>(e.RawData);
                var apiName = APINameRegister.Query(apibase.GetType());
                if (processorMap.TryGetValue(apiName, out var processors)) {
                    foreach (var p in processors)
                    {
                        if (ProcessSync) {
                            processQueue.Enqueue(() => p.ProcessNative(apibase));
                        }
                        else {
                            p.ProcessNative(apibase);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// キューを消化する
        /// </summary>
        public void ProcessQueue()
        {
            while (processQueue.Count != 0)
            {
                processQueue.Dequeue()();
            }
        }
    }

    /// VCP.WebSocket.ControlClient
    /// 制御情報をやり取りするクライアントベース
    /// CommandAPI (ControlAPI) のプロトコルに対応
    public class ControlClient
    {
        protected virtual void OnMessage(object sender, MessageEventArgs e)
        {

        }
    }

    /// VCP.WebSocket.Client
    /// クライアント用のカスタムクラス
    public abstract class Client : IDisposable
    {
        protected WebSocketSharp.WebSocket socket;

        public Client(string url, Handshake hs)
        {
            socket = new WebSocketSharp.WebSocket(url);
            socket.OnMessage += OnMessage;
            socket.OnOpen += (sender, e) => {
                socket.Send(hs.ToJson());
            };
            socket.Connect();
        }

        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// オーバーライドする場合は必ずベースクラスのメソッドも呼ぶこと
        /// Must call base.Dispose() when override
        /// </summary>
        public virtual void Dispose()
        {
            socket.Close();
        }

        ~Client()
        {
            Dispose();
        }

        protected abstract void OnMessage(object sender, MessageEventArgs e);
    }
}