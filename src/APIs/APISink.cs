using System;

/// <summary>VirtualControlProtocol</summary>
namespace VCP.API
{
    public interface ISinkProcessor
    {
        /// <summary>
        /// このプロセッサが取り扱うAPI名を取得
        /// </summary>
        /// <returns></returns>
        string GetSupportedAPIName();
        void ProcessJson(string jsonString);
        void ProcessNative(APIBase source);
    }

    /// <summary>
    /// APIを受信するクライアント
    /// </summary>
    /// <typeparam name="T">クライアントが取り扱うAPI</typeparam>
    public abstract class APISink<T> : ISinkProcessor
        where T : APIBase, new()
    {
        /// <summary>
        /// JSON をネイティブ API に変換したものを受信する
        /// </summary>
        /// <param name="message"></param>
        public abstract void OnMessage(T message);

        public void ProcessJson(string jsonString)
        {
            OnMessage(JsonCompatible.FromJson<T>(jsonString));
        }

        public void ProcessNative(APIBase source)
        {
            if (source is T target)
            {
                OnMessage(target);
            }
            else {
                throw new Exception("Dispatch Error");
            }
        }

        public string GetSupportedAPIName()
        {
            return APINameRegister.Query<T>();
        }
    }
}