using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Services
{
    public interface IMenuAccessService
    {
        Task<List<Models.Type>> GetMenu();
    }
}
