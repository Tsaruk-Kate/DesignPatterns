using System;
using System.Collections.Generic;

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

class LightNode
{
    public virtual string GetOuterHtml() { return ""; }
    public virtual string GetInnerHtml() { return ""; }

    // Додано метод для виклику хуків життєвого циклу
    public virtual void RunLifecycleHooks() { }
}

class LightTextNode : LightNode
{
    private string _text;
    private NodeLifecycleHooks _lifecycleHooks;

    public LightTextNode(string text)
    {
        _text = text;
        _lifecycleHooks = new ElementLifecycleHooks();
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
}

class LightElementNode : LightNode
{
    private string _tagName;
    private string _displayType;
    private string _closingType;
    private List<LightNode> _children;
    private List<string> _cssClasses;
    private NodeLifecycleHooks _lifecycleHooks;

    public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses)
    {
        _tagName = tagName;
        _displayType = displayType;
        _closingType = closingType;
        _cssClasses = cssClasses;
        _children = new List<LightNode>();
        _lifecycleHooks = new ElementLifecycleHooks();
    }

    public void AddChild(LightNode node)
    {
        _children.Add(node);
    }

    public override string GetOuterHtml()
    {
        RunLifecycleHooks(); // Виклик хуків перед отриманням HTML
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

    public override void RunLifecycleHooks()
    {
        _lifecycleHooks.RunLifecycleHooks();
    }
}

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

        header.RunLifecycleHooks();
        Console.WriteLine(header.GetOuterHtml());
        table.RunLifecycleHooks();
        Console.WriteLine(table.GetOuterHtml());
    }
}