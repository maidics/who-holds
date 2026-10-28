using WhoHolds.Cli.Extensions;
using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Enums;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Commands;

public sealed class GetPortHoldersCommand : Command
{
    private readonly Argument<int?> _port = new("port")
    {
        Description = "Port number (0-65535). Omit to list all ports.",
        Arity = ArgumentArity.ZeroOrOne,
    };

    private readonly Option<PortProtocol> _protocol = new("--protocol")
    {
        Description = "Filter via port protocol: TCP, UDP, both. Omit to list both.",
        Aliases = { "-p" },
        Arity = ArgumentArity.ZeroOrOne,
        DefaultValueFactory = _ => PortProtocol.Both,
    };

    private readonly Option<IpVersion> _ipVersion = new("--ip-version")
    {
        Description = "Filter via IP version: IPv4, IPv6, both. Omit to list both.",
        Aliases = { "-ip", "--ipversion", "-ipv" },
        Arity = ArgumentArity.ZeroOrOne,
        DefaultValueFactory = _ => IpVersion.Both,
    };

    private readonly Option<OutputFormat> _format = new("--format")
    {
        Description = "Output format.",
        Arity = ArgumentArity.ZeroOrOne,
        Aliases = { "-f" },
        DefaultValueFactory = _ => OutputFormat.Text,
    };

    public GetPortHoldersCommand(TextWriter stdout, TextWriter stderr)
        : base(
            "port",
            $"Shows which processes are holding a port.\n\n{PortHolderProcess.DescribeFields()}"
        )
    {
        _protocol.AcceptDefinedEnumValuesOnly();
        _ipVersion.AcceptDefinedEnumValuesOnly();
        _format.AcceptDefinedEnumValuesOnly();

        Arguments.Add(_port);
        Options.Add(_protocol);
        Options.Add(_ipVersion);
        Options.Add(_format);

        SetAction(parseResult =>
        {
            try
            {
                var result = Core.WhoHolds.Port(
                    parseResult.GetValue(_port),
                    parseResult.GetValue(_ipVersion),
                    parseResult.GetValue(_protocol)
                );

                var writer = IResultWriter.For(parseResult.GetValue(_format));

                writer.WritePortHolders(result, stdout, stderr);

                return result.GetExitCode();
            }
            catch (Exception e)
            {
                stderr.WriteLine(e.Message);

                return ExitCodes.Error;
            }
        });
    }
}
