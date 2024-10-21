using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chet.Models
{
    public class Chats
    {
        public int Id { get; set; }
        public long chatId { get; set; }

        public string msg { get; set; } = "";
    }
}