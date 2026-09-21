using WhoHolds.Core.Common.Models;

namespace WhoHolds.Core.Utility;

internal static class PathUtils
{
    public static Result CheckFilePath(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            return attributes.HasFlag(FileAttributes.Directory)
                ? Result.Failure("Given path is a directory.")
                : Result.Success();
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
