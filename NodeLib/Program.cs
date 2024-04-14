
using GraphTransferLibrary;
using GraphLibrary;

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

