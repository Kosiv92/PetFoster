﻿using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record BreedName
    {
        public const int MIN_NAME_LENGTH = 2;
        public const int MAX_NAME_LENGTH = 200;

        private BreedName() { }

        public BreedName(string value) => Value = value;

        public string Value { get; }

        public static Result<BreedName> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value)
                || value.Length > MAX_NAME_LENGTH
                || value.Length < MIN_NAME_LENGTH)
            {
                return Result.Failure<BreedName>(NotificationFactory
                    .GetErrorForNonNullableValueWithRange(nameof(BreedName), MIN_NAME_LENGTH, MAX_NAME_LENGTH));
            }

            return Result.Success<BreedName>(new BreedName(value));
        }
    }    
}
