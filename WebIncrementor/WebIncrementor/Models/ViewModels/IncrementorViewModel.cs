using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIncrementor.Models.ViewModels
{
    public class IncrementorViewModel
    {
        public Data data { get; set; }

        public IncrementorViewModel()
        {
            data = new Data();
        }

        public IncrementorViewModel(Incrementor incrementor) : this()
        {
            data.value = incrementor.Value;
        }
    }

    public class Data
    {
        public ulong value { get; set; }
    }
}
