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

    // Конструктор, що ініціалізує елемент з тегом, типом відображення, типом закривання та класами CSS
    public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses)
    {
        _tagName = tagName;
        _displayType = displayType;
        _closingType = closingType;
        _cssClasses = cssClasses;
        _children = new List<LightNode>();
    }

    // Додає дочірній вузол до поточного елементу
    public void AddChild(LightNode node)
    {
        _children.Add(node);
    }

    // Повертає зовнішнє HTML представлення елементу
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

    // Повертає внутрішнє HTML представлення елементу
    public override string GetInnerHtml()
    {
        string result = "";
        foreach (var child in _children)
        {
            result += child.GetInnerHtml();
        }
        return result;
    }
    // Метод для створення ітератора для обходу дерева HTML в глибину
    public IIterator CreateDepthFirstIterator()
    {
        return new DepthFirstIterator(this);
    }

    // Метод для створення ітератора для обходу дерева HTML в ширину
    public IIterator CreateBreadthFirstIterator()
    {
        return new BreadthFirstIterator(this);
    }

    // Властивість для доступу до дочірніх вузлів елементу
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

    // Конструктор, що приймає кореневий вузол дерева
    public DepthFirstIterator(LightNode root)
    {
        Traverse(root);
    }

    // Рекурсивна функція для обходу дерева HTML в глибину
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

    // Повертає наступний елемент для обробки
    public LightNode Next()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return null;
    }

    // Перевіряє, чи є ще елементи для обробки
    public bool HasNext()
    {
        return stack.Count > 0;
    }
}
// Клас ітератора для обходу дерева HTML в ширину
class BreadthFirstIterator : IIterator
{
    private Queue<LightNode> queue = new Queue<LightNode>();

    // Конструктор, що приймає кореневий вузол дерева
    public BreadthFirstIterator(LightNode root)
    {
        Traverse(root);
    }

    // Рекурсивна функція для обходу дерева HTML в ширину
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

    // Повертає наступний елемент для обробки
    public LightNode Next()
    {
        if (queue.Count > 0)
        {
            return queue.Dequeue();
        }
        return null;
    }

    // Перевіряє, чи є ще елементи для обробки
    public bool HasNext()
    {
        return queue.Count > 0;
    }
}
// Основний клас програми
class Program
{
    static void Main(string[] args)
    {
        // Створення прикладу HTML-структури
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

        // Використання ітератора для обходу дерева HTML в глибину
        IIterator depthFirstIterator = header.CreateDepthFirstIterator();
        Console.WriteLine("Depth First Traversal:");
        while (depthFirstIterator.HasNext())
        {
            LightNode node = depthFirstIterator.Next();
            Console.WriteLine(node.GetOuterHtml());
        }
        // Використання ітератора для обходу дерева HTML в ширину
        IIterator breadthFirstIterator = header.CreateBreadthFirstIterator();
        Console.WriteLine("\nBreadth First Traversal:");
        while (breadthFirstIterator.HasNext())
        {
            LightNode node = breadthFirstIterator.Next();
            Console.WriteLine(node.GetOuterHtml());
        }
    }
}