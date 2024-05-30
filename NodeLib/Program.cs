
using GraphTransferLibrary;
using GraphLibrary;

/*
Console.WriteLine("*** before create nodeManager handler");

// graph 1
using Graph manager = new("http://localhost:9090/");

Console.WriteLine("*** after create nodeManager handler");

Node node1 = manager.CreateNode("node 1");
node1.Name = "Test";

Node node2 = manager.CreateNode("node 2");
node2.Name = "Test";

var dict = new Dictionary<string, object>() 
{
    { "key1", "value1" },
    { "key2", "value2" }
}; 

node1.AddChild(node2, dict);

Node node3 =  manager.CreateNode("node 3");

var childs = node1.Childs;
foreach (Edge edge in childs)
{
    edge.Parameters = dict;
}


var input =  Console.ReadLine();


node2.AddChild(node3);

manager.DeleteNode(node1);


// graph 2

using Graph manager_2 = new("http://localhost:9090/");

Node node_2_1 = manager_2.CreateNode("node 2_1");
Node node_2_2 = manager_2.CreateNode("node 2_2");
Node node_2_3 = manager_2.CreateNode("node 2_3");
Node node_2_4 = manager_2.CreateNode("node 2_4");
Node node_2_5 = manager_2.CreateNode("node 2_5");
*/


Test_DFS();


static void Test_DFS()
{
    using Graph graph = new("http://localhost:9090/");

    // первая компонента связности
    var node1 = graph.CreateNode("node 1", "#0ff", 30, value: 12);
    var node2 = graph.CreateNode("node 2", "#00f", 25, value: 11);
    var node3 = graph.CreateNode("node 3", "#f00", 20, value: 5);
    var node4 = graph.CreateNode("node 4", "#00f", 15, value: 3);
    var node5 = graph.CreateNode("node 5", "#f00", 22, value: 1);
    var node6 = graph.CreateNode("node 6", "#990", 35, value: 75);

    node1.AddChild(node2);
    node1.AddChild(node3);
    
    node2.AddChild(node5);
    node2.AddChild(node6);

    node3.AddChild(node6);

    node4.AddChild(node1);

    node5.AddChild(node6);

    node6.AddChild(node4);

    // вторая компонента связности

    var node7 = graph.CreateNode("node 7", "#ff0", 22, value: 18);
    var node8 = graph.CreateNode("node 8", "#aaa", 23, value: 17);
    var node9 = graph.CreateNode("node 9", "#888", 42, value: 15);
    var node10 = graph.CreateNode("node 10", "#00f", 12, value: 11);

    node7.AddChild(node8);
    node7.AddChild(node9);

    node8.AddChild(node10);

    node9.AddChild(node10);

    // граф определён

    // запускаем поиск в глубину на графе
    DFS(graph, node1.Id);

    return;
}


static void DFS(Graph graph, Guid startNodeId)
{
    bool firstIteration = true;
    do
    {
        Node nodeToRun;
        if (firstIteration)
        {
            firstIteration = false;
            nodeToRun = graph.GetNodeById(startNodeId);
        }
        else
        {
            nodeToRun = graph.AllNodes.First(node => node.Visited == 0);
        }

        DFS_visit(nodeToRun);
                
    } while (graph.AllNodes.Where(node => node.Visited != 0).Count() < graph.AllNodes.Count()); // пока не посещены все вершины

    return;
}


static void DFS_visit(Node node)
{
    Console.WriteLine($"node: {node.Name}");
    Console.ReadLine();

    node.Visited = 1;  // отмечаем узел как посещённый
    node.Color = "#0f0"; // для наглядности работы алгоритма помечаем цветом посещённый узел

    // обход по всем дочерним узлам
    foreach (var child in node.Childs)
    {
        Node childNode = child.NodeTo;
        if (childNode.Visited == 0)
        {
            DFS_visit(childNode);
        }
    }
}
