using System.Reflection;
using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.Interop;

[NotInParallel]
internal sealed class ObjectTypesTests
{
    [Before(Test)]
    public void Reset()
    {
        SetInitialized(false);
        SetMap([]);
    }

    private static FieldInfo GetFieldInfo(string fieldName) =>
        typeof(ObjectTypes).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!;

    private static FieldInfo SetInitialized(bool value)
    {
        var field = GetFieldInfo("_initialized");

        field.SetValue(null, value);

        return field;
    }

    private static void SetMap(Dictionary<ushort, string> entries)
    {
        var map = GetMap();

        map.Clear();

        foreach (var kvp in entries)
        {
            map[kvp.Key] = kvp.Value;
        }
    }

    private static Dictionary<ushort, string> GetMap()
    {
        var field = GetFieldInfo("_map");

        return (Dictionary<ushort, string>)field.GetValue(null)!;
    }

    private static void BuildMap()
    {
        var methodInfo = typeof(ObjectTypes).GetMethod(
            "BuildMap",
            BindingFlags.NonPublic | BindingFlags.Static
        )!;

        methodInfo.Invoke(null, null);
    }

    [Test]
    public void GetNameShouldBuildMapWhenInitializedIsFalse()
    {
        try
        {
            ObjectTypes.GetName(1);
        }
        catch
        {
            // ignored: passed ushort key might not be valid
        }

        var initialized = (bool)GetFieldInfo("_initialized").GetValue(null)!;
        initialized.ShouldBeTrue();

        var map = GetMap();
        map.Count.ShouldNotBe(0);
    }

    [Test]
    public void ShouldReturnObjectTypeIfMapContainsIt()
    {
        SetInitialized(true);
        SetMap(new Dictionary<ushort, string> { [1] = "File", [2] = "Dictionary" });

        var name = ObjectTypes.GetName(1);

        name.ShouldBe("File");
    }

    [Test]
    public void ShouldBuildAgainIfNotFound()
    {
        SetInitialized(true);

        try
        {
            ObjectTypes.GetName(1);
        }
        catch
        {
            // ignored: passed ushort key might not be valid
        }

        var map = GetMap();
        map.Count.ShouldNotBe(0);
    }

    [Test]
    public void ShouldThrowArgumentExceptionIfNotFoundAfterRebuilding()
    {
        SetInitialized(true);

        var ex = Should.Throw<ArgumentException>(() => ObjectTypes.GetName(ushort.MaxValue));
        ex.Message.ShouldStartWith("Failed to find object name for object type");

        var map = GetMap();
        map.Count.ShouldNotBe(0);
    }
}
