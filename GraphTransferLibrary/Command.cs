
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace GraphTransferLibrary;

public class Command
{
    public Guid NodeId { get; set; }

    [JsonConverter(typeof(StringEnumConverter))] // Применяем конвертер к свойству (конвертация enum в строку а не в число)
    public CommandType CommandName { get; set; }
    public object? Value;
}


public class Command<T> : Command
{
    public new T? Value { get; set; }
}

