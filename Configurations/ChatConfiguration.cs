using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTelegramBotApi.Types;

namespace chet.Configurations
{
    public class ChatsConfiguration : IEntityTypeConfiguration<Chats>
    {
        public void Configure(EntityTypeBuilder<Chats> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}