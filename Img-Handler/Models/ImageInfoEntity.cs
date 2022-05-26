using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Models
{
    public class ImageInfoEntity : TableEntity
    {
        public ImageInfoEntity()
        {
        }

        public ImageInfoEntity(string imageid, string userid)
        {
            PartitionKey = imageid;
            RowKey = userid;
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public int QuantityDownloads { get; set; }
        public double PercentageDownloads { get; set; }
    }
}
