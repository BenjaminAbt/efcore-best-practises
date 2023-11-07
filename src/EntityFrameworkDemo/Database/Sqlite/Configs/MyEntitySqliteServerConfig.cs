using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Configs;

// This is the entity configuration when used with Sqlite

public class MyEntitySqliteServerConfig : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> b)
    {
        b.ToTable("MyEntities");

        b.HasKey(e => e.Id);

        // you can add custom Sqlite configs here.
    }
}
