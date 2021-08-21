using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ravencoin.ApplicationCore.Models
{
    public class ServerCommand
    {
        [JsonProperty(PropertyName = "id")]
        public string commandId { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string commandMethod { get; set; }

        [JsonProperty(PropertyName = "params")]
        public JObject commandParams { get; set; }

        [JsonProperty(PropertyName = "jsonrpc")]
        public string commandJsonRpc { get; set; }
    }
}
