
using System.Runtime.InteropServices;
using System.Text;

namespace WhoHolds.Core;

/// <summary>
/// Writes to a txt file under artifacts/dump/[os]/[date].txt and additionally to a passed TextWriter.
/// </summary>
internal class DumpWriter : TextWriter
{
    public sealed override Encoding Encoding { get; }

    private readonly StreamWriter _writer;
    private readonly TextWriter _console;

    public DumpWriter(TextWriter console)
    {
        Encoding =  Encoding.UTF8;
        _console = console;
        _writer = new StreamWriter(ResolveFileName(), false, Encoding);
    }

    private static string ResolveFileName()
    {
        string osSubFolder = OperatingSystem.IsWindows() ? "win" :
            OperatingSystem.IsLinux() ? "linux" : throw new PlatformNotSupportedException(RuntimeInformation.OSDescription); // TODO: maybe this should be thrown at application start?
        
        var directory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "dump",  osSubFolder));

        Directory.CreateDirectory(directory);
        
        return Path.Combine(directory, $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.txt");
    }

    public override void Write(char value)
    {
        _writer.Write(value);
        _console.Write(value);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _writer.Dispose();
        }
        
        base.Dispose(disposing);
    }
}
