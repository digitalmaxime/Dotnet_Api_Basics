using EfCore_MySql_CRUD.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore_MySql_CRUD.Infrastructure.FluentMappings;

public class PersonFluentMapping : IEntityTypeConfiguration<Person>
{

        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.Property(t => t.Id).HasColumnName("PersonId");
            builder.HasData(new List<Person>()
            {
                new Person() { Id = 1, Name = "Abou" },
                new Person() { Id = 2, Name = "Ben" },
                new Person() { Id = 3, Name = "Carou" },
                new Person() { Id = 4, Name = "Dan" },
                new Person() { Id = 5, Name = "Emanuel" }
            });
        }
}