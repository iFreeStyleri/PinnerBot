using Microsoft.EntityFrameworkCore;
using Pinner.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pinner.DAL.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder
                .HasKey(t => t.Id);
            builder
                .Property(t => t.Query)
                .HasMaxLength(30)
                .IsRequired();
            builder
                .HasOne(tag => tag.Topic)
                .WithMany(topic => topic.Tags);

        }
    }
}
