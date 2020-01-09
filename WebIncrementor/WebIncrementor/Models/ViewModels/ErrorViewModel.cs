using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIncrementor.Models.ViewModels
{
    public class ErrorViewModel
    {
        public IEnumerable<string> errors { get; set; }

        public ErrorViewModel(params string[] errors)
        {
            this.errors = new List<string>(errors);
        }
    }
}
