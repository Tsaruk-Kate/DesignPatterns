using System;
using System.Collections.Generic;
using System.Linq;

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
// Базовий клас для вузла
abstract class LightNode
{
    public abstract string GetOuterHtml();
    public abstract string GetInnerHtml();
    public abstract void Accept(IVisitor visitor);
}
// Клас для текстового вузла
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
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitTextNode(this);
    }
}
// Розширений клас для елемента вузла з атрибутами
class LightElementNode : LightNode
{
    private string _tagName;
    private string _displayType;
    private string _closingType;
    private List<LightNode> _children;
    private List<string> _cssClasses;
    private Dictionary<string, string> _attributes;
    public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses)
    {
        _tagName = tagName;
        _displayType = displayType;
        _closingType = closingType;
        _cssClasses = cssClasses;
        _children = new List<LightNode>();
        _attributes = new Dictionary<string, string>();
    }
    public void AddAttribute(string attributeName, string attributeValue)
    {
        _attributes[attributeName] = attributeValue;
    }
    public override string GetOuterHtml()
    {
        string attributes = string.Join(" ", _attributes.Select(kv => $"{kv.Key}=\"{kv.Value}\"")); // Генерація рядка атрибутів
        string result = $"<{_tagName} class=\"{string.Join(" ", _cssClasses)}\" display=\"{_displayType}\" closing=\"{_closingType}\" {attributes}>\n";
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
    public void AddChild(LightNode node)
    {
        _children.Add(node);
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
// Клас програми
class Program
{
    static void Main(string[] args)
    {
        // Створюємо дерево HTML
        LightElementNode header = new LightElementNode("h1", "block", "closing", new List<string>());
        header.AddAttribute("id", "main-header");
        LightTextNode headerText = new LightTextNode("Welcome to my page!");
        header.AddChild(headerText);
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