namespace WhoHolds.Core.Interop;

internal readonly record struct SystemClass(int Value, string Name)
{
    public const int SYSTEM_BASIC_INFORMATION = 0;
    public const int SYSTEM_HANDLE_INFORMATION_EX = 64;

    public const int OBJECT_TYPES_INFORMATION = 3;

    public static SystemClass SystemBasicInformation =>
        new(SYSTEM_BASIC_INFORMATION, nameof(SYSTEM_BASIC_INFORMATION));

    public static SystemClass SystemHandleInformationEx =>
        new(SYSTEM_HANDLE_INFORMATION_EX, nameof(SYSTEM_HANDLE_INFORMATION_EX));

    public static SystemClass ObjectTypesInformation =>
        new(OBJECT_TYPES_INFORMATION, nameof(OBJECT_TYPES_INFORMATION));

    public override string ToString()
    {
        return $"{Name}:  {Value}";
    }
}
