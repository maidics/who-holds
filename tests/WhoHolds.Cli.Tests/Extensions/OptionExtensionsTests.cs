using System.CommandLine;
using WhoHolds.Cli.Extensions;
using WhoHolds.Core.Common.Enums;

namespace WhoHolds.Cli.Tests.Extensions;

public sealed class OptionExtensionsTests
{
    private static ParseResult Parse(params string[] args)
    {
        var option = new Option<IpVersion>("--ip-version");
        option.AcceptDefinedEnumValuesOnly();

        var command = new RootCommand { option };

        return command.Parse(args);
    }

    [Test]
    [Arguments("0")]
    [Arguments("4")]
    [Arguments("6")]
    [Arguments("ipv4")]
    [Arguments("both")]
    public void ShouldAcceptDefinedEnumValues(string value)
    {
        Parse("--ip-version", value).Errors.ShouldBeEmpty();
    }

    [Test]
    public void ShouldAcceptMissingOption()
    {
        Parse().Errors.ShouldBeEmpty();
    }

    [Test]
    [Arguments("1")]
    [Arguments("5")]
    [Arguments("-1")]
    public void ShouldRejectUndefinedEnumValues(string value)
    {
        var error = Parse("--ip-version", value).Errors.ShouldHaveSingleItem();

        error.Message.ShouldBe(
            $"'{value}' is not a valid value for --ip-version. Valid values: Both, IPv4, IPv6."
        );
    }
}
