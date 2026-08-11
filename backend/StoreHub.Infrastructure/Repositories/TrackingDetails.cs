using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Models.Order;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Infrastructure.Repositories
{
    public class TrackingDetails : ITrackingDetails
    {
        private readonly AppDbContext _dbcontext;

        public TrackingDetails(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<TrackingResponseModel?> GetTrackingDetails(Guid orderId)
        {
            var order = await _dbcontext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(o => o.TrackingHistory)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            return new TrackingResponseModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.OrderDate.AddDays(3),
                TotalAmount = order.TotalAmount,

                Products = order.OrderItems
                    .Select(item => new TrackingProductModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,

                        ImageUrl = item.Product.ProductImages
                            .Where(x => x.IsPrimary)
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => x.ImageUrl)
                            .FirstOrDefault()

                            ?? item.Product.ProductImages
                                .OrderBy(x => x.DisplayOrder)
                                .Select(x => x.ImageUrl)
                                .FirstOrDefault()

                            ?? string.Empty
                    })
                    .ToList(),

                TrackingHistory = order.TrackingHistory
                    .OrderBy(x => x.StatusDate)
                    .Select(x => new TrackingStatusModel
                    {
                        Status = x.Status,
                        StatusDate = x.StatusDate,
                        Completed = true
                    })
                    .ToList()
            };
        }
    }
}
