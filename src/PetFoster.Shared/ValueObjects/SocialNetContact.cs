using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFoster.Core.ValueObjects;

public sealed class SocialNetContact : ComparableValueObject
{
    public const int MAX_SOCIAL_NAME_LENGTH = 100;
    public const int MAX_ACCOUNT_NAME_LENGTH = 100;

    [JsonConstructor]
    private SocialNetContact(string socialNetName, string accountName)
    {
        (SocialNetName, AccountName) = (socialNetName, accountName);
    }

    public string SocialNetName { get; }

    public string AccountName { get; }

    public static Result<SocialNetContact, Error> Create(string socialNetName, string accountName)
    {
        return string.IsNullOrWhiteSpace(socialNetName)
            || socialNetName.Length > MAX_SOCIAL_NAME_LENGTH
            ? (Result<SocialNetContact, Error>)Errors.General.ValueIsInvalid(
                $"The social network name cannot be empty or contain more than {MAX_SOCIAL_NAME_LENGTH} characters")
            : string.IsNullOrWhiteSpace(accountName)
            || accountName.Length > MAX_SOCIAL_NAME_LENGTH
            ? (Result<SocialNetContact, Error>)Errors.General.ValueIsInvalid(
                $"The account name cannot be empty or contain more than {MAX_ACCOUNT_NAME_LENGTH} characters")
            : (Result<SocialNetContact, Error>)new SocialNetContact(socialNetName, accountName);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return AccountName;
        yield return SocialNetName;
    }
}
