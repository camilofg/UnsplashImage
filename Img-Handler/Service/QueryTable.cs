using Img_Handler.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Service
{
    public class QueryTable
    {
        private static string storageConnString = System.Environment.GetEnvironmentVariable("storageConnectionString");
        private static string tableName = System.Environment.GetEnvironmentVariable("table_name");

        public static List<ImageInfoEntity> QueryImageInfo() {

            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(storageConnString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);

            TableQuery<ImageInfoEntity> query = new TableQuery<ImageInfoEntity>();
            var result = new List<ImageInfoEntity>();

            foreach (ImageInfoEntity entity in table.ExecuteQuery(query)) {
                result.Add(entity);
            }
            return result;
        }
    }
}
