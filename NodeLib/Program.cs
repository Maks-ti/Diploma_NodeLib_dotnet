
using GraphTransferLibrary;
using GraphLibrary;

Console.WriteLine("*** before create nodeManager handler");

// graph 1
using NodeManager manager = new("http://localhost:9090/");

Console.WriteLine("*** after create nodeManager handler");

Node node1 = manager.CreateNode();

Node node2 = manager.CreateNode();

node1.AddChild(node2);

Node node3 =  manager.CreateNode();

var input =  Console.ReadLine();

node2.AddChild(node3);

manager.DeleteNode(node1);


// graph 2

using NodeManager manager_2 = new("http://localhost:9090/");

Node node_2_1 = manager_2.CreateNode();
Node node_2_2 = manager_2.CreateNode();
Node node_2_3 = manager_2.CreateNode();
Node node_2_4 = manager_2.CreateNode();
Node node_2_5 = manager_2.CreateNode();

