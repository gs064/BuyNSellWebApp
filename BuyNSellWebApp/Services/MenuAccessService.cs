using BuyNSellWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Services
{
    public class MenuAccessService : IMenuAccessService
    {
        private readonly ApplicationDbContext _context;

        public MenuAccessService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Type>> GetMenu()
        {
            return await _context.Types.Include(m => m.SubTypes).ToListAsync();
        }
    }
}
