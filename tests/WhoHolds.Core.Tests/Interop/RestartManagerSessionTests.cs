using System.Diagnostics;
using System.Reflection;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class RestartManagerSessionTests : PathHandlerTestBase
{
    [Test]
    public void ConstructorShouldThrowArgumentOutOfRangeExceptionIf0FilePathsPassed()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new RestartManagerSession([]));
    }

    [Test]
    public void ConstructorShouldThrowIfOneOrMoreFilePathsAreInvalid()
    {
        var invalidFile1 = Guid.NewGuid().ToString();
        var invalidFile2 = Guid.NewGuid().ToString();

        string[] files = [CreateTestFile(), invalidFile1, invalidFile2];

        var ex = Should.Throw<ArgumentException>(() => new RestartManagerSession(files));

        ex.Message.ShouldNotContain(files[0]);
        ex.Message.ShouldContain(invalidFile1);
        ex.Message.ShouldContain(invalidFile2);
    }

    [Test]
    public void ShouldCreateSession()
    {
        string[] files = [CreateTestFile(), CreateTestFile()];

        Should.NotThrow(() => new RestartManagerSession(files));
    }

    [Test]
    public void ShouldStartSessionAndDisposeIt()
    {
        string[] files = [CreateTestFile()];

        var session = new RestartManagerSession(files);

        var result = session.Start();
        result.ShouldBeResultedTo(true);

        var startedField = typeof(RestartManagerSession).GetField(
            "_started",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        startedField.ShouldNotBeNull();
        var started = (bool)startedField.GetValue(session).ShouldNotBeNull();
        started.ShouldBeTrue();

        session.Dispose();

        var disposedField = typeof(RestartManagerSession).GetField(
            "_disposed",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        disposedField.ShouldNotBeNull();
        var disposed = (bool)disposedField.GetValue(session).ShouldNotBeNull();
        disposed.ShouldBeTrue();
    }

    [Test]
    public void StartMethodShouldThrowIfDisposed()
    {
        string[] files = [CreateTestFile()];

        var session = new RestartManagerSession(files);
        session.Dispose();

        Should.Throw<ObjectDisposedException>(() => session.Start());
    }

    [Test]
    public void StartMethodShouldThrowIfAlreadyStarted()
    {
        string[] files = [CreateTestFile()];

        using var session = new RestartManagerSession(files);
        var result = session.Start();
        result.ShouldBeResultedTo(true);

        Should.Throw<InvalidOperationException>(() => session.Start());
    }

    [Test]
    public void DisposeShouldBeIdempotent()
    {
        string[] files = [CreateTestFile()];

        var session = new RestartManagerSession(files);
        session.Dispose();

        Should.NotThrow(() => session.Dispose());
        Should.NotThrow(() => session.Dispose());
        Should.NotThrow(() => session.Dispose());
    }

    [Test]
    public void GetProcessesShouldThrowIfSessionIsDisposed()
    {
        string[] files = [CreateTestFile()];

        var session = new RestartManagerSession(files);
        session.Dispose();

        Should.Throw<ObjectDisposedException>(() => session.GetProcesses(out _));
    }

    [Test]
    public void GetProcessesShouldReturnHolderProcessesForFiles()
    {
        var heldFile = CreateTestFile();
        var nonHeldFile = CreateTestFile();

        string[] files = [heldFile, nonHeldFile];
        using var hold = HoldFile(heldFile);

        using var session = new RestartManagerSession(files);
        session.Start();

        var result = session.GetProcesses(out _);
        result.ShouldBeResultedTo(true);

        var processes = result.Value;
        processes.Length.ShouldBe(1);
        processes.ShouldContain(p => p.Process.dwProcessId == Environment.ProcessId);
    }
}
