using PetFoster.Domain.Entities;

namespace PetFoster.Domain.Shared
{
    public static class Errors
    {
        public static class General
        {
            public static Error ValueIsInvalid(string? name = null)
                => Error.Validation("value.is.invalid", name ?? "Value is invalid");

            public static Error NotFound(Guid? id = null)
                => Error.NotFound("record.not.found", $"Record not found {(id == null ? "" : $"for Id {id}")}");

            public static Error ValueIsRequired(string? name = null)
                => Error.Validation("length.is.invalid", $"invalid{(name == null ? " " : " " + name + " ")}");
        }

        public static class Volunteer
        {
            public static Error AlreadyExist()
            {
                return Error.Validation("record.already.exist",
                    "Volunteer with the same email or phone number is already exist");
            }
        }
    }
}
