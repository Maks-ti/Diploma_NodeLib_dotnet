

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using GraphTransferLibrary;
using Newtonsoft.Json;

namespace GraphLibrary;

public class Node
{
    // очередь команд (нужна для отправки команд в UI)
    [JsonIgnore]
    private readonly ConcurrentQueue<Command>? Queue;

    // Id узла
    public Guid Id { get; private set; }

    // список дочерних узлов
    [JsonIgnore]
    private List<Edge> outputEdges;

    // список входящих узлов
    [JsonIgnore]
    internal List<Edge> inputEdges;

    // свойство (извне список дочерних элементов изменить нельзя)
    [JsonIgnore]
    public ReadOnlyCollection<Edge> Childs
    {
        get
        {
            return outputEdges.AsReadOnly();
        }
    }

    // Имя узла
    private string name;

    // цвет в формате HEX
    private string color;

    // размер узла
    private double size;

    // факт выделености
    private bool selected;

    // Факт посещённости узла
    private int visited;

    // значение хранящееся в узле
    private object? value;


    // свойства
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            Queue.Enqueue(new Command<string>
            {
                NodeId = Id,
                CommandName = CommandType.SetName,
                Value = value
            });
        }
    }

    public string Color
    {
        get { return color; }
        set
        {
            color = value;
            Queue.Enqueue(new Command<string>
            {
                NodeId = Id,
                CommandName = CommandType.SetColor,
                Value = value
            });
        }
    }

    public double Size
    {
        get { return size; }
        set
        {
            size = value;
            Queue.Enqueue(new Command<double>
            {
                NodeId = Id,
                CommandName = CommandType.SetSize,
                Value = value
            });
        }
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            Queue.Enqueue(new Command<bool>
            {
                NodeId = Id,
                CommandName = CommandType.SetSelected,
                Value = selected
            });
        }
    }

    public int Visited
    {
        get { return visited; }
        set
        {
            visited = value;
            Queue.Enqueue(new Command<int>
            {
                NodeId = Id,
                CommandName = CommandType.SetVisited,
                Value = value
            });
        }
    }

    public object? Value
    {
        get { return value; }
        set
        {
            this.value = value;
            Queue.Enqueue(new Command<object>
            {
                NodeId = Id,
                CommandName = CommandType.SetValue,
                Value = value
            });
        }
    }

    // add child
    public void AddChild(Node child, Dictionary<string, object>? edgeParameters = null)
    {
        Edge newEdge = new(this, child, edgeParameters);

        // добавляем входящее ребро дочернему объекту
        child.inputEdges.Add(newEdge);

        // добавляем исходящее ребро из текущего объекта
        outputEdges.Add(newEdge);

        Queue?.Enqueue(new Command<Edge>
        {
            NodeId = this.Id,
            CommandName = CommandType.AddChild,
            Value = newEdge
        });
    }

    // delete child
    public void DeleteChild(Node child)
    {
        var edge = outputEdges.FirstOrDefault(e => e.NodeTo == child);

        if (edge == null)
        {
            throw new ArgumentException($"Node id = {child.Id} isn`t in childs of Node id = {this.Id}");
        }
        
        // удаляем входящее ребро для дочернего
        child.inputEdges.Remove(edge);
        // удаляем исходящее ребро из текущего
        outputEdges.Remove(edge);

        Queue?.Enqueue(new Command<Guid>
        {
            NodeId = this.Id,
            CommandName = CommandType.DeleteChild,
            Value = edge.Id
        });
    }

    /// <summary>
    /// возвращает child если найден
    /// null если не найден
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public Node? GetChild (Guid childId)
    {
        return outputEdges.FirstOrDefault(edge => edge.NodeTo.Id == childId)?.NodeTo;
    }

    // delete all input edges (usualy before delete this Node from NodePool [in NodeManager])
    private void DeleteAllInputEdges()
    {
        // удаляем ребро у родительских элементов (ToList что бы не было ошибок при изменении inputEdges в методе DeleteChild)
        foreach (var edge in inputEdges.ToList())
        {
            edge.NodeFrom.DeleteChild(this);
        }

        // по идее inputEdges на данный момент пуст, поэтому RemoveAll выполнять не будем
    }

    // delete all output edges
    private void DeleteAllOutputEdges()
    {
        // удаляем все дочерние рёбра (исходящим элементом ребра является текущий объект - поэтому this)
        foreach(var edge in outputEdges.ToList())
        {
            this.DeleteChild(edge.NodeTo);
        }

        // по идее outputEdges на данный момент пуст, потому что при удалении всех детей - все исходящие рбра были удалены
    }

    // delete this node
    internal void Delete()
    {
        // удаляем входящие и исходящие рёбра
        this.DeleteAllInputEdges();
        this.DeleteAllOutputEdges();

        Queue?.Enqueue(new Command<Guid>
        {
            NodeId = Id,
            CommandName = CommandType.Delete,
            Value = Id
        });
    }

    // constructor
    internal Node(ConcurrentQueue<Command>? queue, string name = "", string color = "", double size = default, 
        bool selected = false, int visited = default, object? value = default)
    {
        Queue = queue;
        Id = Guid.NewGuid();
        this.name = name;
        this.color = color;
        this.size = size;
        this.selected = selected;
        this.visited = visited;
        this.value = value;

        outputEdges = new();
        inputEdges = new();

        Queue?.Enqueue(new Command<Node>
        {
            NodeId = Id,
            CommandName = CommandType.Create,
            Value = this
        });
    }

    public override bool Equals(object? obj)
    {
        if (obj is Node other)
        {
            return this.Id.Equals(other.Id);
        }

        return base.Equals(obj);
    }

    public static bool operator ==(Node obj1, Node obj2) { return obj1.Equals(obj2); }

    public static bool operator != (Node obj1, Node obj2) {  return !obj1.Equals(obj2); }

}


