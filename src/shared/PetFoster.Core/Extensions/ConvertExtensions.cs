namespace PetFoster.Core.Extensions;

public static class ConvertExtensions
{
    public static DateTimeOffset? ConvertToDate(this string inputData)
    {
        if (string.IsNullOrWhiteSpace(inputData))
        {
            return null;
        }

         DateTimeOffset.TryParse(inputData, out DateTimeOffset birthDay);

        return birthDay;
    }
}
