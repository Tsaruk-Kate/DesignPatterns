using System;
using System.Collections.Generic;
using System.Linq;

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
// Інтерфейс для Template Method
abstract class NodeLifecycleHooks
{
    // Абстрактні методи для кроків життєвого циклу
    public abstract void OnCreated();
    public abstract void OnInserted();
    public abstract void OnRemoved();
    public abstract void OnStylesApplied();
    public abstract void OnClassListApplied();
    public abstract void OnTextRendered();
    // Шаблонний метод для виклику кроків життєвого циклу
    public void RunLifecycleHooks()
    {
        OnCreated();
        OnInserted();
        OnRemoved();
        OnStylesApplied();
        OnClassListApplied();
        OnTextRendered();
    }
}
// Клас для кроків життєвого циклу елемента
class ElementLifecycleHooks : NodeLifecycleHooks
{
    public override void OnCreated()
    {
        Console.WriteLine("Element created.");
    }
    public override void OnInserted()
    {
        Console.WriteLine("Element inserted.");
    }
    public override void OnRemoved()
    {
        Console.WriteLine("Element removed.");
    }
    public override void OnStylesApplied()
    {
        Console.WriteLine("Styles applied to element.");
    }
    public override void OnClassListApplied()
    {
        Console.WriteLine("Class list applied to element.");
    }
    public override void OnTextRendered()
    {
        Console.WriteLine("Text rendered inside element.");
    }
}
// Клас для кроків життєвого циклу текстового вузла
class TextNodeLifecycleHooks : NodeLifecycleHooks
{
    public override void OnCreated()
    {
        Console.WriteLine("Text node created.");
    }
    public override void OnInserted()
    {
        Console.WriteLine("Text node inserted.");
    }
    public override void OnRemoved()
    {
        Console.WriteLine("Text node removed.");
    }
    public override void OnStylesApplied()
    {
        Console.WriteLine("Styles applied to text node.");
    }
    public override void OnClassListApplied()
    {
        Console.WriteLine("Class list applied to text node.");
    }
    public override void OnTextRendered()
    {
        Console.WriteLine("Text node rendered.");
    }
}
// Базовий клас для відвідувача
interface IVisitor
{
    void VisitElementNode(LightElementNode elementNode);
    void VisitTextNode(LightTextNode textNode);
    void VisitListElementNode(LightListElementNode listElementNode);
    void VisitListItemNode(LightListItemNode listItemNode);
    void VisitTextInputNode(LightTextInputNode textInputNode);
    void VisitButtonNode(LightButtonNode buttonNode);
    void VisitSelectNode(LightSelectNode selectNode);
}
// Реалізація відвідувача для виводу HTML
class HtmlVisitor : IVisitor
{
    public void VisitElementNode(LightElementNode elementNode)
    {
        Console.WriteLine(elementNode.GetOuterHtml());
    }
    public void VisitTextNode(LightTextNode textNode)
    {
        Console.WriteLine(textNode.GetOuterHtml());
    }
    public void VisitListElementNode(LightListElementNode listElementNode)
    {
        Console.WriteLine(listElementNode.GetOuterHtml());
    }
    public void VisitListItemNode(LightListItemNode listItemNode)
    {
        Console.WriteLine(listItemNode.GetOuterHtml());
    }
    public void VisitTextInputNode(LightTextInputNode textInputNode)
    {
        Console.WriteLine(textInputNode.GetOuterHtml());
    }
    public void VisitButtonNode(LightButtonNode buttonNode)
    {
        Console.WriteLine(buttonNode.GetOuterHtml());
    }
    public void VisitSelectNode(LightSelectNode selectNode)
    {
        Console.WriteLine(selectNode.GetOuterHtml());
    }
}
// Абстрактний клас Node
abstract class LightNode
{
    protected IHtmlState _state;
    protected NodeLifecycleHooks _lifecycleHooks;

    public void SetState(IHtmlState state)
    {
        _state = state;
    }
    public virtual string GetOuterHtml() { return ""; }
    public virtual string GetInnerHtml() { return ""; }
    public virtual void RunLifecycleHooks()
    {
        _lifecycleHooks.RunLifecycleHooks();
    }
    public virtual void SetEditMode() { }
    public virtual void SetViewMode() { }
    public abstract void Accept(IVisitor visitor);
}
// Клас текстового вузла
class LightTextNode : LightNode
{
    private string _text;

    public LightTextNode(string text)
    {
        _text = text;
        _lifecycleHooks = new TextNodeLifecycleHooks();
    }
    public override string GetOuterHtml()
    {
        return _text;
    }
    public override string GetInnerHtml()
    {
        return _text;
    }
    public override void RunLifecycleHooks()
    {
        _lifecycleHooks.RunLifecycleHooks();
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitTextNode(this);
    }
}
// Клас елемента вузла
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
        _lifecycleHooks = new ElementLifecycleHooks();
        _attributes = new Dictionary<string, string>();

    }
    public void AddChild(LightNode node)
    {
        if (node != null)
            _children.Add(node);
    }
    public override string GetOuterHtml()
    {
        RunLifecycleHooks(); // Виклик хуків перед отриманням HTML
        string result = $"<{_tagName} class=\"{string.Join(" ", _cssClasses)}\" display=\"{_displayType}\" closing=\"{_closingType}\"";

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
    public override void RunLifecycleHooks()
    {
        _lifecycleHooks.RunLifecycleHooks();
    }
    public void AddAttribute(string key, string value)
    {
        if (_attributes == null)
            _attributes = new Dictionary<string, string>();

        _attributes[key] = value;
    }
    public void RemoveAttribute(string key)
    {
        if (_attributes != null && _attributes.ContainsKey(key))
            _attributes.Remove(key);
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitElementNode(this);
    }
}
// Клас для елемента списку
class LightListItemNode : LightNode
{
    private List<LightNode> _children;
    public LightListItemNode()
    {
        _children = new List<LightNode>();
    }
    public override string GetOuterHtml()
    {
        string result = "<li>\n";
        foreach (var child in _children)
        {
            result += $"\t{child.GetOuterHtml()}\n";
        }
        result += "</li>";
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
    public void AddChild(LightNode node)
    {
        _children.Add(node);
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitListItemNode(this);
    }
}
// Клас для списку (ul або ol)
class LightListElementNode : LightNode
{
    private List<LightNode> _children;
    private string _listType;
    public LightListElementNode(string listType)
    {
        _listType = listType;
        _children = new List<LightNode>();
    }
    public override string GetOuterHtml()
    {
        string result = $"<{_listType}>\n";
        foreach (var child in _children)
        {
            result += $"\t{child.GetOuterHtml()}\n";
        }
        result += $"</{_listType}>";
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
    public void AddChild(LightNode node)
    {
        _children.Add(node);
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitListElementNode(this);
    }
}
// Клас для елемента форми - текстового поля
class LightTextInputNode : LightNode
{
    private string _name;
    public LightTextInputNode(string name)
    {
        _name = name;
    }
    public override string GetOuterHtml()
    {
        return $"<input type=\"text\" name=\"{_name}\">";
    }
    public override string GetInnerHtml()
    {
        return "";
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitTextInputNode(this);
    }
}
// Клас для елемента форми - кнопки
class LightButtonNode : LightNode
{
    private string _text;
    public LightButtonNode(string text)
    {
        _text = text;
    }
    public override string GetOuterHtml()
    {
        return $"<button>{_text}</button>";
    }
    public override string GetInnerHtml()
    {
        return "";
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitButtonNode(this);
    }
}
// Клас для елемента форми - випадаючого списку
class LightSelectNode : LightNode
{
    private List<string> _options;
    public LightSelectNode(List<string> options)
    {
        _options = options;
    }
    public override string GetOuterHtml()
    {
        string optionsHtml = "";
        foreach (var option in _options)
        {
            optionsHtml += $"<option>{option}</option>";
        }
        return $"<select>{optionsHtml}</select>";
    }
    public override string GetInnerHtml()
    {
        return "";
    }
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitSelectNode(this);
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

        // Перевірка ElementLifecycleHooks
        Console.WriteLine("\nRunning Element Lifecycle Hooks:");
        ElementLifecycleHooks elementLifecycleHooks = new ElementLifecycleHooks();
        elementLifecycleHooks.RunLifecycleHooks();
        // Перевірка TextNodeLifecycleHooks
        Console.WriteLine("\nRunning Text Node Lifecycle Hooks:");
        TextNodeLifecycleHooks textNodeLifecycleHooks = new TextNodeLifecycleHooks();
        textNodeLifecycleHooks.RunLifecycleHooks();

        // Додавання форми з текстовим полем
        LightTextInputNode inputField = new LightTextInputNode("username");
        header.AddChild(inputField);
        // Додавання кнопки
        LightButtonNode submitButton = new LightButtonNode("Submit");
        header.AddChild(submitButton);
        // Додавання випадаючого списку
        List<string> options = new List<string> { "Option 1", "Option 2", "Option 3" };
        LightSelectNode selectList = new LightSelectNode(options);
        header.AddChild(selectList);
        // Використовуємо відвідувача для обходу дерева HTML та виводу
        IVisitor htmlVisitor = new HtmlVisitor();
        header.Accept(htmlVisitor);
    }
}