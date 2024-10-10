using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chet.Models
{
    public class Gun
    {
        public int Id { get; set; }
        public int points { get; set; }
        public long UserId { get; set; }
        public DateTime dateTime{ get; set; }
    }
}