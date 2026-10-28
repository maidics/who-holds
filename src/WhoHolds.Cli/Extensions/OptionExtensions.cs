namespace WhoHolds.Cli.Extensions;

public static class OptionExtensions
{
    extension<TEnum>(Option<TEnum> option)
        where TEnum : struct, Enum
    {
        // enum parsing accepts any integer (e.g. "5" => (IpVersion)5), so reject undefined members
        public void AcceptDefinedEnumValuesOnly()
        {
            option.Validators.Add(result =>
            {
                var value = result.GetValueOrDefault<TEnum>();

                if (!Enum.IsDefined(value))
                    result.AddError(
                        $"'{value}' is not a valid value for {option.Name}. Valid values: {string.Join(", ", Enum.GetNames<TEnum>())}."
                    );
            });
        }
    }
}
