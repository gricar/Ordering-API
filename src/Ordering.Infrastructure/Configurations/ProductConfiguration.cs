﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasConversion(
                        productId => productId.Value,
                        dbId => ProductId.Of(dbId));

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
    }
}
