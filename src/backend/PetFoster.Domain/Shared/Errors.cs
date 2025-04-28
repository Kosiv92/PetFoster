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

            public static Error ReferenceExist()
            {
                return Error.Conflict("record.is.referenced",
                    "Record is referenced by another records");
            }
        }

        public static class Volunteer
        {
            public static Error IdAlreadyExist()
            {
                return Error.Validation("record.already.exist",
                    "Volunteer with the same id is already exist");
            }

            public static Error AlreadyExist()
            {
                return Error.Validation("record.already.exist",
                    "Volunteer with the same email or phone number is already exist");
            }
        }

        public static class Specie
        {
            public static Error AlreadyExist()
            {
                return Error.Validation("record.already.exist",
                    "Specie with the same name is already exist");
            }
        }
    }
}
