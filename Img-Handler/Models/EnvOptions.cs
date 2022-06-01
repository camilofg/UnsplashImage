using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Models
{
    public static class EnvOptions
    {
        public static string ApiKey => Environment.GetEnvironmentVariable("ApiKey");
        public static string StorageConnectionString => Environment.GetEnvironmentVariable("StorageConnectionString");
        public static string Func_Code => Environment.GetEnvironmentVariable("Func_Code");
        public static string UrlBase => Environment.GetEnvironmentVariable("UrlBase");
        public static string Num_Days => Environment.GetEnvironmentVariable("Num_Days");
        public static string Table_Name => Environment.GetEnvironmentVariable("Table_Name");
        public static string HttpFunctionPrefix => Environment.GetEnvironmentVariable("HttpFunctionPrefix");
        public static string Cron_Schedule => Environment.GetEnvironmentVariable("Cron_Schedule");
    }
}
