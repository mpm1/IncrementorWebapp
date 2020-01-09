using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebIncrementor.Models
{
    public class Incrementor
    {
        [Key]
        public string UserId { get; set; }

        public ulong Value { get; set; }

        public Incrementor()
        {
            Value = 0;
        }
    }
}
