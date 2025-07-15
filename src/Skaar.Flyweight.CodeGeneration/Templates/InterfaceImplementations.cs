using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Skaar.Flyweight.Templates;

class InterfaceImplementations
{
    private readonly ITypeSymbol _valueTypeSymbol;
    private readonly List<ITypeSymbol> _iComparableInterfaces;
    private readonly List<ITypeSymbol> _iEnumerableInterfaces;
    private readonly bool _iFormattable;
    public InterfaceImplementations(ITypeSymbol valueTypeSymbol)
    {
        _valueTypeSymbol = valueTypeSymbol;
        _iComparableInterfaces = GetIComparableInterfaces(valueTypeSymbol).ToList();
        _iEnumerableInterfaces = GetIEnumerableInterfaces(valueTypeSymbol).ToList();
        _iFormattable = valueTypeSymbol.AllInterfaces.Any(i => i.Name == "IFormattable");
    }

    public string InterfaceList()
    {
        var interfaces = _iComparableInterfaces
            .Select(i => $"System.IComparable<{i.ToDisplayString()}>")
            .Union(_iEnumerableInterfaces.Select(i => $"System.Collections.Generic.IEnumerable<{i.ToDisplayString()}>"));
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
        
        foreach (var i in _iEnumerableInterfaces)
        {
            stringBuidler.AppendLine($$"""
                System.Collections.Generic.IEnumerator<{{i.ToDisplayString()}}> System.Collections.Generic.IEnumerable<{{i.ToDisplayString()}}>.GetEnumerator() => ((System.Collections.Generic.IEnumerable<{{i.ToDisplayString()}}>) GetInnerValue()).GetEnumerator(); 
            """);
        }

        if (_iEnumerableInterfaces.Any())
        {
            stringBuidler.AppendLine($$"""
                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable) GetInnerValue()).GetEnumerator(); 
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
    private IEnumerable<ITypeSymbol> GetIEnumerableInterfaces(ITypeSymbol valueTypeSymbol)
    {
        var interfaces = valueTypeSymbol.AllInterfaces;
        foreach (var i in interfaces)
        {
            if (i.Name == "IEnumerable" && i.TypeArguments.Length == 1)
            {
                yield return i.TypeArguments[0];
            }
        }
    }
}