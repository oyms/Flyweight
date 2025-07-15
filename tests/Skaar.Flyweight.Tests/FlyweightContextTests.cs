using Shouldly;

namespace Skaar.Flyweight.Tests;

public class FlyweightScopeTests
{
    [Fact]
    public void UsingScope_WhenDisposed_NewInstancesAreNotSame()
    {
        TestTypeForContextNonGeneric first, second, third;
        using(FlyWeightScope.Create())
        {
            first = TestTypeForContextNonGeneric.Get("a");
            second = TestTypeForContextNonGeneric.Get("a");
        }
        third = TestTypeForContextNonGeneric.Get("a");
        
        ReferenceEquals(first, second).ShouldBeTrue();
        ReferenceEquals(second, third).ShouldBeFalse();
    }    
    
    [Fact]
    public void UsingScopeWithGenericType_WhenDisposed_NewInstancesAreNotSame()
    {
        TestTypeForContextGeneric first, second, third;
        using(FlyWeightScope.Create())
        {
            first = TestTypeForContextGeneric.Get(new("a"));
            second = TestTypeForContextGeneric.Get(new("a"));
        }
        third = TestTypeForContextGeneric.Get(new("a"));
        
        ReferenceEquals(first, second).ShouldBeTrue();
        ReferenceEquals(second, third).ShouldBeFalse();
    }
}

public record TestValueForContext(string Value);

[Flyweight<TestValueForContext>]
partial class TestTypeForContextGeneric;
[Flyweight]
partial class TestTypeForContextNonGeneric;