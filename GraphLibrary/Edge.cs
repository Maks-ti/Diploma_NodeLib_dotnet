
using GraphTransferLibrary;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GraphLibrary;

public class Edge
{
    // очередь команд (нужна для отправки команд в UI)
    [JsonIgnore]
    private readonly ConcurrentQueue<Command>? Queue;

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
    private Dictionary<string, object> parameters;
    public Dictionary<string, object> Parameters 
    {
        get
        {
            return parameters;
        }
        set
        {
            parameters = value;
            Queue?.Enqueue(new Command
            {
                ObjId = Id,
                CommandName = CommandType.SetEdgeParametres,
                Value = value
            });
        } 
    }

    internal Edge(Node From, Node To, Dictionary<string, object>? parameters = null, ConcurrentQueue<Command>? queue = null)
    {
        this.Queue = queue;

        Id = Guid.NewGuid();

        NodeFrom = From;
        NodeTo = To;

        NodeFromId = From.Id;
        NodeToId = To.Id;

        this.parameters = (parameters == null) ? new Dictionary<string, object>() : parameters;
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
