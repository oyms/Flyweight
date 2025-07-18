using Shouldly;
using Skaar.Flyweight;
using Skaar.Flyweight.Contracts;

[assembly: GenerateFlyweightClass("Skaar.Flyweight.Tests.ConcurrencyTest.FlyweightType")]
[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.ConcurrencyTestValueType>("Skaar.Flyweight.Tests.ConcurrencyTest.FlyweightTypeGeneric")]
[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.ConcurrencyTestValueType>("Skaar.Flyweight.Tests.ConcurrencyTest.FlyweightTypeGeneric1")]

namespace Skaar.Flyweight.Tests;

public class ConcurrencyTests(ITestContextAccessor contextAccessor)
{
    [Fact]
    public async Task Generate_InScope_ScopeClearsAll()
    {
        const int groups = 1000, perGroup = 1000;
        using (FlyWeightScope.Create())
        {
            var tasks = Enumerable.Range(0, groups).Select(_ => Generate(perGroup, () => ConcurrencyTest.FlyweightType.Get(Guid.NewGuid().ToString("N"))));
            var result = await Task.WhenAll(tasks);
            _ = result.Select(x => ConcurrencyTest.FlyweightType.Get(x.ToString()));
            ConcurrencyTest.FlyweightType.AllValues.Count().ShouldBe(perGroup*groups);
        }
        ConcurrencyTest.FlyweightType.AllValues.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Generate_OutOfScope_GcClearsAll()
    {
        const int groups = 1000, perGroup = 1000;
        var tasks = Enumerable.Range(0, groups).Select(_ => Generate(perGroup, () => ConcurrencyTest.FlyweightTypeGeneric.Get(new(Guid.NewGuid().ToString("N")))));
        await Task.WhenAll(tasks);
        ConcurrencyTest.FlyweightTypeGeneric.AllValues.Count().ShouldBe(perGroup*groups);
        
        await ForceGarbageCollection();

        ConcurrencyTest.FlyweightTypeGeneric.AllValues.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Generate_TwoTypesSameInnerValue_NotComparedToEachOther()
    {
        const int groups = 1000, perGroup = 1000;
        var tasks = Enumerable.Range(0, groups).Select(_ => Generate(perGroup, () => ConcurrencyTest.FlyweightTypeGeneric.Get(new(Guid.NewGuid().ToString("N")))));
        var result = await Task.WhenAll(tasks);
        _ = result.SelectMany(x => x).Select(x => ConcurrencyTest.FlyweightTypeGeneric1.Get(x.GetInnerValue())).ToList();
        ConcurrencyTest.FlyweightTypeGeneric.AllValues.Count().ShouldBe(perGroup*groups);
        ConcurrencyTest.FlyweightTypeGeneric1.AllValues.Count().ShouldBe(perGroup*groups);

        result = null;
        await ForceGarbageCollection();

        ConcurrencyTest.FlyweightTypeGeneric.AllValues.ShouldBeEmpty();
    }

    private async Task ForceGarbageCollection()
    {
        for (int i = 0; i < 3; i++)
        {
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
        
            // Create some memory pressure
            var pressure = new byte[85000];
            await Task.Delay(100, contextAccessor.Current.CancellationToken); // Give GC time to work
        }
    }

    private async Task<IEnumerable<T>> Generate<T>(int count, Func<T> factory)
    {
        await Task.Yield();
        var result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            result.Add(factory());
        }
        return result;
    }
}

public record ConcurrencyTestValueType(string Value);