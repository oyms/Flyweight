using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using Skaar.Flyweight;
using Skaar.Flyweight.Contracts;
using Skaar.Flyweight.Serialization;

namespace Skaar.Flyweight.Testss;

public class FlyweightTests
{
    [Fact]
    public void Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestType.Get("1");
        var second = TestType.Get("1");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentKey_ReturnsFalse()
    {
        var first = TestType.Get("1");
        var second = TestType.Get("2");
        first.ShouldNotBe(second);
        (first != second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeFalse();
    }

    [Fact]
    public void ToString_WithKey_ReturnsKey()
    {
        const string expected = "B10BF14246FE4BA3BC4CE4D9F49BA932";
        var target = TestType.Get(expected);
        target.ToString().ShouldBe(expected);
    }

    [Fact]
    public void Serialization_WithSameValue_ReturnsSameInstance()
    {
        var json = """
                   {
                        "value0": "abc",
                        "value1": "def"
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

[JsonConverter(typeof(FlyweightJsonConverter<TestType>))]
file class TestType : FlyweightBase<TestType>, IFlyweightFactory<TestType>
{
    private TestType(string key) : base(key)
    {
    }

    public static TestType Get(string key)
    {
        return Get(key, value => new TestType(value));
    }
}

file class SerializationTestType
{
    public required TestType Value0 { get; init; }
    public required TestType Value1 { get; init; }
}
