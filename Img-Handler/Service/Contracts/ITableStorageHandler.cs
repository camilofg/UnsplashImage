using Img_Handler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Service.Contracts
{
    public interface ITableStorageHandler
    {
        Task UpsertImageAsync(ImageInfoEntity imageInfo);

        Task<List<ImageInfoEntity>> GetImagesInfo();
    }
}
