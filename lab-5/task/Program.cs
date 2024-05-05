using System;
using System.Collections.Generic;

// Інтерфейс для ітератора
interface IIterator
{
    LightNode Next();
    bool HasNext();
}

// Клас ітератора для обходу дерева HTML в глибину
class DepthFirstIterator : IIterator
{
    private Stack<LightNode> stack = new Stack<LightNode>();

    public DepthFirstIterator(LightNode root)
    {
        Traverse(root);
    }

    private void Traverse(LightNode node)
    {
        stack.Push(node);
        if (node is LightElementNode)
        {
            foreach (var child in ((LightElementNode)node).Children)
            {
                Traverse(child);
            }
        }
    }
    public LightNode Next()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return null;
    }
    public bool HasNext()
    {
        return stack.Count > 0;
    }
}
// Клас ітератора для обходу дерева HTML в ширину
class BreadthFirstIterator : IIterator
{
    private Queue<LightNode> queue = new Queue<LightNode>();
    public BreadthFirstIterator(LightNode root)
    {
        Traverse(root);
    }
    private void Traverse(LightNode node)
    {
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            LightNode currentNode = queue.Dequeue();
            if (currentNode is LightElementNode)
            {
                foreach (var child in ((LightElementNode)currentNode).Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
    public LightNode Next()
    {
        if (queue.Count > 0)
        {
            return queue.Dequeue();
        }
        return null;
    }
    public bool HasNext()
    {
        return queue.Count > 0;
    }
}
// Інтерфейс команди
interface ICommand
{
    void Execute();
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
        _parent.Children.Remove(_child);
    }
}
// Інтерфейс для стейт
interface IHtmlState
{
    void RenderHtml(LightNode node);
    void SwitchToViewMode(LightNode node);
    void SwitchToEditMode(LightNode node);
}
// Конкретний стан «Режим перегляду»
class ViewMode : IHtmlState
{
    public void RenderHtml(LightNode node)
    {
        Console.WriteLine(node.GetOuterHtml());
    }
    public void SwitchToViewMode(LightNode node)
    {
        // Вже в режимі перегляду
    }
    public void SwitchToEditMode(LightNode node)
    {
        node.SetEditMode();
    }
}
// Конкретний стан «Режим редагування»
class EditMode : IHtmlState
{
    public void RenderHtml(LightNode node)
    {
        Console.WriteLine(node.GetInnerHtml());
    }
    public void SwitchToViewMode(LightNode node)
    {
        node.SetViewMode();
    }
    public void SwitchToEditMode(LightNode node)
    {
        // Вже в режимі редагування
    }
}
abstract class LightNode
{
    protected IHtmlState _state;
    public void SetState(IHtmlState state)
    {
        _state = state;
    }
    public abstract string GetOuterHtml();
    public abstract string GetInnerHtml();

    public virtual void SwitchToViewMode()
    {
        _state.SwitchToViewMode(this);
    }
    public virtual void SwitchToEditMode()
    {
        _state.SwitchToEditMode(this);
    }
    public virtual void SetEditMode() { }
    public virtual void SetViewMode() { }
}
class LightTextNode : LightNode
{
    private string _text;
    public LightTextNode(string text)
    {
        _text = text;
        SetState(new ViewMode());
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
    private Dictionary<string, string> _attributes;
    public List<LightNode> Children { get { return _children; } }

    public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses)
    {
        _tagName = tagName;
        _displayType = displayType;
        _closingType = closingType;
        _cssClasses = cssClasses;
        _children = new List<LightNode>();
        _attributes = new Dictionary<string, string>();
        SetState(new ViewMode());
    }
    public void AddChild(LightNode node)
    {
        if (node != null)
            _children.Add(node);
    }
    // Методи додавання та видалення атрибутів
    public void AddAttribute(string key, string value)
    {
        _attributes[key] = value;
    }
    public void RemoveAttribute(string key)
    {
        _attributes.Remove(key);
    }
    public override string GetOuterHtml()
    {
        string result = $"<{_tagName} class=\"{string.Join(" ", _cssClasses)}\" display=\"{_displayType}\" closing=\"{_closingType}\"";

        // Додавання атрибутів до HTML-виводу
        foreach (var attribute in _attributes)
        {
            result += $" {attribute.Key}=\"{attribute.Value}\"";
        }

        result += ">\n";
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
    public override void SetEditMode()
    {
        SetState(new EditMode());
        foreach (var child in _children)
        {
            child.SetEditMode();
        }
    }
    public override void SetViewMode()
    {
        SetState(new ViewMode());
        foreach (var child in _children)
        {
            child.SetViewMode();
        }
    }
}
// Контекстний клас
class HtmlContext
{
    private IHtmlState _state;
    public HtmlContext()
    {
        // Початковий стан - режим перегляду
        TransitionTo(new ViewMode());
    }
    // Перехідні стани
    public void TransitionTo(IHtmlState state)
    {
        Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
        _state = state;
    }
    // Метод рендерингу HTML
    public void RenderHtml(LightNode node)
    {
        _state.RenderHtml(node);
    }
    // Методи додавання та видалення атрибутів у режимі редагування
    public void AddAttribute(LightElementNode node, string key, string value)
    {
        if (_state is EditMode)
        {
            node.AddAttribute(key, value);
        }
        else
        {
            Console.WriteLine("Attributes can only be added in edit mode.");
        }
    }

    public void RemoveAttribute(LightElementNode node, string key)
    {
        if (_state is EditMode)
        {
            node.RemoveAttribute(key);
        }
        else
        {
            Console.WriteLine("Attributes can only be removed in edit mode.");
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        // Створення контексту для HTML
        HtmlContext context = new HtmlContext();

        // Створення елементів дерева HTML
        LightElementNode header = new LightElementNode("h1", "block", "closing", new List<string>());
        LightTextNode headerText = new LightTextNode("Welcome to my page!");
        header.AddChild(headerText);

        LightElementNode table = new LightElementNode("table", "block", "closing", new List<string> { "styled-table" });

        LightElementNode tableRow1 = new LightElementNode("tr", "block", "closing", new List<string>());
        table.AddChild(tableRow1);

        LightElementNode tableData1 = new LightElementNode("td", "inline", "closing", new List<string>());
        LightTextNode dataText1 = new LightTextNode("Cell 1");
        tableData1.AddChild(dataText1);
        tableRow1.AddChild(tableData1);

        LightElementNode tableData2 = new LightElementNode("td", "inline", "closing", new List<string>());
        LightTextNode dataText2 = new LightTextNode("Cell 2");
        tableData2.AddChild(dataText2);
        tableRow1.AddChild(tableData2);

        // Перегляд HTML
        Console.WriteLine("Initial HTML:");
        context.RenderHtml(header);
        context.RenderHtml(table);

        // Виконання ітерацій
        Console.WriteLine("\nDepth First Traversal:");
        IIterator depthFirstIterator = new DepthFirstIterator(header);
        while (depthFirstIterator.HasNext())
        {
            LightNode node = depthFirstIterator.Next();
            context.RenderHtml(node);
        }

        Console.WriteLine("\nBreadth First Traversal:");
        IIterator breadthFirstIterator = new BreadthFirstIterator(header);
        while (breadthFirstIterator.HasNext())
        {
            LightNode node = breadthFirstIterator.Next();
            context.RenderHtml(node);
        }

        // Виконання команд
        ICommand addHeaderCommand = new AddChildCommand(header, headerText);
        ICommand addTableCommand = new AddChildCommand(header, table);
        ICommand removeHeaderCommand = new RemoveChildCommand(header, headerText);

        addHeaderCommand.Execute();
        addTableCommand.Execute();
        removeHeaderCommand.Execute();

        // Зміна стану елементів
        Console.WriteLine("\nChanging to Edit Mode:");
        header.SetEditMode();
        table.SetEditMode();

        Console.WriteLine("\nAfter Edit Mode:");
        context.RenderHtml(header);
        context.RenderHtml(table);

        Console.WriteLine("\nChanging to View Mode:");
        header.SetViewMode();
        table.SetViewMode();

        Console.WriteLine("\nAfter View Mode:");
        context.RenderHtml(header);
        context.RenderHtml(table);

        // Додавання та видалення атрибутів
        Console.WriteLine("\nAdding attributes in Edit Mode:");
        context.AddAttribute(table, "border", "1");
        context.AddAttribute(table, "cellpadding", "5");
        context.RemoveAttribute(table, "class");

        Console.WriteLine("\nAfter Adding/Removing Attributes:");
        context.RenderHtml(header);
        context.RenderHtml(table);
    }
}