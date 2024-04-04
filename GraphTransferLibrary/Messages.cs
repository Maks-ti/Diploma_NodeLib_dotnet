
using Newtonsoft.Json;

namespace GraphTransferLibrary;

public class RpcRequest
{
    [JsonProperty("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonProperty("command")]
    public Command Command { get; set; }

    [JsonProperty("manager_id")]
    public Guid ManagerId { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public class RpcResponse
{
    [JsonProperty("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonProperty("result")]
    public object Result { get; set; }

    [JsonProperty("error")]
    public object Error { get; set; }
}
