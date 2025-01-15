using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class SocialNetContact : ComparableValueObject
    {
        public const int MAX_SOCIAL_NAME_LENGTH = 100;
        public const int MAX_ACCOUNT_NAME_LENGTH = 100;

        private SocialNetContact() { }

        private SocialNetContact(string socialNetName, string accountName) =>
            (SocialNetName, AccountName) = (socialNetName, accountName);
                
        public string SocialNetName { get; }

        public string AccountName { get; }

        public static Result<SocialNetContact, Error> Create(string socialNetName, string accountName)
        {
            if (String.IsNullOrWhiteSpace(socialNetName)
                || socialNetName.Length > SocialNetContact.MAX_SOCIAL_NAME_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"The social network name cannot be empty or contain more than {SocialNetContact.MAX_SOCIAL_NAME_LENGTH} characters");

            if (String.IsNullOrWhiteSpace(accountName)
                || accountName.Length > SocialNetContact.MAX_SOCIAL_NAME_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"The account name cannot be empty or contain more than {SocialNetContact.MAX_ACCOUNT_NAME_LENGTH} characters");

            return new SocialNetContact(socialNetName, accountName);
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return AccountName;
            yield return SocialNetName;
        }
    }
}
