using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using WhoHolds.Cli.Commands;
using WhoHolds.Cli.Output;
using WhoHolds.Cli.Tests.TestInfrastructure;
using WhoHolds.Core.Common.Enums;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Commands;

public sealed class GetPortHoldersCommandTests
{
    private static (int exitCode, string stdout, string stderr) Run(params string[] args)
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        int exitCode = new GetPortHoldersCommand(stdout, stderr).Parse(args).Invoke();

        return (exitCode, stdout.ToString(), stderr.ToString());
    }

    private static Socket BindOnLoopback(AddressFamily addressFamily, ProtocolType protocolType)
    {
        var socketType = protocolType == ProtocolType.Udp ? SocketType.Dgram : SocketType.Stream;
        var socket = new Socket(addressFamily, socketType, protocolType);
        try
        {
            var loopback =
                addressFamily == AddressFamily.InterNetworkV6
                    ? IPAddress.IPv6Loopback
                    : IPAddress.Loopback;

            socket.Bind(new IPEndPoint(loopback, 0)); // 0 means the OS picks the port

            if (socketType == SocketType.Stream)
                socket.Listen(); // bound-only TCP sockets don't appear in the TCP table

            return socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    private static string PortOf(Socket socket) =>
        ((IPEndPoint)socket.LocalEndPoint!).Port.ToString();

    private static Option GetOption(Command command, string name)
    {
        var option = command.Options.FirstOrDefault(o => o.Name == name);
        option.ShouldNotBeNull();
        return option;
    }

    [Test]
    public void ShouldHaveCorrectNameAndDescription()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        command.Name.ShouldBe("port");
        command.Description.ShouldBe("Shows which processes are holding a port.");
    }

    [Test]
    public void ShouldHavePortArgument()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);
        command.Arguments.Count.ShouldBe(1);

        var arg = command.Arguments[0];
        arg.Name.ShouldBe("port");
        arg.Description.ShouldBe("Port number (0-65535). Omit to list all ports.");
        arg.Arity.ShouldBe(ArgumentArity.ZeroOrOne);
    }

    [Test]
    public void ShouldHaveProtocolOption()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        var option = GetOption(command, "--protocol");
        option.Description.ShouldBe("Filter via port protocol: TCP, UDP, both. Omit to list both.");
        option.Arity.ShouldBe(ArgumentArity.ZeroOrOne);
        option.Aliases.ShouldBe(["-p"], ignoreOrder: true);
        option.HasDefaultValue.ShouldBeTrue();
        ((PortProtocol?)option.GetDefaultValue()).ShouldBe(PortProtocol.Both);
    }

    [Test]
    public void ShouldHaveIpVersionOption()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        var option = GetOption(command, "--ip-version");
        option.Description.ShouldBe("Filter via IP version: IPv4, IPv6, both. Omit to list both.");
        option.Arity.ShouldBe(ArgumentArity.ZeroOrOne);
        option.Aliases.ShouldBe(["-ip", "--ipversion", "-ipv"], ignoreOrder: true);
        option.HasDefaultValue.ShouldBeTrue();
        ((IpVersion?)option.GetDefaultValue()).ShouldBe(IpVersion.Both);
    }

    [Test]
    public void ShouldHaveOutputFormatOption()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        var option = GetOption(command, "--format");
        option.Description.ShouldBe("Output format.");
        option.Arity.ShouldBe(ArgumentArity.ZeroOrOne);
        option.Aliases.ShouldBe(["-f"], ignoreOrder: true);
        option.HasDefaultValue.ShouldBeTrue();
        ((OutputFormat?)option.GetDefaultValue()).ShouldBe(OutputFormat.Text);
    }

    [Test]
    public void ShouldParseNoPortAsNull()
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        var parseResult = command.Parse([]);

        parseResult.Errors.ShouldBeEmpty();
        parseResult.GetValue<int?>("port").ShouldBeNull();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("80.5")]
    public void ShouldHaveParseErrorForNonIntegerPort(string port)
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        command.Parse([port]).Errors.ShouldNotBeEmpty();
    }

    [Test]
    [Arguments("tcp", PortProtocol.Tcp)]
    [Arguments("TCP", PortProtocol.Tcp)]
    [Arguments("udp", PortProtocol.Udp)]
    [Arguments("both", PortProtocol.Both)]
    [Arguments("1", PortProtocol.Tcp)]
    [Arguments("2", PortProtocol.Udp)]
    [Arguments("0", PortProtocol.Both)]
    public void ShouldParseProtocol(string value, PortProtocol expected)
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        foreach (var name in new[] { "--protocol", "-p" })
        {
            var parseResult = command.Parse([name, value]);

            parseResult.Errors.ShouldBeEmpty();
            parseResult.GetValue<PortProtocol>("--protocol").ShouldBe(expected);
        }
    }

    [Test]
    [Arguments("4", IpVersion.IPv4)]
    [Arguments("6", IpVersion.IPv6)]
    [Arguments("ipv4", IpVersion.IPv4)]
    [Arguments("IPv6", IpVersion.IPv6)]
    [Arguments("both", IpVersion.Both)]
    [Arguments("0", IpVersion.Both)]
    public void ShouldParseIpVersion(string value, IpVersion expected)
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        foreach (var name in new[] { "--ip-version", "-ip", "--ipversion", "-ipv" })
        {
            var parseResult = command.Parse([name, value]);

            parseResult.Errors.ShouldBeEmpty();
            parseResult.GetValue<IpVersion>("--ip-version").ShouldBe(expected);
        }
    }

    [Test]
    [Arguments("--protocol", "3")]
    [Arguments("-p", "7")]
    [Arguments("--ip-version", "5")]
    [Arguments("-ipv", "1")]
    public void ShouldHaveParseErrorForUndefinedEnumValue(string option, string value)
    {
        var command = new GetPortHoldersCommand(TextWriter.Null, TextWriter.Null);

        command.Parse([option, value]).Errors.ShouldNotBeEmpty();
    }

    [Test]
    [Arguments("-1")]
    [Arguments("65536")]
    public void ShouldReturnErrorForOutOfRangePort(string port)
    {
        var (exitCode, stdout, stderr) = Run(port);

        exitCode.ShouldBe(ExitCodes.Error);

        stdout.ShouldBeEmpty();
        stderr.ShouldNotBeEmpty();
    }

    [Test]
    [Arguments(AddressFamily.InterNetwork, ProtocolType.Tcp)]
    [Arguments(AddressFamily.InterNetworkV6, ProtocolType.Tcp)]
    [Arguments(AddressFamily.InterNetwork, ProtocolType.Udp)]
    [Arguments(AddressFamily.InterNetworkV6, ProtocolType.Udp)]
    public void ShouldWriteTableWhenPortIsHeld(AddressFamily addressFamily, ProtocolType protocol)
    {
        using var socket = BindOnLoopback(addressFamily, protocol);

        var (exitCode, stdout, _) = Run(PortOf(socket));

        exitCode.ShouldBe(ExitCodes.Held);
        stdout.ShouldContain(Environment.ProcessId.ToString());
    }

    [Test]
    [Arguments("--protocol", "udp")]
    [Arguments("-p", "udp")]
    [Arguments("--ip-version", "6")]
    [Arguments("-ipv", "6")]
    public void ShouldWriteTextWhenFilteredOut(string option, string value)
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetwork, ProtocolType.Tcp);

        var (exitCode, stdout, stderr) = Run(PortOf(socket), option, value);

        exitCode.ShouldBe(ExitCodes.NotHeld);
        stdout.Trim().ShouldBe("No processes are holding the port.");
        stderr.ShouldBeEmpty();
    }

    [Test]
    [Arguments("--format", "json")]
    [Arguments("--format", "JSON")]
    [Arguments("-f", "json")]
    [Arguments("-f", "JSON")]
    public void ShouldReturnJsonWhenOutputFormatIsJson(params string[] jsonOutput)
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetwork, ProtocolType.Tcp);

        var (exitCode, stdout, stderr) = Run([
            PortOf(socket),
            "-p",
            "tcp",
            "-ipv",
            "4",
            .. jsonOutput,
        ]);

        exitCode.ShouldBe(ExitCodes.Held);
        stderr.ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout);
        var v = doc.ShouldHaveResultedTo(true, JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(1);

        var holder = v[0];
        holder
            .GetProperty(
                JsonNamingPolicy.CamelCase.ConvertName(nameof(PortHolderProcess.OwnerProcessId))
            )
            .GetInt32()
            .ShouldBe(Environment.ProcessId);
        holder
            .GetProperty(
                JsonNamingPolicy.CamelCase.ConvertName(nameof(PortHolderProcess.LocalIPAddress))
            )
            .GetString()
            .ShouldBe(IPAddress.Loopback.ToString());
    }
}
