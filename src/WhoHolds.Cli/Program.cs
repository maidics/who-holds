using WhoHolds.Cli.Commands;
using WhoHolds.Cli.Output;

var root = new GetFileHoldersRootCommand(Console.Out, Console.Error);
root.Subcommands.Add(new GetPortHoldersCommand(Console.Out, Console.Error));
var parseResult = root.Parse(args);

if (parseResult.Errors.Count > 0)
{
    parseResult.Invoke();
    return ExitCodes.Error;
}

return parseResult.Invoke();
