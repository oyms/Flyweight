using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using Skaar.Flyweight.Contracts;
using Skaar.Flyweight.Serialization;

namespace Skaar.Flyweight.Tests;

public class GenericFlyweightTests
{
    [Fact]
    public void Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestType.Get(new(true, 1));
        var second = TestType.Get(new(true, 1));
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }    
    
    [Fact]
    public void Equals_WithDifferentKey_ReturnsFalse()
    {
        var first = TestType.Get(new(true, 1));
        var second = TestType.Get(new(false, 1));
        (first != second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeFalse();
    }    
    
    [Fact]
    public void ImplicitConversion_WithValue_ReturnsSameValue()
    {
        var input = new ValueType(true, 2);
        var target = TestType.Get(input);
        ValueType value = target;
        value.ShouldBe(input);
    }
    
    [Fact]
    public void Serialization_WithSameValue_ReturnsSameInstance()
    {
        var json = """
                   {
                        "value0": {
                            "boolValue": true,
                            "intValue": 1
                        },
                        "value1": {
                           "boolValue": false,
                           "intValue": 0
                       }
                   }
                   """;
        var first = JsonSerializer.Deserialize<SerializationTestType>(json, JsonSerializerOptions.Web)!;
        var second = JsonSerializer.Deserialize<SerializationTestType>(json, JsonSerializerOptions.Web)!;

        first.Value0.ShouldBe(second.Value0);
        first.Value1.ShouldBe(second.Value1);
        ReferenceEquals(first.Value0, second.Value0).ShouldBeTrue();
        var x = TestType.AllValues;
        x.ShouldNotBeEmpty();
    }
}

// ReSharper disable NotAccessedPositionalProperty.Local
file record ValueType(bool BoolValue, int IntValue);
[JsonConverter(typeof(FlyweightJsonConverter<TestType, ValueType>))]
file class TestType : FlyweightBase<TestType, ValueType>, IFlyweightFactory<TestType, ValueType>
{
    private TestType(ValueType key) : base(key)
    {
    }

    public static TestType Get(ValueType key)
    {
        return Get(key, value => new TestType(value));
    }
}


file class SerializationTestType
{
    public required TestType Value0 { get; init; }
    public required TestType Value1 { get; init; }
}