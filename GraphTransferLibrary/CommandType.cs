
namespace GraphTransferLibrary;

/// <summary>
/// возможные типы команд при работе с узлом графа
/// </summary>
public enum CommandType
{
    Create,
    SetName, 
    SetColor,
    SetSize,
    SetSelected,
    SetVisited,
    SetValue,
    AddChild,
    DeleteChild,
    Delete,

}
