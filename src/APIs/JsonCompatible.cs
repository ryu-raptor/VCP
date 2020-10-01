using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    /// <summary>
    /// Base class of JsonCompatible API Suite
    /// </summary>
    public abstract class JsonCompatible
    {
        public byte[] ToByteJson()
        {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this, this.GetType(), options));
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return JsonSerializer.Serialize(this, this.GetType(), options);
        }

        /// <summary>
        /// Get API name for VCP JSON
        /// </summary>
        /// <returns>camelCase API name</returns>
        public abstract string Type { get; set; }

        public static TDerived FromJson<TDerived>(string jsonString)
        {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            
            return JsonSerializer.Deserialize<TDerived>(jsonString, options);
        }
    }
}