using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Models
{
    public class ImageProperties
    {
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public User User { get; set; }
        public Statistics Stats { get; set; } = new Statistics();
    }
}
