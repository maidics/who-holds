namespace WhoHolds.Core.Utility;

public static class ByteFormat
{
    private static readonly string[] _units = ["B", "KB", "MB", "GB"];

    public static string Humanize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";

        double value = bytes;
        int unit = 0;

        while (value >= 1024 && unit < _units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.##} {_units[unit]}";
    }
}
