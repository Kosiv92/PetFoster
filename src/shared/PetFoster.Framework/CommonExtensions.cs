using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace PetFoster.Framework
{
    public static class CommonExtensions
    {
        public static PropertyBuilder<IReadOnlyList<T>> JsonValueObjectCollectionConversion<T>(
        this PropertyBuilder<IReadOnlyList<T>> builder)
        {
            return builder.HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v, JsonSerializerOptions.Default)!,
                new ValueComparer<IReadOnlyList<T>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                    c => c.ToList()));
        }
    }
}
