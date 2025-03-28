﻿using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class PetColoration : ComparableValueObject
    {
        public const int MAX_NAME_LENGTH = 100;

        private PetColoration() { }

        public PetColoration(string value) => Value = value;

        public string Value { get; }

        public static Result<PetColoration, Error> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length > PetColoration.MAX_NAME_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"Coloration cannot be empty and contain more than {PetColoration.MAX_NAME_LENGTH} characters");

            return new PetColoration(value);
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }   
}
