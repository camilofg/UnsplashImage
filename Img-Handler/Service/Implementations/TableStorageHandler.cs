using Img_Handler.Models;
using Img_Handler.Service.Contracts;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Service.Implementations
{
    public class TableStorageHandler : ITableStorageHandler
    {

        private static string storageConnString = EnvOptions.StorageConnectionString;
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public TableStorageHandler()
        {
            _storageAccount = CloudStorageAccount.Parse(storageConnString);
            _tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _table = _tableClient.GetTableReference(EnvOptions.Table_Name);
        }
        public Task<List<ImageInfoEntity>> GetImagesInfo()
        {
            TableQuery<ImageInfoEntity> query = new TableQuery<ImageInfoEntity>();
            var result = new List<ImageInfoEntity>();

            foreach (ImageInfoEntity entity in _table.ExecuteQuery(query))
            {
                result.Add(entity);
            }
            return Task.FromResult(result);
        }

        public async Task UpsertImageAsync(ImageInfoEntity imageInfo)
        {
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(imageInfo);

            TableResult result = await _table.ExecuteAsync(insertOrReplaceOperation);
            ImageInfoEntity insertedImageInfo = result.Result as ImageInfoEntity;

            Console.WriteLine("Inserted register");
        }
    }
}
