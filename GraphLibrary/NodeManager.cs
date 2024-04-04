using GraphTransferLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLibrary;

public class NodeManager : IDisposable
{
    private readonly ConcurrentQueue<Command> _queue; // очередь комманд
    private readonly List<Node> _nodePool;
    private readonly HttpClient _httpClient;
    private string _senderEndpoint; // url
    private Guid _managerId; // Id NodeManager`а
    private readonly Task _queueHandlerTask; // задача обработки очереди
    private CancellationTokenSource cancellationTokenSource; // cancelation token


    public void Dispose()
    {
        // Добавляем небольшую задержку перед отменой, чтобы убедиться, что все команды добавлены
        Task.Delay(100).Wait(); // Задержка в 10 мс

        cancellationTokenSource.Cancel();
        _queueHandlerTask.Wait();

        _httpClient.Dispose();
        cancellationTokenSource.Dispose();

        Console.WriteLine("NodeManager disposed");
    }

    public NodeManager(string senderEndpoint)
    {
        _managerId = Guid.NewGuid();

        _nodePool = new List<Node>();

        _senderEndpoint = senderEndpoint;

        // create queue
        _queue = new ConcurrentQueue<Command>();

        _httpClient = new HttpClient();


        cancellationTokenSource = new CancellationTokenSource();

        // start queue Handler in new Thread

        _queueHandlerTask = Task.Run(async () => await QueueProcessingAsync(cancellationTokenSource.Token));
    }

    public Node CreateNode(string name = "",
        string color = "",
        double size = default,
        bool selected = false,
        int visited = default,
        object? value = default)
    {
        var newNode = new Node(_queue, name: name, color: color, size: size, selected: selected, visited: visited, value: value);

        _nodePool.Add(newNode); 
        
        return newNode;
    }

    public Node? GetNodeById(Guid id)
    {
        return _nodePool.FirstOrDefault(x => x.Id == id);
    }

    public void DeleteNode(Node node)
    {
        if (!_nodePool.Contains(node)) 
        {
            throw new ArgumentException($"Node with id = {node.Id} does not exists in this NodeManager");
        }

        node.Delete();

        _nodePool.Remove(node);
    }

    /// --- Queue processing


    // обработчик очереди
    private async Task QueueProcessingAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("QueueProcessingAsync");

        bool hasElement; // в цикле do while гарантируем что обработаем все элементы, даже после запроса на окончание процесса
        do
        {
            Console.WriteLine(_queue.Count);

            // получаем элемент, но не удаляем его из очереди
            hasElement = _queue.TryPeek(out Command? command);
            if (hasElement)
            {
                bool success;
                do
                {
                    // обрабатываем полученную команду
                    success = await SendCommandAsync(command);

                    if (success)
                    {
                        // извлекаем только если успешно отправлена команда
                        _queue.TryDequeue(out command);
                    }
                }
                while (success != true); // отправляем пока не отправим
            }
            else
            {
                // очередь пуста - подождём
                await Task.Delay(100);
            }
        }
        while (!cancellationToken.IsCancellationRequested || hasElement);
        

        return;
    }


    // отправка команды
    private async Task<bool> SendCommandAsync(Command command, int requestCounter = 0)
    {
        try
        {
            // создание json-rpc запроса
            var rpcRequest = new RpcRequest
            {
                JsonRpc = "2.0",
                Command = command,
                ManagerId = _managerId,
                Id = Guid.NewGuid().ToString() // Уникальный идентификатор запроса
            };

            string requestJson = JsonConvert.SerializeObject(rpcRequest);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Отправка запроса на сервер (замените URL на свой)
            var response = await _httpClient.PostAsync(this._senderEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var rpcResponse = JsonConvert.DeserializeObject<RpcResponse>(responseJson);

                if (rpcResponse != null && rpcResponse?.Error == null)
                {
                    // Запрос успешно обработан
                    Console.WriteLine($"Request was processed successfully. Result: {rpcResponse?.Result}");
                    return true;
                }
                else
                {
                    // Ошибка при обработке запроса
                    Console.WriteLine($"Request processing error: {rpcResponse?.Error}");
                    return false;
                }
            }
            else
            {
                // Ошибка HTTP-запроса
                Console.WriteLine($"HTTP request error: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine($"Ошибка отправки запроса\n{ex.Message}");
            return false;
        }
    }


}
