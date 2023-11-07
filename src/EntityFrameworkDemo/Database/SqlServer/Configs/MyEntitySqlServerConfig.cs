using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Configs;

// This is the entity configuration when used with SqlServer

public class MyEntitySqlServerConfig : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> b)
    {
        b.ToTable("MyEntities");

        b.HasKey(e => e.Id);

        // you can add custom SqlServer configs here.
        // e.g. limit name length to 4000 (which is the max indexable length for unicode)
        b.Property(e => e.Name)
            .IsRequired()
            .IsUnicode(true)
            .HasMaxLength(4000);
    }
}
