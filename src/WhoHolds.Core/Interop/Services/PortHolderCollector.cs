using System.Diagnostics;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.Services;

internal sealed class PortHolderCollector(int? port)
{
    private readonly List<PortHolderProcess> _holders = [];
    private readonly List<string> _errors = [];
    private bool _anySucceeded;

    public void Collect<TRow>()
        where TRow : unmanaged, INativeSized<TRow>, IPortRowOwnerModule
    {
        var result = PortOwnerModuleTableQuery.Query<TRow>();

        if (!result.Succeeded)
        {
            _errors.AddRange(result.Errors);
            return;
        }

        _anySucceeded = true;

        foreach (var row in result.Value.Entries)
        {
            if (port is null || row.LocalPort == port)
            {
                var moduleInfoResult = PortOwnerModuleInfoQuery.Query(row);

                if (!moduleInfoResult.Succeeded)
                    _errors.AddRange(moduleInfoResult.Errors);

                string? owner = moduleInfoResult.Succeeded ? moduleInfoResult.Value.module : null;
                string? ownerPath = moduleInfoResult.Succeeded
                    ? moduleInfoResult.Value.modulePath
                    : null;
                string? ownerDescription =
                    moduleInfoResult.Succeeded && ownerPath is not null
                        ? FileVersionInfo.GetVersionInfo(ownerPath).FileDescription
                        : null;

                _holders.Add(
                    PortHolderProcess.FromPortRowOwnerModule(
                        row,
                        owner,
                        ownerDescription,
                        ownerPath
                    )
                );
            }
        }
    }

    public Result<PortHolderProcess[]> ToResult() =>
        _anySucceeded
            ? Result.Success(_holders.ToArray(), _errors.ToArray())
            : Result.Failure(_errors.ToArray());
}
