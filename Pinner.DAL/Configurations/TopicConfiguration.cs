using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinner.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinner.DAL.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder
                .HasKey(t => t.Id);
            builder
                .Property(t => t.Nickname)
                .IsRequired()
                .HasMaxLength(15);
            builder
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(15);
            builder
                .HasMany(topic => topic.Tags)
                .WithOne(tag => tag.Topic);

        }

    }
}
