using WhoHolds.Core.Common.Models;

namespace WhoHolds.Core.Utility;

internal static class PathUtils
{
    public static Result<string> ResolveFullPath(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);

            if (attributes.HasFlag(FileAttributes.Directory))
                return Result.Failure("Given path is a directory.");

            return Result.Success(Path.GetFullPath(path));
        }
        catch (FileNotFoundException)
        {
            return Result.Failure("File not found.");
        }
        catch (UnauthorizedAccessException)
        {
            return Result.Failure(
                "Access denied. Try running the application as an administrator."
            );
        }
        catch (Exception ex) when (ex is ArgumentException or IOException)
        {
            return Result.Failure("Invalid file path.");
        }
    }
}
