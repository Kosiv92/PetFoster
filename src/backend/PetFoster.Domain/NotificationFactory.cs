namespace PetFoster.Domain
{
    public static class NotificationFactory
    {
        public static string GetErrorForNullableValueWithRange(string propertyName, int minValue, int maxValue)
        {
            return $"The {propertyName} cannot be empty and must have a length in the range of {minValue}-{maxValue} characters";
        }

        public static string GetErrorForNonNullableValueWithRange(string propertyName, int minValue, int maxValue)
        {
            return $"The {propertyName} must have a length in the range of {minValue}-{maxValue} characters";
        }
    }
}
