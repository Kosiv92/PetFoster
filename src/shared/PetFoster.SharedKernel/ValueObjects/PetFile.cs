﻿namespace PetFoster.SharedKernel.ValueObjects;

public sealed record PetFile
{
    private PetFile() { }

    public PetFile(FilePath pathToStorage)
    {
        PathToStorage = pathToStorage;
    }

    public FilePath PathToStorage { get; }
}
