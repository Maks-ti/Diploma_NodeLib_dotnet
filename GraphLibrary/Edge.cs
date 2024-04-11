
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GraphLibrary;

public class Edge
{
    // узлы в связи
    [JsonIgnore]
    public Node NodeFrom { get; protected set; }

    [JsonIgnore]
    public Node NodeTo { get; protected set; }
    
    // Id ребра
    public Guid Id { get; protected set; }

    // Id узлов в связи
    public Guid NodeFromId { get; protected set; }
    public Guid NodeToId { get; protected set; }

    // Параметры связи
    public Dictionary<string, object> Parameters { get; protected set; }

    internal Edge(Node From, Node To, Dictionary<string, object>? parameters = null)
    {
        Id = Guid.NewGuid();

        NodeFrom = From;
        NodeTo = To;

        NodeFromId = From.Id;
        NodeToId = To.Id;

        Parameters = (parameters == null) ? new Dictionary<string, object>() : parameters;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Edge other)
        {
            return this.NodeFromId.Equals(other.NodeFromId)
                && this.NodeToId.Equals(other.NodeToId);
        }

        return base.Equals(obj);
    }

}
