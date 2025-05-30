﻿namespace PetFoster.SharedKernel;
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

    public static string GetErrorForNonNullableValueWithMaxLimit(string propertyName, int maxValue)
    {
        return $"The {propertyName} cannot be empty and cannot have more than {maxValue} characters";
    }

    public static string GetErrorForNullableValueWithMaxLimit(string propertyName, int maxValue)
    {
        return $"The {propertyName} cannot have more than {maxValue} characters";
    }
}
