using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Skaar.Flyweight.Templates;

class InterfaceImplementations
{
    private List<ITypeSymbol> _iComparableInterfaces;
    public InterfaceImplementations(ITypeSymbol valueTypeSymbol)
    {
        _iComparableInterfaces = GetIComparableInterfaces(valueTypeSymbol).ToList();
    }

    public string InterfaceList()
    {
        if(_iComparableInterfaces.Count == 0) return string.Empty;
        var interfaces = _iComparableInterfaces.Select(i => $"System.IComparable<{i.ToDisplayString()}>");
        return $", {string.Join(", ", interfaces)}";
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