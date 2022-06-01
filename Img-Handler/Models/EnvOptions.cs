using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Models
{
    public class EnvOptions
    {
        //public string ApiKey { get; set; }
        //public string StorageConnectionString { get; set; }
        //public string Func_Code { get; set; }
        public string UrlBase { get; set; }
        public string Num_Days { get; set; }
        public string Table_Name { get; set; }
        public string HttpFunctionPrefix { get; set; }
        public string Cron_Schedule { get; set; }
    }
}
