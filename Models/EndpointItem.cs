using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Models
{
    public class EndpointItem
    {
        public string Title { get; set; }
        public string PageUrl { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }
}
