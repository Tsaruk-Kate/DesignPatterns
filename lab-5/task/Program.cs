using System;
using System.Collections.Generic;

class LightNode
{
    public virtual string GetOuterHtml() { return ""; }
    public virtual string GetInnerHtml() { return ""; }
}

class LightTextNode : LightNode
{
    private string _text;
    public LightTextNode(string text)
    {
        _text = text;
    }
    public override string GetOuterHtml()
    {
        return _text;
    }
    public override string GetInnerHtml()
    {
        return _text;
    }
}

class LightElementNode : LightNode
{
    private string _tagName;
    private string _displayType;
    private string _closingType;
    private List<LightNode> _children;
    private List<string> _cssClasses;

    public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses)
    {
        _tagName = tagName;
        _displayType = displayType;
        _closingType = closingType;
        _cssClasses = cssClasses;
        _children = new List<LightNode>();
    }

    public void AddChild(LightNode node)
    {
        _children.Add(node);
    }

    public void RemoveChild(LightNode node)
    {
        _children.Remove(node);
    }

    public override string GetOuterHtml()
    {
        string result = $"<{_tagName} class=\"{string.Join(" ", _cssClasses)}\" display=\"{_displayType}\" closing=\"{_closingType}\">\n";
        foreach (var child in _children)
        {
            result += $"\t{child.GetOuterHtml()}\n";
        }
        if (_closingType == "closing")
        {
            result += $"</{_tagName}>";
        }
        return result;
    }

    public override string GetInnerHtml()
    {
        string result = "";
        foreach (var child in _children)
        {
            result += child.GetInnerHtml();
        }
        return result;
    }
}
// Інтерфейс команди
interface ICommand
{
    void Execute(); // Метод виконання команди
}

// Команда для додавання дочірнього вузла
class AddChildCommand : ICommand
{
    private LightElementNode _parent;
    private LightNode _child;

    public AddChildCommand(LightElementNode parent, LightNode child)
    {
        _parent = parent;
        _child = child;
    }

    public void Execute()
    {
        _parent.AddChild(_child);
    }
}

// Команда для видалення дочірнього вузла
class RemoveChildCommand : ICommand
{
    private LightElementNode _parent;
    private LightNode _child;

    public RemoveChildCommand(LightElementNode parent, LightNode child)
    {
        _parent = parent;
        _child = child;
    }

    public void Execute()
    {
        _parent.RemoveChild(_child);
    }
}

// Виконавець команд
class CommandInvoker
{
    private List<ICommand> _commands = new List<ICommand>();

    public void StoreAndExecute(ICommand command)
    {
        _commands.Add(command);
        command.Execute();
    }
}

// Головний клас програми
class Program
{
    static void Main(string[] args)
    {
        CommandInvoker invoker = new CommandInvoker();

        // Створюємо елементи дерева HTML
        LightElementNode header = new LightElementNode("h1", "block", "closing", new List<string>());
        LightTextNode headerText = new LightTextNode("Welcome to my page!");
        invoker.StoreAndExecute(new AddChildCommand(header, headerText));

        LightElementNode table = new LightElementNode("table", "block", "closing", new List<string> { "styled-table" });
        invoker.StoreAndExecute(new AddChildCommand(header, table));

        LightElementNode tableRow1 = new LightElementNode("tr", "block", "closing", new List<string>());
        invoker.StoreAndExecute(new AddChildCommand(table, tableRow1));

        LightElementNode tableData1 = new LightElementNode("td", "inline", "closing", new List<string>());
        LightTextNode dataText1 = new LightTextNode("Cell 1");
        invoker.StoreAndExecute(new AddChildCommand(tableRow1, tableData1));
        invoker.StoreAndExecute(new AddChildCommand(tableData1, dataText1));

        LightElementNode tableData2 = new LightElementNode("td", "inline", "closing", new List<string>());
        LightTextNode dataText2 = new LightTextNode("Cell 2");
        invoker.StoreAndExecute(new AddChildCommand(tableRow1, tableData2));
        invoker.StoreAndExecute(new AddChildCommand(tableData2, dataText2));

        // Видаляємо заголовок
        invoker.StoreAndExecute(new RemoveChildCommand(header, headerText));

        // Виводимо HTML
        Console.WriteLine(header.GetOuterHtml());
        Console.WriteLine(table.GetOuterHtml());
    }
}