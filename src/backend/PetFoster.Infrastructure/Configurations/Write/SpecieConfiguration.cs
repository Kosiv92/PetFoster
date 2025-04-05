﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Infrastructure.Configurations.Write
{
    internal class SpecieConfiguration : IEntityTypeConfiguration<Specie>
    {
        public void Configure(EntityTypeBuilder<Specie> builder)
        {
            builder.ToTable("species");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => SpecieId.Create(value));

            builder.Property(m => m.Name)
                .HasMaxLength(SpecieName.MAX_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => SpecieName.Create(value).Value);

            builder.HasMany(m => m.Breeds)
                .WithOne(m => m.Specie)
                .HasForeignKey("specie_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
