using System;
using System.Collections.Generic;

// Базовий клас для усіх елементів в HTML документі
class LightNode
{
    public virtual string GetOuterHtml() { return ""; }
    public virtual string GetInnerHtml() { return ""; }
}

// Клас для представлення текстових вузлів в HTML документі
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

// Клас для представлення елементів в HTML документі
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

    public IIterator CreateDepthFirstIterator()
    {
        return new DepthFirstIterator(this);
    }

    public IIterator CreateBreadthFirstIterator()
    {
        return new BreadthFirstIterator(this);
    }

    public List<LightNode> Children { get { return _children; } }
}

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

class Program
{
    static void Main(string[] args)
    {
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

        ICommand addHeaderCommand = new AddChildCommand(header, headerText);
        ICommand addTableCommand = new AddChildCommand(header, table);
        ICommand removeHeaderCommand = new RemoveChildCommand(header, headerText);

        addHeaderCommand.Execute();
        addTableCommand.Execute();
        removeHeaderCommand.Execute();

        Console.WriteLine("Depth First Traversal:");
        IIterator depthFirstIterator = header.CreateDepthFirstIterator();
        while (depthFirstIterator.HasNext())
        {
            LightNode node = depthFirstIterator.Next();
            Console.WriteLine(node.GetOuterHtml());
        }

        Console.WriteLine("\nBreadth First Traversal:");
        IIterator breadthFirstIterator = header.CreateBreadthFirstIterator();
        while (breadthFirstIterator.HasNext())
        {
            LightNode node = breadthFirstIterator.Next();
            Console.WriteLine(node.GetOuterHtml());
        }
    }
}