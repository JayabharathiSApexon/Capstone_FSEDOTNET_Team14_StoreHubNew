using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Models.Order;
using StoreHub.Infrastructure.Data;
using StoreHub.Domain.Entities;

namespace StoreHub.Infrastructure.Repositories
{
    public class TrackingDetails : ITrackingDetails
    {
        private readonly AppDbContext _dbcontext;

        public TrackingDetails(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Order?> GetTrackingDetailsAsync(Guid orderId)
        {
            return await _dbcontext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(o => o.TrackingHistory)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
