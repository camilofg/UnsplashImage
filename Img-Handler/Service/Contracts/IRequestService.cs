using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler.Service.Contracts
{
    public interface IRequestService
    {
        Task<string> CallApiAsync(string url);
    }
}
