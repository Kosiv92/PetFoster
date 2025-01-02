using CSharpFunctionalExtensions;
using PetFoster.Domain.Entities;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record SocialNetContact
    {
        public const int MAX_SOCIAL_NAME_LENGTH = 100;
        public const int MAX_ACCOUNT_NAME_LENGTH = 100;

        private SocialNetContact() { }

        private SocialNetContact(string socialNetName, string accountName) =>
            (SocialNetName, AccountName) = (socialNetName, accountName);

        public Volunteer Volunteer { get; private set; } = null!;

        public string SocialNetName { get; }

        public string AccountName { get; }

        public static Result<SocialNetContact> Create(string socialNetName, string accountName)
        {
            if (String.IsNullOrWhiteSpace(socialNetName)
                || socialNetName.Length > SocialNetContact.MAX_SOCIAL_NAME_LENGTH)
                return Result.Failure<SocialNetContact>(
                    $"The social network name cannot be empty or contain more than {SocialNetContact.MAX_SOCIAL_NAME_LENGTH} characters");

            if (String.IsNullOrWhiteSpace(accountName)
                || accountName.Length > SocialNetContact.MAX_SOCIAL_NAME_LENGTH)
                return Result.Failure<SocialNetContact>(
                    $"The account name cannot be empty or contain more than {SocialNetContact.MAX_ACCOUNT_NAME_LENGTH} characters");

            return Result.Success<SocialNetContact>(new SocialNetContact(socialNetName, accountName));
        }


    }
}
