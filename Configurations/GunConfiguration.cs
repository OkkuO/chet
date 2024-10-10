using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace chet.Configurations
{
    public class GunConfiguration : IEntityTypeConfiguration<Gun>
    {
        public void Configure(EntityTypeBuilder<Gun> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}