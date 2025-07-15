using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Skaar.Flyweight.Templates;

class InterfaceImplementations
{
    private readonly ITypeSymbol _valueTypeSymbol;
    private List<ITypeSymbol> _iComparableInterfaces;
    private bool _iFormattable;
    public InterfaceImplementations(ITypeSymbol valueTypeSymbol)
    {
        _valueTypeSymbol = valueTypeSymbol;
        _iComparableInterfaces = GetIComparableInterfaces(valueTypeSymbol).ToList();
        _iFormattable = valueTypeSymbol.AllInterfaces.Any(i => i.Name == "IFormattable");
    }

    public string InterfaceList()
    {
        var interfaces = _iComparableInterfaces.Select(i => $"System.IComparable<{i.ToDisplayString()}>");
        if (_iFormattable)
        {
            interfaces = interfaces.Append($"System.IFormattable");
        }
        var list = $", {string.Join(", ", interfaces)}";
        return list.Length > 2 ? list : string.Empty;
    }

    public string Implementations()
    {
        var stringBuidler = new StringBuilder();
        foreach (var i in _iComparableInterfaces)
        {
            stringBuidler.AppendLine($$"""
                int System.IComparable<{{i.ToDisplayString()}}>.CompareTo({{i.ToDisplayString()}} other) => GetInnerValue().CompareTo(other); 
            """);
        }

        if (_iFormattable)
        {
            stringBuidler.AppendLine($$"""
                string System.IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ((System.IFormattable) GetInnerValue()).ToString(format, formatProvider); 
            """);
        }
        return stringBuidler.ToString();
    }
    
    private IEnumerable<ITypeSymbol> GetIComparableInterfaces(ITypeSymbol valueTypeSymbol)
    {
        var interfaces = valueTypeSymbol.AllInterfaces;
        foreach (var i in interfaces)
        {
            if (i.Name == "IComparable" && i.TypeArguments.Length == 1)
            {
                yield return i.TypeArguments[0];
            }
        }
    }
}